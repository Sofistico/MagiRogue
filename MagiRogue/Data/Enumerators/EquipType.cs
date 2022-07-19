using Newtonsoft.Json;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum EquipType
    {
        None,
        Head,
        Torso,
        Arm,
        Leg,
        Foot,
        Hand,
        Finger,
        Tail,
        Wing,
        Neck
    }
}