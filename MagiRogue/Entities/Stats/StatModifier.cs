using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities.Stats
{
    public enum StatModType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMulti = 300,
    }

    public class StatModifier
    {
        public readonly float Value;
        public readonly StatModType ModType;
        public readonly int Order;
        public readonly object Source;

        public StatModifier(float value, StatModType modType, int order, object source)
        {
            Value = value;
            ModType = modType;
            Order = order;
            Source = source;
        }

        // Requires Value and Type. Calls the "Main" constructor and sets Order and Source to their default values: (int)type and null, respectively.
        public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }

        // Requires Value, Type and Order. Sets Source to its default value: null
        public StatModifier(float value, StatModType type, int order) : this(value, type, order, null) { }

        // Requires Value, Type and Source. Sets Order to its default value: (int)Type
        public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
    }
}