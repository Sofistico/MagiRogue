using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace MagiRogue.Data.Enumerators
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DamageTypes
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
        Mind = 11,
        Ligthing = 12
    }
}