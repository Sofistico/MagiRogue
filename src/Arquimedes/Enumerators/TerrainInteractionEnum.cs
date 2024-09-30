using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators;

[JsonConverter(typeof(StringEnumConverter))]
public enum TerrainInteractionEnum {
    None,
    CreateField,
    Destroys,
}
