using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
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
