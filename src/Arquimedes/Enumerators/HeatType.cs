using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HeatType
    {
        Coldest,
        Colder,
        Cold,
        Warm,
        Warmer,
        Warmest
    }
}