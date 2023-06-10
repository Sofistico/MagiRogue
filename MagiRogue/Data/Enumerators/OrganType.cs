using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrganType
    {
        Heart,
        Brain,
        Digestive,
        Mouth,
        Teeth,
        Filtering,
        Breather,
        Protective,
        Visual,
        Auditory,
        Nerve
    }
}