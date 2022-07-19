using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SettlementSize
    {
        Default = 0,
        /// <summary>
        /// Occupies one map of the chunk
        /// </summary>
        Small = 1,
        /// <summary>
        /// Occupies 2x2 maps of the chunk
        /// </summary>
        Medium = 2 * 2,
        /// <summary>
        /// Occupies the whole chunck, so about 3x3 maps.
        /// </summary>
        Large = 3 * 3,
    }
}