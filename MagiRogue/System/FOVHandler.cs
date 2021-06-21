using GoRogue.GameFramework;
using SadRogue.Primitives.GridViews;
using SadRogue.Primitives;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using System;
using System.Linq;
using GoRogue.SpatialMaps;

namespace MagiRogue.System
{
    /// <summary>
    /// The FOVHandler that handles what sees what and what to do with it, currently needs to be rewritten.
    /// </summary>
    public abstract class FOVHandler
    {
        /// <summary>
        /// Possible states for the <see cref="FOVHandler"/> FOVVisibilityHandler to be in.
        /// </summary>
        public enum FovState
        {
            /// <summary>
            /// Enabled state -- FOVVisibilityHandler will actively set things as seen/unseen when appropriate.
            /// </summary>
            Enabled,

            /// <summary>
            /// Disabled state.  All items in the map will be set as seen, and the FOVVisibilityHandler
            /// will not set visibility of any items as FOV changes or as items are added/removed.
            /// </summary>
            DisabledResetVisibility,

            /// <summary>
            /// Disabled state.  No changes to the current visibility of terrain/entities will be made, and the FOVVisibilityHandler
            /// will not set visibility of any items as FOV changes or as items are added/removed.
            /// </summary>
            DisabledNoResetVisibility
        }

        /// <summary>
        /// Whether or not the FOVVisibilityHandler is actively setting things to seen/unseen as appropriate.
        /// </summary>
        public bool IsEnabled => CurrentState == FovState.Enabled;

        private FovState _currentState;

        /// <summary>
        /// The current state of the handler.  See <see cref="State"/> documentation for details
        /// on each possible value.
        /// </summary>
        /// <remarks>
        /// If the component has been added to a map, setting this value will set all values in
        /// the map according to the new state.
        ///
        /// When the component is added to a map, the visibility of all values in that map will
        /// be set according to this value.
        /// </remarks>
        public FovState CurrentState
        {
            get => _currentState;

            set
            {
                // Nothing to do if the old value is the same as the new
                if (value == _currentState) return;

                // Otherwise, set the state value, and apply it to the map if
                // there is one.
                _currentState = value;

                //ApplyStateToMap()
            }
        }

        /// <summary>
        /// The map that this handler manages visibility of objects for.
        /// </summary>
        public Map Map { get; }

        /// <summary>
        /// Creates a FOVVisibilityHandler that will manage visibility of objects for the given map.
        /// </summary>
        /// <param name="map">The map this handler will manage visibility for.</param>
        /// <param name="startingState">The starting state to put the handler in.</param>
        protected FOVHandler(Map map, FovState startingState)
        {
            Map = map;

            map.ObjectAdded += Map_ObjectAdded;
            map.ObjectMoved += Map_ObjectMoved;
            map.FOVRecalculated += Map_FOVRecalculated;

            //CurrentState = startingState;

            SetState(startingState);
        }

