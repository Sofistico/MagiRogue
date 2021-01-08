using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities.Stats
{
    public enum StatModType
    {
        Flat,
        Percent
    }

    public class StatModifier
    {
        public readonly float Value;
        public readonly StatModType ModType;
        public readonly int Order;

        public StatModifier(float value, StatModType modType, int order)
        {
            Value = value;
            ModType = modType;
            Order = order;
        }

        // Add a new constructor that automatically sets a default Order, in case the user doesn't want to manually define it
        public StatModifier(float value, StatModType modType) : this(value, modType, (int)modType)
        {
            // just leave empty
        }
    }
}