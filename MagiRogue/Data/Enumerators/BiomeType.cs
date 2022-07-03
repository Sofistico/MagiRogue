using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BiomeType
    {
        Sea,
        Desert,
        Savanna,
        TropicalRainforest,
        Grassland,
        Woodland,
        SeasonalForest,
        TemperateRainforest,
        BorealForest,
        Tundra,
        Ice,
        Mountain,
        Null
    }
}