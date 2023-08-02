using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SeasonType
    {
        Spring = 1,
        Summer = 2,
        Autumn = 3,
        Winter = 4
    }
}