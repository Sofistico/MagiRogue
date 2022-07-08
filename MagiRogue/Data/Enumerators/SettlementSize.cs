using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SettlementSize
    {
        Default,
        /// <summary>
        /// Occupies one map of the chunk
        /// </summary>
        Small,
        /// <summary>
        /// Occupies 2x2 maps of the chunk
        /// </summary>
        Medium,
        /// <summary>
        /// Occupies the whole chunck
        /// </summary>
        Large,
    }
}