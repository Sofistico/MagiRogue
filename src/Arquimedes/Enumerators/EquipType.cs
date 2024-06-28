using Newtonsoft.Json;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum EquipType
    {
        None,
        Head,
        UpperBody,
        LowerBody,
        Arm,
        Leg,
        Foot,
        Hand,
        Finger,
        Tail,
        Wing,
        Neck,
        Held
    }
}