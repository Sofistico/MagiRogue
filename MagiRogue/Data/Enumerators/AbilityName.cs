﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.Data.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AbilityName
    {
        None,

        // Combat skills
        ArmorUse,
        Unarmed,
        Misc,
        Fist,
        Sword,
        Staff,
        Hammer,
        Spear,
        Axe,

        // Craft skills
        Mason,
        WoodCraft,
        Forge,
        Smelt,
        Weaver,
        Alchemy,
        Enchantment,

        // Various job skills
        Farm,
        Medicine,
        Surgeon,
        Miner,
        Brewer,
        Cook,

        // Social skills
        Intimidator,
        Liar,
        Negotiator,
        Persuader,

        //Misc
        Concentration,
        Write,
        Teach,
        Read,
        Leader,
        MilitaryTactics,
        Swin,

        // scholary
        MagicLore,
        Mathematician,
        Astronomer,
    }
}