        /// <summary>
        /// Sets the state of the FOVVisibilityHandler, affecting its behavior appropriately.
        /// </summary>
        /// <param name="state">The new state for the FOVVisibilityHandler.  See <see cref="FovState"/> documentation for details.</param>
        private void SetState(FovState state)
        {
            CurrentState = state;

            switch (state)
            {
                case FovState.Enabled:
                    foreach (Point pos in Map.Positions())
                    {
                        TileBase terrain = Map.GetTerrainAt<TileBase>(pos);
                        if (terrain == null) return;
                        if (terrain != null && Map.PlayerFOV.BooleanFOV[pos])
                        {
                            UpdateTerrainSeen(terrain);
                        }
                        else if (terrain != null)
                            UpdateTerrainUnseen(terrain);
                    }
                    foreach (Entity entity in Map.Entities.Items.Cast<Entity>())
                    {
                        if (Map.PlayerFOV.BooleanFOV[entity.Position])
                            UpdateEntitySeen(entity);
                        else
                            UpdateEntityUnseen(entity);
                    }

                    break;

                case FovState.DisabledResetVisibility:
                    break;

                case FovState.DisabledNoResetVisibility:
                    foreach (Point pos in Map.Positions())
                    {
                        TileBase terrain = Map.GetTerrainAt<TileBase>(pos);
                        if (terrain == null) return;
                        if (terrain != null)
                            UpdateTerrainSeen(terrain);
                    }

                    foreach (Entity entity in Map.Entities.Items.Cast<Entity>())
                    {
                        UpdateEntitySeen(entity);
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the state to enabled.
        /// </summary>
        public void Enable() => SetState(FovState.Enabled);

        /// <summary>
        /// Sets the state to disabled.  If <paramref name="resetVisibilityToSeen"/> is true, all items will be set to seen before
        /// the FOVVisibilityHandler is disabled.
        /// </summary>
        /// <param name="resetVisibilityToSeen">Whether or not to set all items in the map to seen before disabling the FOVVisibilityHandler.</param>
        public void Disable(bool resetVisibilityToSeen = true)
            => SetState(resetVisibilityToSeen ? FovState.DisabledResetVisibility : FovState.DisabledNoResetVisibility);

        /// <summary>
        /// Implement to make appropriate changes to a terrain tile that is now inside FOV.
        /// </summary>
        /// <param name="terrain">Terrain tile to modify.</param>
        protected abstract void UpdateTerrainSeen(TileBase terrain);

        /// <summary>
        /// Implement to make appropriate changes to a terrain tile that is now outside FOV.
        /// </summary>
        /// <param name="terrain">Terrain tile to modify.</param>
        protected abstract void UpdateTerrainUnseen(TileBase terrain);

        /// <summary>
        /// Implement to make appropriate changes to an entity that is now inside FOV.
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        protected abstract void UpdateEntitySeen(Entity entity);

        /// <summary>
        /// Implement to make appropriate changes to an entity that is now outside FOV.
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        protected abstract void UpdateEntityUnseen(Entity entity);

        private void Map_ObjectMoved(object sender, ItemMovedEventArgs<IGameObject> e)
        {
            if (!IsEnabled) return;

            if (Map.PlayerFOV.BooleanFOV[e.NewPosition])
                UpdateEntitySeen((Entity)(e.Item));
            else
                UpdateEntityUnseen((Entity)(e.Item));
        }

        private void Map_ObjectAdded(object sender, ItemEventArgs<IGameObject> e)
        {
            if (!IsEnabled) return;

            if (e.Item.Layer == 0) // terrain
            {
                if (Map.PlayerFOV.BooleanFOV[e.Position])
                    UpdateTerrainSeen((TileBase)(e.Item));
                else
                    UpdateTerrainUnseen((TileBase)(e.Item));
            }
            else // Entities
            {
                if (Map.PlayerFOV.BooleanFOV[e.Position])
                    UpdateEntitySeen((Entity)e.Item);
                else
                    UpdateEntityUnseen((Entity)e.Item);
            }
        }

        private void Map_FOVRecalculated(object sender, EventArgs e)
        {
            if (!IsEnabled) return;

            foreach (Point position in Map.PlayerFOV.NewlySeen)
            {
                TileBase terrain = Map.GetTerrainAt<TileBase>(position);
                if (terrain != null)
                    UpdateTerrainSeen(terrain);

                foreach (Entity entity in Map.GetEntitiesAt<Entity>(position))
                    UpdateEntitySeen(entity);
            }

            foreach (Point position in Map.PlayerFOV.NewlyUnseen)
            {
                TileBase terrain = Map.GetTerrainAt<TileBase>(position);
                if (terrain != null)
                    UpdateTerrainUnseen(terrain);

                foreach (Entity entity in Map.GetEntitiesAt<Entity>(position))
                    UpdateEntityUnseen(entity);
            }
        }
    }
}