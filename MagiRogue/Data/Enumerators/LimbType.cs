using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Enumerators
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LimbType
    {
        Head,
        Neck,
        UpperBody,
        LowerBody,
        Arm,
        Leg,
        Hand,
        Finger,
        Foot,
        Toe,
        Tail,
        Wing,
        Horn,
        Hoof,
    }
}