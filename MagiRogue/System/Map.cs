using GoRogue;
using GoRogue.GameFramework;
using GoRogue.SpatialMaps;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using MagiRogue.System.Time;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using System;
using System.Diagnostics;
using System.Linq;

namespace MagiRogue.System
{
    /// <summary>
    /// Stores, manipulates and queries Tile data, uses GoRogue Map class
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
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
        { get { return _tiles; } set { _tiles = value; } }

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

        public Point? LastPlayerPosition { get; set; }

        public string MapName { get; }
        public bool NeedsUpdate { get; internal set; }
        public bool IsActive { get; set; }
        public uint MapId { get; private set; }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Build a new map with a specified width and height, has entity layers,
        /// they being 0-Furniture, 1-ghosts, 2-Items, 3-Actors, 4-Player.
        /// \nAll Maps must have 60x60 size, for a total of 3600 tiles inside of it, for chunck loading.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Map(string mapName, int width = 60, int height = 60) :
            base(CreateTerrain(width, height), Enum.GetNames(typeof(MapLayer)).Length - 1,
            Distance.Euclidean,
            entityLayersSupportingMultipleItems: LayerMasker.DEFAULT.Mask
            ((int)MapLayer.ITEMS, (int)MapLayer.GHOSTS, (int)MapLayer.PLAYER))
        {
            Tiles = (ArrayView<TileBase>)((LambdaSettableTranslationGridView<TileBase, IGameObject>)Terrain).BaseGrid;

            // Treat the fov as a component.
            GoRogueComponents.Add
                (new MagiRogueFOVVisibilityHandler(this, Color.DarkSlateGray, (int)MapLayer.GHOSTS));

            _entityRender = new SadConsole.Entities.Renderer();
            MapName = mapName;
        }

        #endregion Constructor

        #region HelperMethods

        private static ISettableGridView<IGameObject> CreateTerrain(int width, int heigth)
        {
            var goRogueTerrain = new ArrayView<TileBase>(width, heigth);
            return new LambdaSettableTranslationGridView<TileBase, IGameObject>(goRogueTerrain, t => t, g => g as TileBase);
        }

        public void RemoveAllEntities()
        {
            foreach (Entity item in Entities.Items)
            {
                Remove(item);
            }
        }

        public void UnloadFromMemory()
        {
            IsActive = false;
        }

        public void LoadToMemory()
        {
            IsActive = true;
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
            RemoveEntity(entity);

            _entityRender.Remove(entity);

            // Link up the entity's Moved event to a new handler
            entity.Moved -= OnEntityMoved;

            _entityRender.IsDirty = true;
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
        private void OnEntityMoved(object sender, GameObjectPropertyChanged<Point> args)
        {
            if (args.Item is Player player)
            {
                FovCalculate(player);
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

        private void FovCalculate(Actor actor)
        {
            /*if (PlayerFOV.CurrentFOV.Count() >= actor.Stats.ViewRadius)*/
            PlayerFOV.Calculate(actor.Position, actor.Stats.ViewRadius, Radius.Circle);
            FOVRecalculated?.Invoke(this, EventArgs.Empty);
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
            Point rngPoint = new Point(rng.Next(Width - 1), rng.Next(Height - 1));

            while (!IsTileWalkable(rngPoint))
                rngPoint = new Point(rng.Next(Width - 1), rng.Next(Height - 1));

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

        #endregion HelperMethods

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