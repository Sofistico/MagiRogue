using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.ECS.Components.ActorComponents;
using MagusEngine.ECS.Components.TilesComponents;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagusEngine.Systems
{
    /// <summary>
    /// Handler that will make all terrain/entities inside FOV visible as normal, all entities
    /// outside of FOV invisible, all terrain outside of FOV invisible if unexplored, and set its
    /// foreground to <see cref="ExploredColorTint"/> if explored but out of FOV.
    /// </summary>
    public sealed class MagiRogueFOVVisibilityHandler : FOVHandler
    {
        private readonly int _ghostLayer;
        private readonly Dictionary<uint, MagiEntity> ghosts = new();

        /// <summary>
        /// Foreground color to set to all terrain that is outside of FOV but has been explored.
        /// </summary>
        public Color ExploredColorTint { get; }

        /// <summary>
        /// Creates a DefaultFOVVisibilityHandler that will manage visibility of objects for the
        /// given map as noted in the class description.
        /// </summary>
        /// <param name="map">The map this handler will manage visibility for.</param>
        /// <param name="unexploredColor">
        /// Foreground color to set to all terrain tiles that are outside of FOV but have been explored.
        /// </param>
        /// <param name="startingState">The starting state to put the handler in.</param>
        public MagiRogueFOVVisibilityHandler(MagiMap map, Color unexploredColor, int ghostLayer,
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
            if (!IsEnabled) return;

            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    if (Map.PlayerExplored[x, y] && Map.Terrain[x, y] is Tile tile)
                    {
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
        protected override void UpdateEntitySeen(MagiEntity entity)
        {
            if (entity.Layer == _ghostLayer)
            {
                Map.RemoveMagiEntity(entity);

                return;
            }
            else if (ghosts.ContainsKey(entity.ID))
            {
                RemoveEntityGhost(entity);
            }
            entity.SadCell.IsVisible = true;
        }

        private void RemoveEntityGhost(MagiEntity entity)
        {
            var value = ghosts[entity.ID];
            ghosts.Remove(entity.ID);
            Map.RemoveMagiEntity(value);
        }

        /// <summary>
        /// Makes entity invisible and add a ghost like entity containing its appearence and last
        /// know position
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        protected override void UpdateEntityUnseen(MagiEntity entity)
        {
            if (entity.Layer == _ghostLayer || entity.AlwaySeen)
                return;

            if (entity.Layer != _ghostLayer
                && Map.PlayerExplored[entity.Position]
                && entity.LeavesGhost
                && !ghosts.ContainsKey(entity.ID))
            {
                MagiEntity ghost = new(ExploredColorTint,
                    entity.SadCell.AppearanceSingle!.Appearance.Background!,
                    entity.SadCell.AppearanceSingle.Appearance.Glyph,
                    entity.Position,
                    _ghostLayer)
                {
                    Name = $"Ghost {entity.Name}"
                };
                ghost.SadCell.IsVisible = true;
                if (ghosts.TryAdd(entity.ID, ghost))
                    Map.AddMagiEntity(ghost);
            }
            entity.SadCell.IsVisible = false;
        }

        /// <summary>
        /// Makes terrain visible and sets its foreground color to its regular value.
        /// </summary>
        /// <param name="terrain">Terrain to modify.</param>
        protected override void UpdateTerrainSeen(Tile terrain)
        {
            terrain.Appearence.IsVisible = true;

            if (terrain.GoRogueComponents.Contains<IllusionComponent>())
            {
                var illusion = terrain.GoRogueComponents.GetFirstOrDefault<IllusionComponent>();
                terrain.LastSeenAppereance?.CopyAppearanceFrom(illusion!.FakeAppearence);
            }

            // If the appearances don't match currently, synchronize them
            if (terrain.LastSeenAppereance?.Matches(terrain.Appearence) == false)
            {
                terrain.Appearence.CopyAppearanceFrom(terrain.LastSeenAppereance);
            }
            if (terrain.GetComponent<Plant>(out var plant))
            {
                terrain.Appearence.CopyAppearanceFrom(plant!.SadGlyph);
            }
        }

        /// <summary>
        /// Makes terrain invisible if it is not explored. Makes terrain visible but sets its
        /// foreground to <see cref="ExploredColorTint"/> if it is explored.
        /// </summary>
        /// <param name="terrain">Terrain to modify.</param>
        protected override void UpdateTerrainUnseen(Tile terrain)
        {
            if (Map.PlayerExplored[terrain.Position])
            {
                ApplyMemoryAppearance(terrain);
            }
            else
            {
                // If the unseen tile isn't explored, it's invisible
                terrain.Appearence.IsVisible = false;
            }
        }

        private void ApplyMemoryAppearance(Tile tile)
        {
            tile.Appearence.Foreground = ExploredColorTint;
        }
    }
}
