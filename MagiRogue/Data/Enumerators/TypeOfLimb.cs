using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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