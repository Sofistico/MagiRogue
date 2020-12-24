using GoRogue;
using System;
using GoRogue.MapViews;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using SadConsole;
using System.Linq;
using GoRogue.GameFramework;
using SadConsole.Components;

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

        /// <summary>
        /// All cell tiles of the map, it's a TileBase array, should never be directly declared to create new tiles, rather
        /// it must use <see cref="Map.SetTerrain(IGameObject)"/>.
        /// </summary>
        public TileBase[] Tiles { get { return _tiles; } set { _tiles = value; } }

        /// <summary>
        /// A static IDGenerator that all Entities can access
        /// </summary>
        public static IDGenerator IDGenerator = new IDGenerator();

        /*/// <summary>
        /// Keeps track of all the Entities on the map
        /// </summary>
        public new MultiSpatialMap<Entity> Entities;*/

        /// <summary>
        /// Fires whenever FOV is recalculated.
        /// </summary>
        public event EventHandler FOVRecalculated;

        /// <summary>
        /// The current fov handler of the map
        /// </summary>
        public FOVHandler FOVHandler { get; }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Build a new map with a specified width and height, has  entity layers,
        /// they being 1-Furniture, 2-Items, 3-Actors
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Map(int width, int height) : base(CreateTerrain(width, height), Enum.GetNames(typeof(MapLayer)).Length - 1,
            Distance.EUCLIDEAN,
            entityLayersSupportingMultipleItems: LayerMasker.DEFAULT.Mask((int)MapLayer.ITEMS))
        {
            //Tiles = new TileBase[width * height];
            Tiles = ((ArrayMap<TileBase>)((LambdaSettableTranslationMap<TileBase, IGameObject>)Terrain).BaseMap);
            //Entities = new MultiSpatialMap<Entity>();
            FOVHandler = new DefaultFOVVisibilityHandler(this, ColorAnsi.BlackBright);

            entitySyncersByLayer = new MultipleConsoleEntityDrawingComponent[Enum.GetNames(typeof(MapLayer)).Length - 1];
            for (int i = 0; i < entitySyncersByLayer.Length; i++)
                entitySyncersByLayer[i] = new MultipleConsoleEntityDrawingComponent();
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
            // Remove from spatial map
            //Entities.Remove(entity);

            RemoveEntity(entity);
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
            //Entities.Add(entity, entity.Position);

            // Initilizes the field of view of the player, will do different for monsters
            if (entity is Player actor)
            {
                /*actor.FieldOfViewSystem.Initialize(ViewMap);
                actor.FieldOfViewSystem.Calculate();*/
                CalculateFOV(position: actor.Position, actor.ViewRadius, radiusShape: Radius.CIRCLE);
            }

            AddEntity(entity);

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
                CalculateFOV(position: actor.Position, actor.ViewRadius, radiusShape: Radius.CIRCLE);
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
    public enum MapLayer
    {
        TERRAIN,
        ITEMS,
        ACTORS
    }
}