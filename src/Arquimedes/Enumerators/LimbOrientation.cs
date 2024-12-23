﻿using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Arquimedes.Enumerators
{
    /// <summary>
    /// Marks if the limb is right, left, or center.
    /// </summary>
    [DataContract]
    [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
    public enum BodyPartOrientation
    { Right, Left, Center, Tbd }
}