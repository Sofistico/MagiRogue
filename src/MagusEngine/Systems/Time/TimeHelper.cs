using System;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic;
using MagusEngine.Core.MapStuff;

namespace MagusEngine.Systems.Time
{
    public static class TimeHelper
    {
        public const int Wait = 100;
        public const int Interact = 50;
        public const int Wear = 200;
        public const int MagicalThings = 650;
        public const int AiFailed = -1;

        public static long GetWalkTime(Actor actor, Tile tileToMove)
        {
            return (long)(tileToMove.MoveTimeCost / actor.GetActorSpeed());
        }

        public static long GetWalkTime(Actor actor, Point pos)
        {
            var tile = Find.CurrentMap?.GetTileAt(pos);
            return GetWalkTime(actor, tile!);
        }

        public static long GetWalkTime(Actor actor)
        {
            var tile = ((MagiMap)actor.CurrentMap!).GetTileAt(actor.Position);
            return GetWalkTime(actor, tile!);
        }

        public static long GetWorldWalkTime(Actor actor, Tile tile)
        {
            // TODO: Need to fix this time to represent how slow it is to move on the overmap based
            // on the size of a overmap tile, which is to be defined.
            return (long)(tile.MoveTimeCost * 100 / actor.GetActorSpeed());
        }

        public static long GetAttackTime(Actor actor, Attack attack)
        {
            // Improved attack time calculation with realistic physics
            double baseTime = attack.PrepareVelocity + attack.RecoverVelocity;
            double actorSpeed = actor.GetActorSpeed();
            double skillBonus = Math.Max(0.5, 1.0 - (actor.GetRelevantAttackAbility(actor.WieldedItem()) * 0.01));

            return (long)(baseTime * skillBonus / Math.Max(actorSpeed, 0.1));
        }

        public static long GetAttackTimeWithWeapon(Actor actor, Attack attack, Item weapon)
        {
            double baseTime = attack.PrepareVelocity + attack.RecoverVelocity;
            double weaponWeight = weapon?.Mass ?? 1.0;
            double actorSpeed = actor.GetActorSpeed();
            double skillBonus = Math.Max(0.5, 1.0 - (actor.GetRelevantAttackAbility(weapon) * 0.01));
            double strengthFactor = Math.Max(0.5, 1.0 + (actor.GetStrenght() - weaponWeight) * 0.01);

            return (long)(baseTime * weaponWeight * skillBonus * strengthFactor / Math.Max(actorSpeed, 0.1));
        }

        public static long GetCastingTime(Actor actor, Spell spellCasted)
        {
            return (long)(MagicalThings * (spellCasted.SpellLevel + (spellCasted.MagicCost * 5)) / actor.GetActorBaseCastingSpeed(spellCasted.ShapingAbility));
        }

        public static long GetShootingTime(Player getPlayer, double mass)
        {
            return (long)(Wait + (mass * 10 / (getPlayer.GetActorSpeed())));
        }
    }
}
