using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MaterialType
    {
        None,
        Stone,
        Meat,
        Bone,
        Teeth,
        GroundPowder,
        Metal,
        Glass,
        Wood,
        Plant,
        Scale,
        Skin,
        Fat,
        Fuel,
        Liquid,
    }
}
