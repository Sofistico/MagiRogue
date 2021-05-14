using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

    [Flags]
    public enum DamageType
    {
        None = 0,
        Physical = 1 >> 1,
        Force = 1 >> 2,
        Fire = 1 >> 3,
        Cold = 1 >> 4,
        Poison = 1 >> 5,
        Acid = 1 >> 6,
        Electricity = 1 >> 7,
        Soul = 1 >> 8,
        Mind = 1 >> 9
    }
}