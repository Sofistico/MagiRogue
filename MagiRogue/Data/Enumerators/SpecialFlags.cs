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

        // unit flag Tbd
        MagicUser,

        // race flags
        Regenerator,
        Mundane,
        Animal,
        CanBeFamiliar,
        CanLearn,
        CanSpeak,
        Sapient, // combines CanLearn and CanSpeak into one

        // race behavior flags
        Diurnal,
        Nocturnal,
        Crepuscular,
        AllActive,
        CommonDomestic, // the race is a common domestic animal
        CommonPet, // the animal is easily adopted by civ race
        Pet, // the animal can be adopted
    }
}