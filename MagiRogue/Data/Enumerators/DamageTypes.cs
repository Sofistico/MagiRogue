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
        Blunt = 1 << 0,
        Sharp = 1 << 1,
        Pierce = 1 << 2,
        Force = 1 << 3,
        Fire = 1 << 4,
        Cold = 1 << 5,
        Poison = 1 << 6,
        Acid = 1 << 7,
        Shock = 1 << 8,
        Soul = 1 << 9,
        Mind = 1 << 10,
        Lightning = 1 << 11,

        Steam = Fire | Cold,
        Frostbite = Sharp | Cold | Poison,
        Thunderstorm = Shock | Lightning | Force,
    }
}