using MagiRogue.Entities;
using MagiRogue.System.Magic;

namespace MagiRogue.System.Time
{
    public static class TimeHelper
    {
        public const int WalkTime = 100;
        public const int AttackTime = 150;
        public const int Wait = 100;
        public const int Interact = 50;
        public const int Wear = 200;
        public const int MagicalThings = 400;

        public static int GetWalkTime(Actor actor)
        {
            return (int)(WalkTime / actor.Stats.Speed);
        }

        public static int GetAttackTime(Actor actor)
        {
            return (int)(AttackTime / actor.Stats.Speed);
        }

        public static int GetCastingTime(Actor actor, SpellBase spellCasted)
        {
            return (int)
                ((MagicalThings + spellCasted.SpellLevel + spellCasted.ManaCost)
                / (actor.Stats.Speed + actor.Magic.ShapingSkills * 0.5));
        }
    }
}