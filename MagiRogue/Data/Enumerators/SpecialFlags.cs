using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpecialFlag
    {
        // hf flags
        Deity,
        DeityDescended,
        MythAge,
        Supernatural,
        Magical,
        MagicCreator,
        OpenedPortal,
        GaveMagicToCreation,
        Antagonist,
        RaceCreator,
        WorldCreator,
        MagicKiller,

        // unit flag
        MageUser,
        Wizard,

        // race flags
        Regenerator,
        Mundane,
        Animal,
        CanBeFamiliar,
        CanLearn
    }
}