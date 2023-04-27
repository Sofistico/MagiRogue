using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using MagiRogue.GameSys.Tiles;

namespace MagiRogue.GameSys.Time
{
    public static class TimeHelper
    {
        public const int Wait = 100;
        public const int Interact = 50;
        public const int Wear = 200;
        public const int MagicalThings = 250;
        public const int AiFailed = -1;

        public static int GetWalkTime(Actor actor, TileBase tileToMove)
        {
            return (int)(tileToMove.MoveTimeCost / actor.GetActorBaseSpeed());
        }

        public static int GetWalkTime(Actor actor, Point pos)
        {
            var tile = GameLoop.GetCurrentMap().GetTileAt<TileBase>(pos);
            return GetWalkTime(actor, tile);
        }

        public static int GetWalkTime(Actor actor)
        {
            var tile = ((Map)actor.CurrentMap).GetTileAt<TileBase>(actor.Position);
            return GetWalkTime(actor, tile);
        }

        public static int GetWorldWalkTime(Actor actor, WorldTile tile)
        {
            // TODO: Need to fix this time to represent how slow it is to move on the overmap based
            // on the size of a overmap tile, which is to be defined.
            return (int)((tile.MoveTimeCost * 100) / actor.GetActorBaseSpeed());
        }

        public static int GetAttackTime(Actor actor, Attack attack)
        {
            return (int)actor.GetAttackVelocity(attack) / attack.RecoverVelocity;
        }

        public static int GetCastingTime(Actor actor, SpellBase spellCasted)
        {
            return (int)
                ((MagicalThings * (spellCasted.SpellLevel + spellCasted.ManaCost))
                    / (actor.GetActorBaseCastingSpeed()));
        }
    }
}