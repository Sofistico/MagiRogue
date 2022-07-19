using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
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