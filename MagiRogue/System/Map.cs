﻿using GoRogue;
using GoRogue.MapViews;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using SadConsole;
using System.Linq;

namespace MagiRogue.System
{
    // Stores, manipulates and queries Tile data
    public class Map
    {
        #region Properties

        private TileBase[] _tiles; // Contains all tiles objects
        private int _width;
        private int _height;

        /// <summary>
        /// All cell tiles of the map, it's a TileBase array
        /// </summary>
        public TileBase[] Tiles { get { return _tiles; } set { _tiles = value; } }

        /// <summary>
        /// Width of the map
        /// </summary>
        public int Width { get { return _width; } set { _width = value; } }

        /// <summary>
        /// Height of the map
        /// </summary>
        public int Height { get { return _height; } set { _height = value; } }

        /// <summary>
        /// Keeps track of all the Entities on the map
        /// </summary>
        public MultiSpatialMap<Entity> Entities;

        /// <summary>
        /// A static IDGenerator that all Entities can access
        /// </summary>
        public static IDGenerator IDGenerator = new IDGenerator();

        /// <summary>
        /// Holds the information of all walkables positions
        /// </summary>
        public IMapView<bool> WalkabilityMap { get; }

        /// <summary>
        /// pathfinding property
        /// </summary>
        public Pathfinding Pathfinding { get; }

        /// <summary>
        /// Holds the information of the view map
        /// </summary>
        public IMapView<bool> ViewMap { get; }

        /// <summary>
        /// Holds the information of all explored tiles, true for explored and false for not explored, change color if not
        /// explored.
        /// </summary>
        //public IMapView<bool> ExploredMap;

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Build a new map with a specified width and height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Map(int width, int height)
        {
            _width = width;
            _height = height;
            Tiles = new TileBase[width * height];
            Entities = new MultiSpatialMap<Entity>();
            ArrayMap<TileBase> viewTiles = new ArrayMap<TileBase>(Tiles, _height);
            WalkabilityMap = new LambdaTranslationMap<TileBase, bool>(viewTiles, val => val.IsBlockingMove);
            ViewMap = new LambdaTranslationMap<TileBase, bool>(viewTiles, val => val.IsBlockingSight);
            Pathfinding = new Pathfinding(WalkabilityMap);
            //ExploredMap = new LambdaTranslationMap<TileBase, bool>(viewTiles, a => a.IsExplored);
        }

        #endregion Constructor

        #region HelperMethods

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
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
            // then return whether the tile is walkable
            return !_tiles[location.Y * Width + location.X].IsBlockingMove;
        }

        public bool IsTileVisible(Point location)
        {
            return false;
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
            return Entities.GetItems(location).OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Removes an Entity from the MultiSpatialMap
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(Entity entity)
        {
            // Remove from spatial map
            Entities.Remove(entity);

            // Clears the memory of the field of view
            if (entity is Actor actor)
            {
                actor.FieldOfViewSystem.Reset();
            }

            // Link up the entity's Moved event to a new handler
            entity.Moved -= OnEntityMoved;
        }

        /// <summary>
        /// Adds an Entity to the MultiSpatialMap
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Entity entity)
        {
            // add entity to spatial map
            Entities.Add(entity, entity.Position);

            // Initilizes the field of view of the player, will do different for monsters
            if (entity is Actor actor)
            {
                actor.FieldOfViewSystem.Initialize(ViewMap);
                actor.FieldOfViewSystem.Calculate();
            }

            // Link up the entity's Moved event to a new handler
            entity.Moved += OnEntityMoved;
        }

        /// <summary>
        /// When the Entity's .Moved value changes, it triggers this event handler
        /// which updates the Entity's current position in the SpatialMap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEntityMoved(object sender, Entity.EntityMovedEventArgs args)
        {
            if (args.Entity is Actor actor)
            {
                actor.FieldOfViewSystem.Calculate();
            }

            Entities.Move(args.Entity as Entity, args.Entity.Position);
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
            int locationIndex = Helpers.GetIndexFromPoint(x, y, Width);
            // make sure the index is within the boundaries of the map!
            if (locationIndex <= Width * Height && locationIndex >= 0)
            {
                return Tiles[locationIndex] is T ? (T)Tiles[locationIndex] : null;
            }
            else return null;
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

        #endregion HelperMethods
    }
}