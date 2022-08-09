using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InjurySeverity
    {
        Bruise,
        Minor,
        Inhibited,
        Broken,
        Pulped, // for blunt attacks, equivalent to missing the limb and for organs
        Missing,
    }
}