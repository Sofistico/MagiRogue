using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TileType
    {
        Null,
        Floor,
        Wall,
        Liquid,
        Node,
        WorldTile,
        Door,
        TreeTrunk
    }
}