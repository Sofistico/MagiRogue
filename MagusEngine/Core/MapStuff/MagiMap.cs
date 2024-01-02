using Arquimedes.Enumerators;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using GoRogue.Random;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.ECS;
using MagusEngine.ECS.Components.ActorComponents;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Factory;
using MagusEngine.Serialization.MapConverter;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadConsole;
using SadConsole.Entities;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives.SpatialMaps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagusEngine.Core.MapStuff
{
    /// <summary>
    /// Stores, manipulates and queries Tile data, uses GoRogue Map class
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    [JsonConverter(typeof(MapJsonConverter))]
    public sealed class MagiMap : GoRogue.GameFramework.Map, IDisposable
    {
        #region Fields

        private MagiEntity? _gameObjectControlled;
        private EntityManager _entityManager;
        private bool _disposed;
        private Dictionary<Func<Actor, bool>, Actor[]> _lastCalledActors = [];
        private bool _needsToUpdateActorsDict;
        private Lazy<Tile[]> lazyTiles = new();

        private readonly Dictionary<uint, IGameObject> _idMap;
        private readonly EntityRegistry _registry = new(500);

        #endregion Fields

        #region Properties

        /// <summary>
        /// Fires whenever FOV is recalculated.
        /// </summary>
        public event EventHandler? FOVRecalculated;

        /// <summary>
        /// Fires whenever the value of <see cref="ControlledEntitiy"/> is changed.
        /// </summary>
        public event EventHandler<ControlledGameObjectChangedArgs>? ControlledGameObjectChanged;

        public MagiEntity? ControlledEntitiy
        {
            get => _gameObjectControlled;

            set
            {
                if (_gameObjectControlled != value)
                {
                    MagiEntity? oldObject = _gameObjectControlled;
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
        public Dictionary<Direction, MagiMap> MapZoneConnections { get; set; }
        public List<Room>? Rooms { get; set; }

        public EntityManager EntityRender { get => _entityManager; }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Build a new map with a specified width and height, has entity layers, they being
        /// 0-Furniture, 1-ghosts, 2-Items, 3-Actors, 4-Player. \nMaps can have any size, but the
        /// default is 60x60, for a nice 3600 tiles per map
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public MagiMap(string mapName, int width = 60, int height = 60, bool usesWeighEvaluation = true) :
            base(width, height, Enum.GetNames(typeof(MapLayer)).Length - 1,
            Distance.Euclidean,
            entityLayersSupportingMultipleItems: LayerMasker.Default.Mask((int)MapLayer.ITEMS,
                (int)MapLayer.GHOSTS,
                (int)MapLayer.ACTORS,
                (int)MapLayer.SPECIAL))
        {
            // Treat the fov as a component.
            GoRogueComponents.Add
                (new MagiRogueFOVVisibilityHandler(this, Color.DarkSlateGray, (int)MapLayer.GHOSTS));

            _entityManager = new();
            MapName = mapName;
            MapZoneConnections = new();

            // pathfinding
            if (usesWeighEvaluation)
            {
                var weights = new LambdaGridView<double>(Width, Height, pos =>
                {
                    var tile = (Tile?)Terrain[pos.ToIndex(Width)];
                    return tile is not null ? MathMagi.Round((double)((double)tile.MoveTimeCost / 100)) : 0;
                });
                AStar = new AStar(WalkabilityView, Distance.Euclidean, weights, 0.01);
            }

            _idMap = new();
            ObjectAdded += OnObjectAdded;
            ObjectRemoved += OnObjectRemoved;
        }

        #endregion Constructor

        #region Methods

        public void RemoveAllEntities()
        {
            foreach (IGameObject item in Entities.Items)
            {
                if (item is MagiEntity entity)
                    RemoveMagiEntity(entity);
                else
                    RemoveEntity(item);
            }

            _registry.RemoveComponentAll();
        }

        public void SetSeed(ulong seed, uint x, uint y, uint i)
        {
            Seed = seed + x + y + (x * x) + (y * y) + (x * y) - (x + y + i);
        }

        /// <summary>
        /// IsTileWalkable checks to see if the actor has tried to walk off the map or into a
        /// non-walkable tile Returns true if the tile location is walkable false if tile location
        /// is not walkable or is off-map
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsTileWalkable(Point location)
        {
            // first make sure that actor isn't trying to move off the limits of the map
            if (CheckForIndexOutOfBounds(location))
                return false;

            // then return whether the tile is walkable
            return Terrain[(location.Y * Width) + location.X]?.IsWalkable == true;
        }

        /// <summary>
        /// IsTileWalkable checks to see if the actor has tried to walk off the map or into a
        /// non-walkable tile Returns true if the tile location is walkable false if tile location
        /// is not walkable or is off-map
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public bool IsTileWalkable(Point location, MagiEntity entity)
        {
            // first make sure that actor isn't trying to move off the limits of the map
            if (CheckForIndexOutOfBounds(location))
                return false;
            if (entity.IgnoresWalls)
                return true;

            // then return whether the tile is walkable
            return Terrain[(location.Y * Width) + location.X]?.IsWalkable == true;
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
                    Tile? door = GetTileAt<DoorComponent>(room.DoorsPoint[t]);
                    room.Doors.Add(door);
                }
            }
        }

        /// <summary>
        /// Checking whether a certain type of entity is at a specified location the manager's list
        /// of entities and if it exists, return that Entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <returns></returns>
        public T? GetEntityAt<T>(Point location) where T : MagiEntity
        {
            return Entities.GetItemsAt(location).OfType<T>().FirstOrDefault(e => e.CanInteract);
        }

        public Tile GetClosestWaterTile(int range, Point position)
        {
            var waters = GetAllTilesWithComponents<WaterTile>();

            return position.GetClosest<Tile>(range, waters, null);
        }

        public Actor[] GetAllActors(Func<Actor, bool>? actionToRunInActors = null)
        {
            if (_lastCalledActors.TryGetValue(actionToRunInActors!, out Actor[]? value)
                && !_needsToUpdateActorsDict)
            {
                return value;
            }
            _lastCalledActors.Clear();
            _needsToUpdateActorsDict = false;
            Actor[] search;
            if (actionToRunInActors is null)
            {
                search = Entities.GetLayer((int)MapLayer.ACTORS).Items.Cast<Actor>().ToArray();
            }
            else
            {
                search = Entities.GetLayer((int)MapLayer.ACTORS).Items.Cast<Actor>().Where(actionToRunInActors).ToArray();
            }
            _lastCalledActors.TryAdd(actionToRunInActors, search);

            return search;
        }

        public bool EntityIsThere(Point pos)
        {
            MagiEntity? entity = GetEntityAt<MagiEntity>(pos);
            return entity is not null;
        }

        public bool EntityIsThere<T>(Point pos, out T? entity) where T : MagiEntity
        {
            entity = GetEntityAt<T>(pos);
            return entity is not null;
        }

        /// <summary>
        /// Removes an Entity from the Entities Field
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveMagiEntity(MagiEntity entity)
        {
            if (Entities.Contains(entity))
            {
                RemoveEntity(entity);

                // Link up the entity's Moved event to a new handler
                entity.PositionChanged -= OnPositionChanged;

                _needsToUpdateActorsDict = true;
                _registry.Destroy(entity.ID);
            }
            if (_entityManager.Contains(entity.SadCell))
            {
                _entityManager.Remove(entity.SadCell);
                _entityManager.IsDirty = true;
            }
        }

        /// <summary>
        /// Adds an Entity to the Entities field
        /// </summary>
        /// <param name="entity"></param>
        public void AddMagiEntity(MagiEntity entity)
        {
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
                entity.PositionChanged += OnPositionChanged;
            }

            _entityManager.Add(entity.SadCell);

            _needsToUpdateActorsDict = true;
        }

        public void AddComponentToEntity<T>(uint id, T component)
        {
            _registry.AddComponent(id, component);
        }

        /// <summary>
        /// When the Entity's .Moved value changes, it triggers this event handler which updates the
        /// Entity's current position in the SpatialMap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPositionChanged(object? sender, ValueChangedEventArgs<Point>? args)
        {
            if (sender is Player player)
            {
                FovCalculate(player);
                LastPlayerPosition = player.Position;
            }
            _entityManager.IsDirty = true;
        }

        /// <summary>
        /// really snazzy way of checking whether a certain type of tile is at a specified location
        /// in the map's Tiles and if it exists, return that Tile accepts an x/y coordinate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile? GetTileAt<T>(int x, int y)
        {
            int locationIndex = Point.ToIndex(x, y, Width);
            var tile = locationIndex <= Width * Height && locationIndex >= 0
                ? (Tile?)Terrain[locationIndex]
                : null;
            if (tile?.GoRogueComponents.Contains(typeof(T)) == false && typeof(T) != typeof(Tile))
            {
                return default;
            }

            // make sure the index is within the boundaries of the map!
            return tile;
        }

        /// <summary>
        /// Checks if a specific type of tile at a specified location is on the map. If it exists,
        /// returns that Tile This form of the method accepts a Point coordinate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="location"></param>
        /// <returns></returns>
        public Tile? GetTileAt<T>(Point location)
        {
            return GetTileAt<T>(location.X, location.Y);
        }

        public T? GetComponentInTileAt<T>(Point location, string? tag = null) where T : class
        {
            return GetTileAt<T>(location.X, location.Y)?.GetComponent<T>(tag);
        }

        public Tile? GetTileAt(Point location) => GetTileAt<Tile>(location);

        /// <summary>
        /// Gets the entity by it's id
        /// </summary>
        /// <typeparam name="T">Any type of entity</typeparam>
        /// <param name="id">The id of the entity to find</param>
        /// <returns>Returns the entity owner of the id</returns>
        public MagiEntity GetEntityById(uint id) => (MagiEntity)_idMap[id];

        /// <summary>
        /// Gets the entity by it's id
        /// </summary>
        /// <typeparam name="T">Any type of entity</typeparam>
        /// <param name="id">The id of the entity to find</param>
        /// <returns>Returns the entity owner of the id</returns>
        public T SafeGetEntityById<T>(uint id) where T : IGameObject
        {
            // TODO: this shit is wonky, need to do something about it
            var filter = Entities.FirstOrDefault(i => i.Item.ID == id);

            if (filter != default)
            {
                return (T)filter.Item;
            }
            return default;
        }

        public void ConfigureRender(ScreenSurface renderer)
        {
            if (renderer.SadComponents.Contains(_entityManager))
            {
                return;
            }
            _entityManager = new();
            renderer.SadComponents.Add(_entityManager);
            _entityManager.DoEntityUpdate = true;

            foreach (var spatialMap in Entities.GetLayersInMask(LayerMasker.MaskAllAbove((int)MapLayer.TERRAIN)))
            {
                _entityManager.AddRange(spatialMap.Items.Cast<MagiEntity>().Select(i => i.SadCell));
            }
            //_entityRender.AddRange(Entities.Items.Cast<MagiEntity>());
            renderer.IsDirty = true;
        }

        public string SaveMapToJson(Player player)
        {
            if (LastPlayerPosition == Point.None && player.CurrentMap == this)
                LastPlayerPosition = player.Position;
            var json = JsonConvert.SerializeObject(
                this,
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
        /// Forcefully sets a seed to a map, if you want to generate a new seed for the map, use <see cref="SetSeed(int, int, int)"/>
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
        /// This is used to get a random point that is walkable inside the map, mainly used when
        /// adding an entity and there is already an entity there, so it picks another random location.
        /// </summary>
        /// <returns>Returns an Point to a tile that is walkable and there is no actor there</returns>
        public Point GetRandomWalkableTile()
        {
            var rng = GoRogue.Random.GlobalRandom.DefaultRNG;
            Point rngPoint = new(rng.NextInt(Width - 1), rng.NextInt(Height - 1));

            while (!IsTileWalkable(rngPoint))
                rngPoint = new Point(rng.NextInt(Width - 1), rng.NextInt(Height - 1));

            return rngPoint;
        }

        /// <summary>
        /// This is used to get a random point that is random inside the map.
        /// </summary>
        /// <returns>Returns an Point that is random inside the map</returns>
        public Point GetRandomPos()
        {
            return new Point(GlobalRandom.DefaultRNG.NextInt(Width - 1), GlobalRandom.DefaultRNG.NextInt(Height - 1));
        }

        /// <summary>
        /// Returns if the Point is inside the index of the map, makes sure that nothing tries to go
        /// outside the map.
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
            int count = Terrain.Count;
            ArrayView<bool> mapView = new(Width, Height);
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

        public List<Tile> ReturnAllTrees()
        {
            List<Tile> result = new();
            foreach (Point p in Terrain.Positions())
            {
                var tree = (Tile?)Terrain[p];
                if (tree?.Name?.Equals("Tree") == true)
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
            int tries = 1000;
            while (!CheckIfRoomFitsInsideMap(r) && --tries != 0)
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
            if (r.RoomPoints.Any(CheckForIndexOutOfBounds))
                return false;
            return r.RoomRectangle.Position.X <= Width || r.RoomRectangle.Position.Y <= Height;
        }

        private void FindOtherPlaceForRoom(Room r)
        {
            int newRoomX = GlobalRandom.DefaultRNG.NextInt(0, Width - r.RoomRectangle.Width);
            int newRoomY = GlobalRandom.DefaultRNG.NextInt(0, Height - r.RoomRectangle.Height);

            r.ChangeRoomPos(newRoomX, newRoomY);
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
            Point[] posRoom = r.RoomPoints;

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
                    throw new NullReferenceException("Tried to create a room with a non existent furniture! \n" +
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
                    Tile tile;
                    if (str.Equals("debug_tree"))
                    {
                        tile = TileFactory.GenericTree(pos);
                    }
                    else if (str.Equals("t_grass"))
                    {
                        tile = TileFactory.GenericGrass(pos);
                    }
                    else
                    {
                        var arr = str.Split(';');
                        tile = TileFactory.CreateTile(pos, Enum.Parse<TileType>(arr[0]), arr[1]);
                    }
                    SetTerrain(tile);
                }
                catch (NullReferenceException ex)
                {
                    throw new NullReferenceException("Tried to create a room with a non existent tile! \n" +
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
                string s = strs[GlobalRandom.DefaultRNG.NextInt(0, nmbrOfObjects)];
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

        private void OnObjectRemoved(object? sender, ItemEventArgs<IGameObject> e)
        {
            if (e.Item is not Tile)
                _idMap.Remove(e.Item.ID);
        }

        private void OnObjectAdded(object? sender, ItemEventArgs<IGameObject> e)
        {
            if (e.Item is not Tile)
                _idMap[e.Item.ID] = e.Item;
        }

        public ColoredGlyph[]? GetTilesAppearence()
        {
            if (!lazyTiles.IsValueCreated)
            {
                var glyphs = new ColoredGlyph[Terrain.Count];
                lazyTiles = new Lazy<Tile[]>(new Tile[Terrain.Count]);
                for (int i = 0; i < Terrain.Count; i++)
                {
                    if (Terrain[i] is not Tile tile)
                        continue;
                    lazyTiles.Value[i] = tile;
                    glyphs[i] = tile.Appearence;
                }
                return glyphs;
            }
            else
            {
                return Array.ConvertAll(lazyTiles.Value, i => i.Appearence);
            }
        }

        #endregion Methods

        #region overrides

        public override bool Equals(object? obj)
        {
            // Check if obj is null
            if (obj == null)
            {
                return false;
            }

            // Check if obj is of the same type
            if (obj.GetType() != GetType())
            {
                return false;
            }

            // Compare hash codes
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Seed, MapId);
        }

        #endregion overrides

        #region Dispose

        public void DestroyMap()
        {
            RemoveAllEntities();
            if (GoRogueComponents.Count > 0)
                GoRogueComponents.GetFirstOrDefault<FOVHandler>()?.DisposeMap();
            _lastCalledActors.Clear();
            _lastCalledActors = null!;
            ControlledGameObjectChanged = null!;
            ControlledEntitiy = null!;
            _entityManager = null!;
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

        public IGameObject FindTypeOfFood(Food whatToEat, IGameObject entity)
        {
            const int defaultSearchRange = 25;

            foreach (var objId in _registry.CompView<FoodComponent>())
            {
                var searchEntity = _idMap[objId];
                var foodComp = _registry.GetComponent<FoodComponent>(searchEntity.ID);
                if (foodComp.FoodType.HasFlag(whatToEat)
                    && searchEntity.Position.GetDistance(entity.Position) <= defaultSearchRange)
                {
                    return searchEntity;
                }
            }

            return null;
        }

        public Tile[] GetAllTilesWithComponents<TFind>() where TFind : class
        {
            List<Tile> tiles = new();
            foreach (var item in Terrain.Positions())
            {
                var tile = (Tile?)Terrain[item];
                if (tile?.HasComponent<TFind>() == true)
                {
                    tiles.Add(tile);
                }
            }
            return tiles.ToArray();
        }

        public List<Room?> FindRoomsByTag(RoomTag tag) => Rooms?.FindAll(i => i.Tag == tag)!;

        ~MagiMap()
        {
            Dispose();
        }

        #endregion Dispose
    }

    #region Event

    public class ControlledGameObjectChangedArgs : EventArgs
    {
        public MagiEntity? OldObject;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldObject"/>
        public ControlledGameObjectChangedArgs(MagiEntity? oldObject) => OldObject = oldObject;
    }

    #endregion Event
}
