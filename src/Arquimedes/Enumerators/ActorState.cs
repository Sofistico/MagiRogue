using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
