using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomTag
    {
        Generic,
        Inn,
        Temple,
        Blacksmith,
        Clothier,
        Alchemist,
        Hovel,
        Abandoned,
        House,
        Throne,
        Kitchen,
        GenericWorkshop,
        Dinner,
        DungeonKeeper,
        Farm,
        ReserachRoom,
        EnchantingRoom
    }
}