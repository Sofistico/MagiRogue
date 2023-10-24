using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BiomeType
    {
        Sea = -3,
        Mountain = -2,
        Ice = -1,
        Null,
        BorealForest,
        Tundra,
        Desert,
        Savanna,
        TropicalRainforest,
        Grassland,
        Woodland,
        SeasonalForest,
        TemperateRainforest,
    }
}