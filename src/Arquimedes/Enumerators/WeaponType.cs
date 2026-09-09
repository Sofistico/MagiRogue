using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WeaponType
    {
        Misc,
        Fist,
        Sword,
        Hammer,
        Spear,
        Axe,
    }
}
