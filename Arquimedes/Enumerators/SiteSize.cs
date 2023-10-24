using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SiteSize
    {
        None = 0,
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