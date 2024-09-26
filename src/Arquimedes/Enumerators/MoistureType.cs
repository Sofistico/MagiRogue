using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MoistureType
    {
        Wettest,
        Wetter,
        Wet,
        Dry,
        Dryer,
        Dryest
    }
}