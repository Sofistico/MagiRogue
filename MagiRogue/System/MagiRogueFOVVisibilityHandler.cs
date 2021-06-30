using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using SadConsole;
using SadConsole.Effects;
using System.Linq;
using System;

namespace MagiRogue.System
{
    /// <summary>
    /// Handler that will make all terrain/entities inside FOV visible as normal, all entities outside of FOV invisible, all
    /// terrain outside of FOV invisible if unexplored, and set its foreground to <see cref="ExploredColorTint"/> if explored but out of FOV.
    /// </summary>
    public class MagiRogueFOVVisibilityHandler : FOVHandler
    {
        private readonly int _ghostLayer;

        /// <summary>
        /// Foreground color to set to all terrain that is outside of FOV but has been explored.
        /// </summary>
        public Color ExploredColorTint { get; }

        /// <summary>
        /// Creates a DefaultFOVVisibilityHandler that will manage visibility of objects for the given map as noted in the class description.
        /// </summary>
        /// <param name="map">The map this handler will manage visibility for.</param>
        /// <param name="unexploredColor">Foreground color to set to all terrain tiles that are outside of FOV but have been explored.</param>
        /// <param name="startingState">The starting state to put the handler in.</param>
        public MagiRogueFOVVisibilityHandler(Map map, Color unexploredColor, int ghostLayer,
            FovState startingState = FovState.Enabled) :
            base(map, startingState)
        {
            ExploredColorTint = unexploredColor;
            _ghostLayer = ghostLayer;
        }

        /// <summary>
        /// Foreground color to set to all terrain that is outside of FOV but has been explored.
        /// </summary>
        public void RefreshExploredTerrain()
        {
            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    if (Map.PlayerExplored[x, y])
                    {
                        TileBase tile = Map.Terrain[x, y] as TileBase;
                        UpdateTerrainSeen(tile);
                        UpdateTerrainUnseen(tile);
                    }
                }
            }
        }

        /// <summary>
        /// Makes entity visible.
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        protected override void UpdateEntitySeen(Entity entity)
        {
            if (entity.Layer == _ghostLayer)
            {
                Map.Remove(entity);
                return;
            }

            entity.IsVisible = true;
        }

        /// <summary>
        /// Makes entity invisible and add a ghost like entity containing its appearence and last know position
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        protected override void UpdateEntityUnseen(Entity entity)
        {
            if (entity.Layer == _ghostLayer)
                return;

            if (entity.Layer != _ghostLayer && Map.PlayerExplored[entity.Position] && entity.LeavesGhost)
            {
                Entity ghost = new Entity((Color)ExploredColorTint,
                    entity.Appearance.Background,
                    entity.Appearance.Glyph,
                    entity.Position,
                    _ghostLayer)
                {
                    IsVisible = true,
                    Name = $"Ghost {entity.Name}"
                };

                //ghost.OnCalculateRenderPosition();

                Map.Add(ghost);
            }
            entity.IsVisible = false;
        }

        /// <summary>
        /// Makes terrain visible and sets its foreground color to its regular value.
        /// </summary>
        /// <param name="terrain">Terrain to modify.</param>
        protected override void UpdateTerrainSeen(TileBase terrain)
        {
            terrain.IsVisible = true;
            // If the appearances don't match currently, synchronize them
            if (!terrain.LastSeenAppereance.Matches(terrain))
            {
                terrain.CopyAppearanceFrom(terrain.LastSeenAppereance);
                terrain.LastSeenAppereance.IsVisible = false;
            }

            // Set up an event handler that will keep the actual appearance in sync with the
            // true one, so long as the tile remains visible to the player.  Because this could be
            // called twice in certain state changes, we make sure we do not add the handler twice.
            //
            // Removing the event even if it does not exist does not throw exception and appears to be the
            // most performant way to account for duplicates.
            terrain.IsDirtySet -= On_VisibleTileTrueAppearanceIsDirtySet;
            terrain.IsDirtySet += On_VisibleTileTrueAppearanceIsDirtySet;
        }

        /// <summary>
        /// Makes terrain invisible if it is not explored.  Makes terrain visible but sets its foreground to
        /// <see cref="ExploredColorTint"/> if it is explored.
        /// </summary>
        /// <param name="terrain">Terrain to modify.</param>
        protected override void UpdateTerrainUnseen(TileBase terrain)
        {
            // If the event handler for synchronizing the appearance with true appearance is added, remove it
            // which will cause the appearance to remain as last-seen if it changes
            terrain.IsDirtySet -= On_VisibleTileTrueAppearanceIsDirtySet;

            if (Map.PlayerExplored[terrain.Position])
            {
                ApplyMemoryAppearance(terrain);
            }
            else
            {
                // If the unseen tile isn't explored, it's invisible
                terrain.LastSeenAppereance.IsVisible = false;
                //terrain.IsVisible = false;
            }
            terrain.LastSeenAppereance.IsDirty = true;
            terrain.IsDirty = true;
        }

        private void ApplyMemoryAppearance(ColoredGlyph tile)
        {
            tile.Foreground = ExploredColorTint;
        }

#nullable enable

        private void On_VisibleTileTrueAppearanceIsDirtySet(object? sender, EventArgs e)
#nullable disable
        {
            // Sender will not be null because of event invariants.  Cast is safe since we
            // control what this handler is added to and it is checked first
            var awareTerrain = (TileBase)sender!;

            // If appearances are synchronized, there is nothing to do
            if (awareTerrain.LastSeenAppereance.Matches(awareTerrain))
                return;

            // Otherwise, synchronize them
            awareTerrain.LastSeenAppereance.CopyAppearanceFrom(awareTerrain);
            awareTerrain.LastSeenAppereance.IsVisible = awareTerrain.IsVisible;
        }
    }
}