using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AgeGroup
    {
        Baby,
        Child,
        Teen,
        Adult,
        Elderly
    }
}
