using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpellContext
    {
        None,
        Offensive,
        Defensive,
        Perception,
        Healing,
        Buff,
        Debuff,
        Teleport
    }
}
