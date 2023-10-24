using Newtonsoft.Json;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum RoadStatus
    {
        Normal,
        Abandoned
    }
}