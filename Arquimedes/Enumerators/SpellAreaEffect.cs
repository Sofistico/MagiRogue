using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpellAreaEffect
    {
        Self,
        Struck,
        Target,
        Ball,
        Beam,
        Cone,
        Level,
        World
    }
}