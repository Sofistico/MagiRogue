using Newtonsoft.Json;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum RoadStatus
    {
        Normal,
        Abandoned
    }
}