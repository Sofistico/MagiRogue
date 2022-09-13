using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Responsability
    {
        Null,
        LeadArmy,
        AdviseRuler,
        CourtMage,
        Rule,
        Administrate
    }
}