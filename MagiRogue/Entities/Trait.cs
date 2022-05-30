using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Trait
    {
        None,
        Confortable
    }
}