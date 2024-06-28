using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Responsability
    {
        Null,
        LeadArmy,
        AdviseRuler,
        CourtMage,
        MagicStuff,
        Rule,
        Administrate
    }
}