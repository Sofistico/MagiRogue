using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InjurySeverity
    {
        Scratch,
        LigthInjury,
        MediumInjury,
        SeriousInjury,
        Crippling,
        Fatal,
        LimbLoss
    }
}