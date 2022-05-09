using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using MagiRogue.GameSys.Tiles;
using SadRogue.Primitives;

namespace MagiRogue.GameSys.Time
{
    public static class TimeHelper
    {
        //public const int WalkTime = 100;
        public const int AttackTime = 150;
        public const int Wait = 100;
        public const int Interact = 50;
        public const int Wear = 200;
        public const int MagicalThings = 250;
        //public const int Year = 31536000;

        public static int GetWalkTime(Actor actor, TileBase tileToMove)
        {
            return (int)(tileToMove.MoveTimeCost / actor.Stats.Speed);
        }

        public static int GetWalkTime(Actor actor, Point pos)
        {
            var tile = GameLoop.GetCurrentMap().GetTileAt<TileBase>(pos);
            return GetWalkTime(actor, tile);
        }

        public static int GetWorldWalkTime(Actor actor, WorldTile tile)
        {
            // TODO: Need to fix this time to represent how slow it is to move on the overmap based
            // on the size of a overmap tile, which is to be defined.
            return (int)((tile.MoveTimeCost * 100) / actor.Stats.Speed);
        }

        public static int GetAttackTime(Actor actor)
        {
            return (int)(AttackTime / actor.Stats.Speed);
        }

        public static int GetCastingTime(Actor actor, SpellBase spellCasted)
        {
            return (int)
                ((MagicalThings + spellCasted.SpellLevel + spellCasted.ManaCost)
                / (actor.Stats.Speed + actor.Magic.ShapingSkill * 0.5));
        }
    }
}