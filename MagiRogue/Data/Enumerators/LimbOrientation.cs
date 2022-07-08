using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

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