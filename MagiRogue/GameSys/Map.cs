using GoRogue;
using GoRogue.DiceNotation;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using GoRogue.SpatialMaps;
using MagiRogue.Data;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Time;
using MagiRogue.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MagiRogue.GameSys
{
    /// <summary>
    /// Stores, manipulates and queries Tile data, uses GoRogue Map class
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [JsonConverter(typeof(MapJsonConverter))]
    public class Map : GoRogue.GameFramework.Map
    {
        #region Properties

        private TileBase[] _tiles; // Contains all tiles objects
        private Entity _gameObjectControlled;
        private SadConsole.Entities.Renderer _entityRender;

        /// <summary>
        /// All cell tiles of the map, it's a TileBase array, should never be directly declared to create new tiles, rather
        /// it must use <see cref="Map.SetTerrain(IGameObject)"/>.
        /// </summary>
        public TileBase[] Tiles
        { get { return _tiles; } private set { _tiles = value; } }

        /// <summary>
        /// Fires whenever FOV is recalculated.
        /// </summary>
        public event EventHandler FOVRecalculated;

        /// <summary>
        /// Fires whenever the value of <see cref="ControlledEntitiy"/> is changed.
        /// </summary>
        public event EventHandler<ControlledGameObjectChangedArgs> ControlledGameObjectChanged;

        public Entity ControlledEntitiy
        {
            get => _gameObjectControlled;

            set
            {
                if (_gameObjectControlled != value)
                {
                    Entity oldObject = _gameObjectControlled;
                    _gameObjectControlled = value;
                    ControlledGameObjectChanged?.Invoke(this, new ControlledGameObjectChangedArgs(oldObject));
                }
            }
        }

        public Point LastPlayerPosition { get; set; } = Point.None;
        public string MapName { get; set; }
        public bool NeedsUpdate { get; internal set; }
        public bool IsCurrentMap { get; set; }
        public bool IsActive { get; set; }
        public uint MapId { get; private set; }
        public ulong Seed { get; set; }
        public int ZAmount { get; set; }
        public Dictionary<Direction, Map> MapZoneConnections { get; set; }
        public List<Room> Rooms { get; set; }
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
        public Map(string mapName, int width = 60, int height = 60) :
            base(CreateTerrain(width, height), Enum.GetNames(typeof(MapLayer)).Length - 1,
            Distance.Euclidean,
            entityLayersSupportingMultipleItems: LayerMasker.Default.Mask
            ((int)MapLayer.ITEMS, (int)MapLayer.GHOSTS, (int)MapLayer.PLAYER))
        {
            Tiles = (ArrayView<TileBase>)((LambdaSettableTranslationGridView<TileBase, IGameObject>)Terrain).BaseGrid;

            // Treat the fov as a component.
            GoRogueComponents.Add
                (new MagiRogueFOVVisibilityHandler(this, Color.DarkSlateGray, (int)MapLayer.GHOSTS));

            _entityRender = new SadConsole.Entities.Renderer();
            MapName = mapName;
            MapZoneConnections = new();
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
            foreach (Entity item in Entities.Items)
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
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
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
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
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
        public T GetEntityAt<T>(Point location) where T : Entity
        {
            return Entities.GetItemsAt(location).OfType<T>().FirstOrDefault(e => e.CanInteract);
        }

        public Entity GetClosestEntity(Point originPos, int range)
        {
            Entity closest = null;
            double bestDistance = double.MaxValue;

            foreach (Entity entity in Entities.Items)
            {
                if (entity is not Player)
                {
                    double distance = Point.EuclideanDistanceMagnitude(originPos, entity.Position);

                    if (distance < bestDistance && (distance <= range || range == 0))
                    {
                        bestDistance = distance;
                        closest = entity;
                    }
                }
            }

            return closest;
        }

        /// <summary>
        /// Removes an Entity from the Entities Field
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(Entity entity)
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
        public void Add(Entity entity)
        {
            if (entity.CurrentMap is not null)
            {
                Map map = (Map)entity.CurrentMap;
                map.ControlledEntitiy = null;
                map.Remove(entity);
            }

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
            if (locationIndex <= Width * Height && locationIndex >= 0)
            {
                return Tiles[locationIndex] is T t ? t : null;
            }
            else return null;
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
            foreach (TileBase tile in Tiles)
            {
                RemoveTerrain(tile);
            }
        }

        /// <summary>
        /// Gets the entity by it's id
        /// </summary>
        /// <typeparam name="T">Any type of entity</typeparam>
        /// <param name="id">The id of the entity to find</param>
        /// <returns>Returns the entity owner of the id</returns>
        public Entity GetEntityById(uint id)
        {
            // TODO: this shit is wonky, need to do something about it
            var filter = from entity in Entities.Items
                         where entity.ID == id
                         select entity;

            if (filter.Any())
            {
                return (Entity)filter.FirstOrDefault();
            }
            return null;
        }

        public void ConfigureRender(SadConsole.Console renderer)
        {
            if (renderer.SadComponents.Contains(_entityRender))
            {
                return;
            }
            _entityRender = new();
            renderer.SadComponents.Add(_entityRender);
            _entityRender.DoEntityUpdate = true;
            foreach (Entity item in Entities.Items)
            {
                _entityRender.Add(item);
            }
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
            if (actor.Anatomy.CanSee)
            {
                PlayerFOV.Calculate(actor.Position, actor.Stats.ViewRadius, Radius.Circle);
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
            if (point.X < 0 || point.Y < 0
                || point.X >= Width || point.Y >= Height)
            {
                return true;
            }

            return false;
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
            FastAStar astar = new FastAStar(mapView, DistanceMeasurement);

            return astar;
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
            if (Rooms is null)
                Rooms = new List<Room>();
            if (!CheckIfRoomFitsInsideMap(r))
            {
                try
                {
                    FindOtherPlaceForRoom(r);
                }
                catch (ApplicationException ex)
                {
                    throw ex;
                }
            }
            Rooms.Add(r);
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
        }

        /// <summary>
        /// Adds a list of rooms to the map.
        /// </summary>
        /// <param name="r"></param>
        public void AddRooms(List<Room> r)
        {
            if (Rooms is null)
                Rooms = new List<Room>();
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
                    r.Terrain.TryGetValue(c.ToString(), out var ter);
                    r.Furniture.TryGetValue(c.ToString(), out var fur);
                    TryToPutTerrain(pos, ter);
                    TryToPutFurniture(pos, fur);
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
                    Add(furniture);
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
                    TileBase tile = DataManager.QueryTileInData(str);
                    tile.Position = pos;
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
                    obj = s.StartsWith('[') ? s.Split("\r\n")[1].Replace(',', ' ').Trim() : s;
                }
            }

            return obj;
        }

        public void DestroyMap()
        {
            RemoveAllEntities();
            RemoveAllTiles();
            GoRogueComponents.GetFirstOrDefault<FOVHandler>().DisposeMap();
            Tiles = null;
            ControlledGameObjectChanged = null;
            this.ControlledEntitiy = null;
            _entityRender = null;
            GoRogueComponents.Clear();
        }

        #endregion HelperMethods

        /*

                #region Desconstructor

                ~Map()
                {
        #if DEBUG
                    // This is here because i suspect there is a minor memory leak in the map class, with this here
                    // at the very least it seems that the memory is not that great
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();
                    //GC.Collect();
        #endif

                    foreach (Entity item in Entities.Items)
                    {
                        Remove(item);
                    }
                    Tiles = null;
                    ControlledGameObjectChanged = null;
                    this.ControlledEntitiy = null;
                    _entityRender = null;
                    GoRogueComponents.GetFirstOrDefault<FOVHandler>().DisposeMap();
                    GoRogueComponents.Clear();
        #if DEBUG
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();
                    //GC.Collect();
        #endif
                }

                #endregion Desconstructor

        */
    }

    // enum for defining maplayer for things, so that a monster and a player can occupy the same tile as
    // an item for example.
    // If it stops working, add back the player map layer
    public enum MapLayer
    {
        TERRAIN,
        GHOSTS,
        ITEMS,
        ACTORS,
        FURNITURE,
        PLAYER
    }

    #region Event

    public class ControlledGameObjectChangedArgs : EventArgs
    {
        public Entity OldObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldObject"/>
        public ControlledGameObjectChangedArgs(Entity oldObject) => OldObject = oldObject;
    }

    #endregion Event
}