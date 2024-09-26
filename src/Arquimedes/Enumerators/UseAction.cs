using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UseAction
    {
        None,
        Sit,
        Study,
        Craft,
        Alchemy,
        Distill,
        Enchant,
        Rest,
        VisExtract,
        Hammer,
        Lockpick,
        Pry,
        Unlight,
        Store
    }
}