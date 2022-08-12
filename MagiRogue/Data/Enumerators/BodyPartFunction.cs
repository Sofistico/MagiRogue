using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BodyPartFunction
    {
        Root, // where the body begins, normally the upper body
        Limb,
        Grasp,
        Stance,
        Flier,
        Digit,
        Aperture,

        Vital,
        Thought,
        Breathe,
        Embedded,
    }
}