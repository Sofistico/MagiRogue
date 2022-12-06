using Newtonsoft.Json;
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
        Sword,
        Staff,
        Hammer,
        Spear,
        Axe,
        Shield,
        Dodge,

        // Craft skills
        GlassMaking,
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
        Discipline,
        Write,
        Student,
        Teach,
        Read,
        Leader,
        MilitaryTactics,
        Swin,

        // Magic specific
        MagicTheory,
        Gestures,
        Incantation,
        ManaShaping,

        // scholary
        MagicLore,
        Research,
        Mathematics,
        Astronomer,
        Chemist,
        Physics,
        Enginner,
    }
}