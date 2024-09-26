using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Arquimedes.Enumerators
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AbilityCategory
    {
        None,

        // Combat skills
        ArmorUse,
        Unarmed,
        Bite,
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
        /// <summary>
        /// The amount of mana finess required to pull of a spell, something can only be casted if
        /// you can have enough control to properly control the mana, see <see cref="SpellBase.Proficiency"/>.
        /// <para>Should be at minimum a 3 to cast the simplest battle spell reliable.</para>
        /// </summary>
        MagicShaping,
        /// <summary>
        /// How likely it is to penetrate the resistance of another being, the formula should be to
        /// win against the resistance ((0.3 * Proficiency) + (ShapingSkill * 0.5) + MagicPenetration)
        /// + bonusLuck &gt;= (InnateResistance + MagicResistance) * 2
        /// </summary>
        MagicPenetration,
        /// <summary>
        /// The resistance to magic from other stuff add to a being
        /// </summary>
        MagicResistance,

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