using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoomTag
    {
        Generic,
        Debug,
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
        ResearchRoom,
        EnchantingRoom,
        Ritual
    }
}