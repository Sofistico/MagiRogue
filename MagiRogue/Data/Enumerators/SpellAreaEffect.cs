using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpellAreaEffect
    {
        Self,
        Target,
        Ball,
        Beam,
        Cone,
        Level,
        World
    }
}