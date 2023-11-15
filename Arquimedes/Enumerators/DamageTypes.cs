using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DamageTypes
    {
        None = 0,
        Blunt = 1 << 0,
        Sharp = 1 << 1,
        Pierce = 1 << 2,
        Fire = 1 << 4,
        Cold = 1 << 5,
        Steam = Fire | Cold,
        Poison = 1 << 6,
        Frostbite = Sharp | Cold | Poison,
        Acid = 1 << 7,
        Shock = 1 << 8,
        Soul = 1 << 9,
        Mind = 1 << 10,
        Lightning = 1 << 11,
        Thunderstorm = Shock | Lightning,
    }
}
