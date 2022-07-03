using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DamageType
    {
        None = 0,
        Blunt = 1,
        Sharp = 2,
        Point = 3,
        Force = 4,
        Fire = 5,
        Cold = 6,
        Poison = 7,
        Acid = 8,
        Shock = 9,
        Soul = 10,
        Mind = 11
    }
}