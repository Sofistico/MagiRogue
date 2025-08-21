using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum KeymapAction
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        MoveUpLeft,
        MoveUpRight,
        MoveDownLeft,
        MoveDownRight
    }
}
