using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BodyPartFunction
    {
        None,
        Root, // where the body begins, normally the upper body
        Limb,
        Grasp,
        Stance,
        Flier,
        Digit,
        Aperture,
        Horn,

        Vital,
        Thought,
        Breath,
        Tongue,
        Mouth,
        Visual,
        Protection, // bone, skulls, ribcage and etc
        Teeth,
    }
}