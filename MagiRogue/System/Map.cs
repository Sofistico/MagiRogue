﻿using GoRogue;
using GoRogue.GameFramework;
using GoRogue.MapViews;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using MagiRogue.System.Time;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;
using System;
using System.Linq;

namespace MagiRogue.System
{
    /// <summary>
    /// Stores, manipulates and queries Tile data, uses GoRogue Map class
    /// </summary>
    public class Map : GoRogue.GameFramework.Map
    {
        #region Properties

        // One of these per layer, so we force the rendering order to be what we want (high layers
        // appearing on top of low layers). They're added to consoles in order of this array, first
        // to last, which controls the render order.
        private readonly MultipleConsoleEntityDrawingComponent[] entitySyncersByLayer;

        private TileBase[] _tiles; // Contains all tiles objects

        private Entity _gameObjectControlled;

        /// <summary>
        /// All cell tiles of the map, it's a TileBase array, should never be directly declared to create new tiles, rather
        /// it must use <see cref="Map.SetTerrain(IGameObject)"/>.
        /// </summary>
        public TileBase[] Tiles { get { return _tiles; } set { _tiles = value; } }

        /// <summary>
        /// Fires whenever FOV is recalculated.
        /// </summary>
        public event EventHandler FOVRecalculated;

        /// <summary>
        /// The current fov handler of the map
        /// </summary>
        public FOVHandler FOVHandler { get; }

        public TimeSystem Time { get; private set; }

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

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Build a new map with a specified width and height, has  entity layers,
        /// they being 0-Furniture, 1-ghosts, 2-Items, 3-Actors, 4-Player
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Map(int width, int height) : base(CreateTerrain(width, height), Enum.GetNames(typeof(MapLayer)).Length - 1,
            Distance.EUCLIDEAN,
            entityLayersSupportingMultipleItems: LayerMasker.DEFAULT.Mask((int)MapLayer.ITEMS, (int)MapLayer.GHOSTS, (int)MapLayer.PLAYER))
        {
            Tiles = ((ArrayMap<TileBase>)((LambdaSettableTranslationMap<TileBase, IGameObject>)Terrain).BaseMap);
            FOVHandler = new MagiRogueFOVVisibilityHandler(this, ColorAnsi.BlackBright, (int)MapLayer.GHOSTS);

            entitySyncersByLayer = new MultipleConsoleEntityDrawingComponent[Enum.GetNames(typeof(MapLayer)).Length - 1];
            for (int i = 0; i < entitySyncersByLayer.Length; i++)
                entitySyncersByLayer[i] = new MultipleConsoleEntityDrawingComponent();

            Time = new TimeSystem();
        }

        #endregion Constructor

        #region HelperMethods

        private static ISettableMapView<IGameObject> CreateTerrain(int width, int heigth)
        {
            var goRogueTerrain = new ArrayMap<TileBase>(width, heigth);
            return new LambdaSettableTranslationMap<TileBase, IGameObject>(goRogueTerrain, t => t, g => g as TileBase);
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
            if (location.X < 0 || location.Y < 0 || location.X >= Width || location.Y >= Height)
                return false;
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
            return Entities.GetItems(location).OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Removes an Entity from the MultiSpatialMap
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(Entity entity)
        {
            if (!RemoveEntity(entity))
            {
                throw new Exception($"Failed to remove {entity.Name} || {entity.Position} || {entity.GetType()}");
            }

            // Set this up to sycer properly
            entitySyncersByLayer[entity.Layer - 1].Entities.Remove(entity);

            // Link up the entity's Moved event to a new handler
            entity.Moved -= OnEntityMoved;
        }

        /// <summary>
        /// Adds an Entity to the MultiSpatialMap
        /// </summary>
        /// <param name="entity"></param>
        public void Add(Entity entity)
        {
            // Initilizes the field of view of the player, will do different for monsters
            if (entity is Player player)
            {
                CalculateFOV(position: player.Position, player.Stats.ViewRadius, radiusShape: Radius.CIRCLE);
                ControlledEntitiy = player;
            }

            // Set this up to sycer properly
            entitySyncersByLayer[entity.Layer - 1].Entities.Add(entity);

            if (!AddEntity(entity))
            {
                throw new Exception($"Failed to add {entity.Name} || {entity.Position} || {entity.GetType()}");
            }

            if (entity is Actor monster)
            {
                EntityTimeNode entityNode = new EntityTimeNode(monster.ID, Time.TimePassed.Ticks + 100);
                Time.RegisterEntity(entityNode);
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
            if (args.Entity is Player actor)
            {
                CalculateFOV(position: actor.Position, actor.Stats.ViewRadius, radiusShape: Radius.CIRCLE);
            }
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
#pragma warning disable IDE0038 // Usar a correspondência de padrão
                return Tiles[locationIndex] is T ? (T)Tiles[locationIndex] : null;
#pragma warning restore IDE0038 // Usar a correspondência de padrão
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
            // Syncs the entity and addeds to the renderer
            foreach (MultipleConsoleEntityDrawingComponent sync in entitySyncersByLayer)
            {
                renderer.Components.Add(sync);
                // Necessary so that the MultipleConsoleEntityDrawingComponent doesn't try to forcefully update
                // visibility.
                sync.HandleIsVisible = false;
            }

            renderer.IsDirty = true;
        }

        #endregion HelperMethods

        #region Overload

        /// <inheritdoc />
        public override void CalculateFOV(int x, int y, double radius, Distance radiusShape)
        {
            base.CalculateFOV(x, y, radius, radiusShape);

            FOVRecalculated?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public override void CalculateFOV(int x, int y, double radius, Distance radiusShape, double angle, double span)
        {
            base.CalculateFOV(x, y, radius, radiusShape, angle, span);

            FOVRecalculated?.Invoke(this, EventArgs.Empty);
        }

        #endregion Overload
    }

    // enum for defining maplayer for things, so that a monster and a player can occupy the same tile as an item for example.
    // If it stops working, add back the player map layer
    public enum MapLayer
    {
        TERRAIN,
        GHOSTS,
        ITEMS,
        ACTORS,
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