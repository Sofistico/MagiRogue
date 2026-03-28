using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum KeymapAction
    {
        MoveNorth,
        MoveSouth,
        MoveLeft,
        MoveRight,
        MoveNorthLeft,
        MoveNorthRight,
        MoveSouthLeft,
        MoveSouthRight,
        MoveUp,
        MoveDown,
        OpenInventory,
        ThownItem,
        EscapeMenu,
        WaitTillRested,
        WaitOneSecond,
        WaitOneMoment,
        PickUp,
        DropItem,
        CloseDoor,
        OpenDoor,
        Look,
        SpellCasting,
        WaitScreen,
        ConfirmAction,
        CancelAction,
        Interact
    }
}
