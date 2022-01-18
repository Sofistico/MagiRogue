using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.System.Planet
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