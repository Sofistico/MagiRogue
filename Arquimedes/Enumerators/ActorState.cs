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
    [Flags]
    public enum ActorState
    {
        Normal,
        Uncontrolled,
        Prone,
        Sleeping,
    }
}
