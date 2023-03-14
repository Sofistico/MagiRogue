using GoRogue.GameFramework;
using GoRogue.Pathing;
using GoRogue.SpatialMaps;
using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Veggies;
using MagiRogue.Utils;
using MagiRogue.Utils.Extensions;
using Microsoft.Toolkit.HighPerformance;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadConsole;
using SadConsole.Entities;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MagiRogue.GameSys
{
    /// <summary>
    /// Stores, manipulates and queries Tile data, uses GoRogue Map class
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [JsonConverter(typeof(MapJsonConverter))]
    public sealed class Map : GoRogue.GameFramework.Map, IDisposable
    {
        #region Fields

        private MagiEntity _gameObjectControlled;
        private SadConsole.Entities.Renderer _entityRender;
        private bool _disposed;

        #endregion Fields

        #region Properties

        /// <summary>
        /// All cell tiles of the map, it's a TileBase array, should never be directly declared to create new tiles, rather
        /// it must use <see cref="Map.SetTerrain(IGameObject)"/>.
        /// </summary>
        public TileBase[] Tiles { get; private set; }

        /// <summary>
        /// Fires whenever FOV is recalculated.
        /// </summary>
        public event EventHandler FOVRecalculated;

        /// <summary>
        /// Fires whenever the value of <see cref="ControlledEntitiy"/> is changed.
        /// </summary>
        public event EventHandler<ControlledGameObjectChangedArgs> ControlledGameObjectChanged;

        public MagiEntity ControlledEntitiy
        {
            get => _gameObjectControlled;

            set
            {
                if (_gameObjectControlled != value)
                {
                    MagiEntity oldObject = _gameObjectControlled;
                    _gameObjectControlled = value;
                    ControlledGameObjectChanged?.Invoke(this, new ControlledGameObjectChangedArgs(oldObject));
                }
            }
        }

        public Point LastPlayerPosition { get; set; } = Point.None;
        public string MapName { get; set; }
        public bool NeedsUpdate { get; set; }
        public bool IsCurrentMap { get; set; }
        public bool IsActive { get; set; }
        public uint MapId { get; private set; }
        public ulong Seed { get; set; }
        public int ZAmount { get; set; }
        public Dictionary<Direction, Map> MapZoneConnections { get; set; }
        public List<Room> Rooms { get; set; }

        public Renderer EntityRender { get => _entityRender; }
        //public Light[] Ilumination { get; set; }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Build a new map with a specified width and height, has entity layers,
        /// they being 0-Furniture, 1-ghosts, 2-Items, 3-Actors, 4-Player.
        /// \nMaps can have any size, but the default is 60x60, for a nice 3600 tiles per map
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Map(string mapName, int width = 60, int height = 60, bool usesWeighEvaluation = true) :
            base(CreateTerrain(width, height), Enum.GetNames(typeof(MapLayer)).Length - 1,
            Distance.Euclidean,
            entityLayersSupportingMultipleItems: LayerMasker.Default.Mask
            ((int)MapLayer.VEGETATION, (int)MapLayer.ITEMS, (int)MapLayer.GHOSTS, (int)MapLayer.PLAYER))
        {
            Tiles = (ArrayView<TileBase>)((LambdaSettableTranslationGridView<TileBase, IGameObject>)Terrain).BaseGrid;

            // Treat the fov as a component.
            GoRogueComponents.Add
                (new MagiRogueFOVVisibilityHandler(this, Color.DarkSlateGray, (int)MapLayer.GHOSTS));

            _entityRender = new SadConsole.Entities.Renderer();
            MapName = mapName;
            MapZoneConnections = new();

            // pathfinding
            if (usesWeighEvaluation)
            {
                var weights = new LambdaGridView<double>(Width, Height, pos =>
                {
                    var tile = Tiles[pos.ToIndex(Width)];
                    return tile is not null ? MathMagi.Round((double)((double)tile.MoveTimeCost / 100)) : 0;
                });
                AStar = new AStar(WalkabilityView, Distance.Euclidean, weights, 0.01);
            }
            //Ilumination = new Light[Width * Height];
        }

        #endregion Constructor

        #region HelperMethods

        private static ISettableGridView<IGameObject?> CreateTerrain(int width, int heigth)
        {
            var goRogueTerrain = new ArrayView<TileBase>(width, heigth);
            return new LambdaSettableTranslationGridView<TileBase, IGameObject?>(goRogueTerrain, t => t, g => g as TileBase);
        }

        public void RemoveAllEntities()
        {
            foreach (MagiEntity item in Entities.Items.Cast<MagiEntity>())
            {
                Remove(item);
            }
        }

        public void SetSeed(ulong seed, uint x, uint y, uint i)
        {
            Seed = seed + x + y + (x * x) + (y * y) + (x * y) - (x + y + i);
        }

        /// <summary>
        /// IsTileWalkable checks
        /// to see if the actor has tried
        /// to walk off the map or into a non-walkable tile
        /// Returns true if the tile location is walkable
        /// false if tile location is not walkable or is off-map
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsTileWalkable(Point location)
        {
            // first make sure that actor isn't trying to move
            // off the limits of the map
            if (CheckForIndexOutOfBounds(location))
                return false;

            // then return whether the tile is walkable
            return !Tiles[(location.Y * Width) + location.X].IsBlockingMove;
        }

        /// <summary>
        /// IsTileWalkable checks
        /// to see if the actor has tried
        /// to walk off the map or into a non-walkable tile
        /// Returns true if the tile location is walkable
        /// false if tile location is not walkable or is off-map
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsTileWalkable(Point location, Actor actor)
        {
            // first make sure that actor isn't trying to move
            // off the limits of the map
            if (CheckForIndexOutOfBounds(location))
                return false;
            if (actor.IgnoresWalls)
                return true;

            // then return whether the tile is walkable
            return !Tiles[(location.Y * Width) + location.X].IsBlockingMove;
        }

        /// <summary>
        /// this method will make sure that each room has it's properties all connected up!
        /// </summary>
        public void UpdateRooms()
        {
            if (Rooms is null)
                return;
            for (int i = 0; i < Rooms.Count; i++)
            {
                var room = Rooms[i];
                for (int t = 0; t < room.DoorsPoint.Count; t++)
                {
                    TileDoor door = GetTileAt<TileDoor>(room.DoorsPoint[t]);
                    room.Doors.Add(door);
                }
            }
        }

        /// <summary>
        /// Checking whether a certain type of
        /// entity is at a specified location the manager's list of entities
        /// and if it exists, return that Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <returns></returns>
        public T GetEntityAt<T>(Point location) where T : MagiEntity
        {
            return Entities.GetItemsAt(location).OfType<T>().FirstOrDefault(e => e.CanInteract);
        }

        public WaterTile GetClosestWaterTile(int range, Point position)
        {
            var waters = GetAllTilesOfType<WaterTile>();
            return GetClosest<WaterTile>(position, range, waters);
        }

        public static T? GetClosest<T>(Point originPos, int range, List<T> listT) where T : IGameObject
        {
            T closest = default;
            double bestDistance = double.MaxValue;

            foreach (T t in listT)
            {
                if (t is null) continue;
                if (t is not Player)
                {
                    double distance = Point.EuclideanDistanceMagnitude(originPos, t.Position);

                    if (distance < bestDistance && (distance <= range || range == 0))
                    {
                        bestDistance = distance;
                        closest = t;
                    }
                }
            }

            return closest;
        }

        public static T? GetClosest<T>(Point originPos, int range, T[] listT) where T : IGameObject
        {
            T closest = default;
            double bestDistance = double.MaxValue;

            foreach (T t in listT)
            {
                if (t is null) continue;
                if (t is not Player)
                {
                    double distance = Point.EuclideanDistanceMagnitude(originPos, t.Position);

                    if (distance < bestDistance && (distance <= range || range == 0))
                    {
                        bestDistance = distance;
                        closest = t;
                    }
                }
            }

            return closest;
        }

        public static T? GetClosest<T>(Point originPos, int range, ReadOnlySpan<T> listT) where T : IGameObject
        {
            T closest = default;
            double bestDistance = double.MaxValue;

            foreach (T t in listT)
            {
                if (t is null) continue;
                if (t is not Player)
                {
                    double distance = Point.EuclideanDistanceMagnitude(originPos, t.Position);

                    if (distance < bestDistance && (distance <= range || range == 0))
                    {
                        bestDistance = distance;
                        closest = t;
                    }
                }
            }

            return closest;
        }

        public bool EntityIsThere(Point pos)
        {
            MagiEntity? entity = GetEntityAt<MagiEntity>(pos);
            return entity is not null;
        }

        /// <summary>
        /// Removes an Entity from the Entities Field
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(MagiEntity entity)
        {
            if (Entities.Contains(entity))
            {
                RemoveEntity(entity);

                _entityRender.Remove(entity);

                // Link up the entity's Moved event to a new handler
                entity.Moved -= OnEntityMoved;

                _entityRender.IsDirty = true;
            }
        }

        /// <summary>
        /// Adds an Entity to the Entities field
        /// </summary>
        /// <param name="entity"></param>
        public void AddMagiEntity(MagiEntity entity)
        {
            /*if (entity.CurrentMap is not null)
            {
                Map map = (Map)entity.CurrentMap;
                map.ControlledEntitiy = null;
                map.Remove(entity);
            }*/

            try
            {
                AddEntity(entity);
            }
            catch (ArgumentException)
            {
                entity.Position = GetRandomWalkableTile();
                AddEntity(entity);
#if DEBUG
                Debug.Print("An entity tried to telefrag in a place where it couldn't");
#endif
            }
            // Initilizes the field of view of the player, will do different for monsters
            if (entity is Player player)
            {
                FovCalculate(player);
                ControlledEntitiy = player;
                ForceFovCalculation();
                LastPlayerPosition = player.Position;
            }

            _entityRender.Add(entity);

            // Link up the entity's Moved event to a new handler
            entity.Moved += OnEntityMoved;
        }

        /// <summary>
        /// When the Entity's .Moved value changes, it triggers this event handler
        /// which updates the Entity's current position in the SpatialMap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEntityMoved(object? sender, GameObjectPropertyChanged<Point>? args)
        {
            if (args.Item is Player player)
            {
                FovCalculate(player);
                LastPlayerPosition = player.Position;
            }

            _entityRender.IsDirty = true;
        }

        /// <summary>
        /// really snazzy way of checking whether a certain type of
        /// tile is at a specified location in the map's Tiles
        /// and if it exists, return that Tile
        /// accepts an x/y coordinate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>

        public T GetTileAt<T>(int x, int y) where T : TileBase
        {
            int locationIndex = Point.ToIndex(x, y, Width);

            // make sure the index is within the boundaries of the map!
            return locationIndex <= Width * Height && locationIndex >= 0 ? Tiles[locationIndex] is T t ? t : null : null;
        }

        public TileBase GetTileAt(int x, int y)
        {
            return GetTileAt<TileBase>(x, y);
        }

        /// <summary>
        /// Checks if a specific type of tile at a specified location
        /// is on the map. If it exists, returns that Tile
        /// This form of the method accepts a Point coordinate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <returns></returns>
        public T GetTileAt<T>(Point location) where T : TileBase
        {
            return GetTileAt<T>(location.X, location.Y);
        }

        public TileBase GetTileAt(Point location)
        {
            return GetTileAt<TileBase>(location);
        }

        public void RemoveAllTiles()
        {
            if (Tiles is null)
                return;
            try
            {
                foreach (TileBase tile in Tiles)
                {
                    if (tile != null)
                        RemoveTerrain(tile);
                }
            }
            catch (Exception)
            {
                GameLoop.WriteToLog("Tried to destroy a tile that didn't exist!");
            }
        }

        /// <summary>
        /// Gets the entity by it's id
        /// </summary>
        /// <typeparam name="T">Any type of entity</typeparam>
        /// <param name="id">The id of the entity to find</param>
        /// <returns>Returns the entity owner of the id</returns>

        public MagiEntity GetEntityById(uint id)
        {
            // TODO: this shit is wonky, need to do something about it
            var filter = from entity in Entities.Items
                         where entity.ID == id
                         select entity;

            if (filter.Any())
            {
                return (MagiEntity)filter.FirstOrDefault();
            }
            return null;
        }

        public void ConfigureRender(ScreenSurface renderer)
        {
            if (renderer.SadComponents.Contains(_entityRender))
            {
                return;
            }
            _entityRender = new();
            renderer.SadComponents.Add(_entityRender);
            _entityRender.DoEntityUpdate = true;
            _entityRender.AddRange(Entities.Items.Cast<MagiEntity>());
            renderer.IsDirty = true;
        }

        public string SaveMapToJson(Player player)
        {
            if (LastPlayerPosition == Point.None && player.CurrentMap == this)
                LastPlayerPosition = player.Position;
            var json = JsonConvert.SerializeObject(
                this,
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            IsActive = false;

            return json;
        }

        private void FovCalculate(Actor actor)
        {
            /*if (PlayerFOV.CurrentFOV.Count() >= actor.Stats.ViewRadius)*/
            if (actor.GetAnatomy().CanSee)
            {
                PlayerFOV.Calculate(actor.Position, actor.GetViewRadius(), Radius.Circle);
                FOVRecalculated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Forcefully sets a seed to a map, if you want to generate a new seed for the map,
        /// use <see cref="SetSeed(int, int, int)"/>
        /// </summary>
        /// <param name="seed">The seed to replace</param>
        internal void SetSeed(ulong seed)
        {
            Seed = seed;
        }

        /// <summary>
        /// Fix for the FOV problem that sometimes don't work
        /// </summary>
        public void ForceFovCalculation()
        {
            Actor actor = (Actor)ControlledEntitiy;
            FovCalculate(actor);
        }

        /// <summary>
        /// This is used to get a random point that is walkable inside the map, mainly used when adding an entity
        /// and there is already an entity there, so it picks another random location.
        /// </summary>
        /// <returns>Returns an Point to a tile that is walkable and there is no actor there</returns>
        public Point GetRandomWalkableTile()
        {
            var rng = GoRogue.Random.GlobalRandom.DefaultRNG;
            Point rngPoint = new Point(rng.NextInt(Width - 1), rng.NextInt(Height - 1));

            while (!IsTileWalkable(rngPoint))
                rngPoint = new Point(rng.NextInt(Width - 1), rng.NextInt(Height - 1));

            return rngPoint;
        }

        /// <summary>
        /// Returns if the Point is inside the index of the map, makes sure that nothing tries to go outside the map.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>

        public bool CheckForIndexOutOfBounds(Point point)
        {
            return point.X < 0 || point.Y < 0
                || point.X >= Width || point.Y >= Height;
        }

        private string GetDebuggerDisplay()
        {
            return MapName;
        }

        public void SetId(uint id) => MapId = id;

        public FastAStar AStarWithAllWalkable()
        {
            int count = Tiles.Length;
            ArrayView<bool> mapView = new ArrayView<bool>(Width, Height);
            for (int i = 0; i < count; i++)
            {
                mapView[i] = true;
            }
            return new FastAStar(mapView, DistanceMeasurement);
        }

        /// <summary>
        /// Returns the rectangle containing the bounds of the map
        /// </summary>
        /// <returns></returns>

        public Rectangle MapBounds() => Terrain.Bounds();

        public List<TileBase> ReturnAllTrees()
        {
            List<TileBase> result = new List<TileBase>();
            foreach (TileBase tree in Tiles)
            {
                if (tree.Name.Equals("Tree"))
                {
                    result.Add(tree);
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the room to the map.
        /// </summary>
        /// <param name="r"></param>
        public void AddRoom(Room r)
        {
            Rooms ??= new List<Room>();
            if (!CheckIfRoomFitsInsideMap(r))
            {
                try
                {
                    FindOtherPlaceForRoom(r);
                }
                catch (ApplicationException)
                {
                    throw;
                }
            }
            Rooms.Add(r);
        }

        public Room AddRoom(RoomTemplate template, Point pointBegin)
        {
            var r = template.ConfigureRoom(pointBegin);
            AddRoom(r);
            SpawnRoomThingsOnMap(r);
            return r;
        }

        public bool CheckIfRoomFitsInsideMap(Room r)
        {
            return r.RoomRectangle.Position.X <= Width || r.RoomRectangle.Position.Y <= Height;
        }

        private void FindOtherPlaceForRoom(Room r)
        {
            int newRoomX = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(1, Width - r.RoomRectangle.Width);
            int newRoomY = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(1, Height - r.RoomRectangle.Height);

            r.ChangeRoomPos(newRoomX, newRoomY);
            if (!CheckIfRoomFitsInsideMap(r))
            {
                throw new ApplicationException("Tried to place a room inside a map that cound't fit it!");
            }
            if (r.PositionsRoom().Any(CheckForIndexOutOfBounds))
            {
                throw new ApplicationException("Tried to place a room inside a map that cound't fit it!");
            }
        }

        /// <summary>
        /// Adds a list of rooms to the map.
        /// </summary>
        /// <param name="r"></param>

        public void AddRooms(List<Room> r)
        {
            Rooms ??= new List<Room>();
            Rooms.AddRange(r);
        }

        public void SpawnRoomThingsOnMap(Room r)
        {
            Point[] posRoom = r.PositionsRoom();

            for (int x = 0; x < r.Template.Obj.Rows.Length; x++)
            {
                string currentRow = r.Template.Obj.Rows[x];
                for (int y = 0; y < currentRow.Length; y++)
                {
                    char c = currentRow[y];
                    Point pos = posRoom[Point.ToIndex(x, y, r.Template.Obj.Rows.Length)];
                    if (CheckForIndexOutOfBounds(pos))
                    {
                        // skip over if not in the map
                        continue;
                    }
                    r.Terrain.TryGetValue(c.ToString(), out var ter);
                    TryToPutTerrain(pos, ter);
                    if (r.Furniture is not null)
                    {
                        r.Furniture.TryGetValue(c.ToString(), out var fur);
                        TryToPutFurniture(pos, fur);
                    }
                }
            }
        }

        private void TryToPutFurniture(Point pos, object? fur)
        {
            if (fur is not null)
            {
                string str;
                if (fur is JArray array)
                {
                    str = ParseRandomChance(array);
                }
                else
                {
                    str = fur.ToString();
                }
                try
                {
                    Furniture furniture = DataManager.QueryFurnitureInData(str);
                    furniture.Position = pos;
                    AddMagiEntity(furniture);
                }
                catch (NullReferenceException ex)
                {
                    throw new NullReferenceException($"Tried to create a room with a non existent furniture! \n" +
                        $"Furniture: {str}, Exception: {ex}");
                }
            }
        }

        private void TryToPutTerrain(Point pos, object? ter)
        {
            if (ter is not null)
            {
                string str;
                if (ter is JArray array)
                {
                    str = ParseRandomChance(array);
                }
                else
                {
                    str = ter.ToString();
                }
                try
                {
                    TileBase tile;
                    if (str.Equals("debug_tree"))
                    {
                        tile = TileEncyclopedia.GenericTree(pos);
                    }
                    else if (str.Equals("t_grass"))
                    {
                        tile = TileEncyclopedia.GenericGrass(pos);
                    }
                    else
                    {
                        tile = DataManager.QueryTileInData(str);
                        tile.Position = pos;
                    }
                    SetTerrain(tile);
                }
                catch (NullReferenceException ex)
                {
                    throw new NullReferenceException($"Tried to create a room with a non existent tile! \n" +
                        $"Tile: {str}, Exception: {ex}");
                }
            }
        }

        private static string ParseRandomChance(JArray array)
        {
            List<string> strs = new();
            for (int i = 0; i < array.Count; i++)
            {
                var child = array[i];
                var s = child.ToString();
                strs.Add(s);
            }
            int nmbrOfObjects = strs.Count;
            string obj = "";
            while (string.IsNullOrEmpty(obj))
            {
                string s = strs[GameLoop.GlobalRand.NextInt(0, nmbrOfObjects)];
                // one in ten
                int mod = 10;
                if (s.StartsWith('['))
                {
                    var test = s.Split("\r\n");
                    mod -= int.Parse(test[2]);
                }
                bool gotIt = Mrn.OneIn(mod);
                if (gotIt)
                {
                    obj = s.StartsWith('[') ? s.Split("\r\n")[1].Replace(',', ' ').Replace('\"', ' ').Trim()
                        : s;
                }
            }

            return obj;
        }

        #endregion HelperMethods

        #region Dispose

        public void DestroyMap()
        {
            RemoveAllEntities();
            RemoveAllTiles();
            if (GoRogueComponents.Count > 0)
                GoRogueComponents.GetFirstOrDefault<FOVHandler>().DisposeMap();
            Tiles = null;
            ControlledGameObjectChanged = null;
            this.ControlledEntitiy = null;
            _entityRender = null;
            GoRogueComponents.Clear();
            _disposed = true;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                DestroyMap();
                GC.SuppressFinalize(this);
            }
        }

        public IGameObject FindTypeOfFood(Food whatToEat, Point entityPos)
        {
            const int defaultSearchRange = 25;

            switch (whatToEat)
            {
                case Food.Carnivore:
                    var meat = GetClosest(entityPos, defaultSearchRange, GetAllMeatsEvenAlive());
                    if (meat is not null)
                    {
                        return meat;
                    }
                    break;

                case Food.Herbivore:
                    var plant = GetClosest(entityPos, defaultSearchRange, GetAllPlantsEvenItem());
                    if (plant is not null)
                    {
                        return plant;
                    }
                    break;

                case Food.Omnivere:
                    // well if it works...
                    var objList = new List<IGameObject>();
                    var meatO = GetClosest(entityPos, defaultSearchRange, GetAllMeatsEvenAlive());
                    if (meatO is not null)
                    {
                        objList.Add(meatO);
                    }
                    var plantO = GetClosest(entityPos, defaultSearchRange, GetAllPlantsEvenItem());
                    if (plantO is not null)
                    {
                        objList.Add(plantO);
                    }
                    return objList.GetRandomItemFromList();

                default:
                    return null;
            }
            return null;
        }

        private ReadOnlySpan<MagiEntity> GetAllMeatsEvenAlive()
        {
            var meats = Entities.GetLayersInMask(LayerMasker.Mask((int)MapLayer.ACTORS, (int)MapLayer.ITEMS));
            var list = new List<MagiEntity>();
            foreach (var item in meats)
            {
                if (item is Item deadMeat && deadMeat.Material.Type == MaterialType.Meat)
                {
                    list.Add(deadMeat);
                    continue;
                }
                list.Add((MagiEntity)item);
            }

            return list.AsSpan();
        }

        private IGameObject[] GetAllPlantsEvenItem()
        {
            var items = Entities.GetLayer((int)MapLayer.ITEMS).Where(i => i.Item is Item item && (item.ItemType == ItemType.PlantFood)).Select(i => i.Item).ToArray();
            var tilesWithVeggies = Array.FindAll(Tiles, i => i.HoldsVegetation && i.Vegetations.Any());
            IGameObject[] result = new IGameObject[tilesWithVeggies.Length + items.Length];
            int offSet = 0;
            for (int i = 0; i < tilesWithVeggies.Length; i++)
            {
                TileBase? item = tilesWithVeggies[i];
                for (int x = 0; x < item.Vegetations.Length; x++)
                {
                    result[i] = item.Vegetations[x];
                    item.Vegetations[x].Position = item.Position;
                    offSet++;
                }
            }
            // this smells like good stuff!
            // TODO: Great stuff here!
            for (int i = 0; i < items.Length; i++)
            {
                result[i + offSet] = items[i];
            }
            return result;
        }

        public TFind[] GetAllTilesOfType<TFind>()
        {
            return Tiles.OfType<TFind>().ToArray();
        }

        ~Map()
        {
            Dispose();
        }

        #endregion Dispose
    }

    #region Event

    public class ControlledGameObjectChangedArgs : EventArgs
    {
        public MagiEntity OldObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldObject"/>
        public ControlledGameObjectChangedArgs(MagiEntity oldObject) => OldObject = oldObject;
    }

    #endregion Event
}