using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;

namespace MagiRogue.Utils
{
    public static class CombatUtils
    {
        public static DamageType SetFlag(DamageType a, DamageType b)
        {
            return a | b;
        }

        public static DamageType UnsetFlag(DamageType a, DamageType b)
        {
            return a & (~b);
        }

        // Works with "None" as well
        public static bool HasFlag(DamageType a, DamageType b)
        {
            return (a & b) == b;
        }

        public static DamageType ToogleFlag(DamageType a, DamageType b)
        {
            return a ^ b;
        }

        public static void DealDamage(int dmg, Entity entity, DamageType dmgType)
        {
            if (entity is Actor)
            {
                Actor actor = (Actor)entity;

                actor.Stats.Health -= dmg;

                if (actor.Stats.Health < 0)
                {
                    Commands.CommandManager.ResolveDeath(actor);
                }
            }

            if (entity is Item)
            {
                Item item = (Item)entity;

                item.Condition -= dmg;
            }

            GameLoop.UIManager.MessageLog.Add($"The {entity.Name} took {dmg} {dmgType}");
        }
    }

    [Flags]
    public enum DamageType
    {
        None = 0,
        Physical = 1,
        Force = 2,
        Fire = 3,
        Cold = 4,
        Poison = 5,
        Acid = 6,
        Electricity = 7,
        Soul = 8,
        Mind = 9
    }
}