using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SelectContext
    {
        LimbType,
        OrganType,
        BodyPartFunction,
        Id,
        Category
    }
}
