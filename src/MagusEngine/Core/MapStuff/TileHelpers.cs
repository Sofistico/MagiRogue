using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Services.Factory;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils;

namespace MagusEngine.Core.MapStuff
{
    public static class TileHelpers
    {
        public static bool ChangeTileEffect(Point target, Actor actor, int modifier, TileType change)
        {
            if (actor is null || actor?.CurrentMagiMap is null)
                return false;

            var tile = actor.CurrentMagiMap?.GetTileAt<Tile>(target)!;
            var threashold = PhysicsSystem.GetMiningDificulty(tile.Material);
            if ((modifier * (125 + Mrn.Normal1D100Dice)) <= threashold)
                return false;

            tile.ChangeTileType(change);
            tile.GoRogueComponents.Clear();

            return true;
        }
    }
}
