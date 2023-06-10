using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
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
