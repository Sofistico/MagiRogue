using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TissueShape
    {
        Layer,
        Strand,
        Feather,
        Scale
    }
}
