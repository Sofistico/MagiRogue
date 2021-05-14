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
        Physical = 0,
        Force = 1 >> 1,
        Fire = 1 >> 2,
        Cold = 1 >> 3,
        Poison = 1 >> 4,
        Acid = 1 >> 5,
        Electricity = 1 >> 6
    }
}