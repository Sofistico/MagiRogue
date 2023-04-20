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
        Blunt = 1 << 1,
        Sharp = 1 << 2,
        Point = 1 << 3,
        Force = 1 << 4,
        Fire = 1 << 5,
        Cold = 1 << 6,
        Poison = 1 << 7,
        Acid = 1 << 8,
        Shock = 1 << 9,
        Soul = 1 << 10,
        Mind = 1 << 11,
        Lightning = 1 << 12,

        Steam = Fire | Cold,
        Frostbite = Sharp | Cold | Poison,
        Thunderstorm = Shock | Lightning | Force,
    }
}