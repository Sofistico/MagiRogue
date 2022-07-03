using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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