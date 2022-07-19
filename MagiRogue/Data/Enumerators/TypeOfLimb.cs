using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Enumerators
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeOfLimb
    {
        Head,
        Torso,
        Arm,
        Leg,
        Foot,
        Hand,
        Tail,
        Wing,
        Neck,
        Finger,
        Toe,
        Misc
    }
}