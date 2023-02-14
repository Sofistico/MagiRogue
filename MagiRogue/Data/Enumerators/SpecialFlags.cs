﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpecialFlag
    {
        // hf flags
        Myth,
        DemiMyth,
        MythDescended,
        Supernatural,
        Magical,
        MagicCreator,
        OpenedPortal,
        GaveMagicToCreation,
        Antagonist,
        WorldCreator,
        MagicKiller,

        // unit flag Tbd
        MagicUser,
        BuiltTower,

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
        Wild, // this animal can be found in the wild
        Grazer, // eats grass from the ground
        Hunter, // animal hunts for meat
        PackAimal, // animal forms packs
        NoSleep,
        NoEat,
        NoDrink,

        //Civ stuff:
        Married,
    }
}