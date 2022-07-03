using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data.Enumerators
{
    /// <summary>
    /// Marks if the limb is right, left, or center.
    /// </summary>
    [DataContract]
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public enum LimbOrientation
    { Right, Left, Center }
}