using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorldDirection
    {
        Left,
        Right,
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
    }
}