using GoRogue.DiceNotation;
using MagiRogue.Entities;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MagiRogue.System.Magic
{
    /// <summary>
    /// The class that is the manager of the magic system to an entity
    /// </summary>
    public class MagicManager
    {
        // Create a magic inspired by Mother of learning
        public List<SpellBase> KnowSpells { get; set; }

        /// <summary>
        /// The amount of mana finess required to pull of a spell, something can only be casted if you can
        /// have enough control to properly control the mana, see <see cref="SpellBase.Proficiency"/>.
        /// <para> Should be at minimum a 5 to cast the simplest battle spell.</para>
        /// </summary>
        public int ShapingSkill { get; set; }

        /// <summary>
        /// The innate resistance to magic from a being, how hard it is to harm another with pure magic
        /// </summary>
        public int InnateMagicResistance { get; set; } = 0;

        /// <summary>
        /// The resistance to magic from other stuff add to a being
        /// </summary>
        public int MagicResistance { get; set; } = 0;

        /// <summary>
        /// The research skill, how long it takes to properly research something, there is no upper limit.
        /// </summary>
        public int ResearchSkill { get; set; }

        /// <summary>
        /// How likely it is to penetrate the resistance of another being,
        /// the formula should be to win against
        /// the resistance
        /// ((0.3 * Proficiency) + (ShapingSkill * 0.5) + MagicPenetration)
        /// + bonusLuck >= (InnateResistance + MagicResistance) * 2
        /// </summary>
        public int MagicPenetration { get; set; } = 1;

        /// <summary>
        /// The amount of research, enchanting and lab work you can do in a session.
        /// </summary>
        public int LabTotal { get; set; }

        /// <summary>
        /// Any enchantment that affects something
        /// </summary>
        public List<ISpellEffect> Enchantments { get; set; } = new List<ISpellEffect>();

        public MagicManager()
        {
            KnowSpells = new List<SpellBase>();
        }

        public static int CalculateSpellDamage(Stat entityStats, SpellBase spellCasted)
        {
            int baseDamage = (int)(spellCasted.Power + spellCasted.SpellLevel
                + entityStats.MindStat + (entityStats.SoulStat * 0.5));

            int rngDmg = Dice.Roll($"{spellCasted.SpellLevel}d{baseDamage}");

            int damageAfterModifiers = (int)(rngDmg * spellCasted.Proficiency);

            return damageAfterModifiers;
        }

        public static bool PenetrateResistance(SpellBase spellCasted, Entity caster, Entity defender,
            int bonusLuck) =>
            (int)((0.3 * spellCasted.Proficiency) + (caster.Magic.ShapingSkill * 0.5)
            + caster.Magic.MagicPenetration) + bonusLuck >= (defender.Magic.InnateMagicResistance
            + defender.Magic.MagicResistance) * 2;

        public SpellBase QuerySpell(string spellId)
        {
            return KnowSpells.FirstOrDefault(i => i.SpellId.Equals(spellId));
        }
    }

    /// <summary>
    /// Scholls of magic
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MagicSchool
    {
        ///<summary>projects mana into different forms, mainly made for combat magic</summary>
        Projection,
        ///<summary>The art of unmaking spells, of dispelling and countering magic</summary>
        Negation,
        ///<summary>The art of animating something with magic, making it do someting autonomosly</summary>
        Animation,
        ///<summary>The art of knowing things with magic</summary>
        Divination,
        ///<summary>The art of natural and unnatural trasformation of things with magic</summary>
        Alteration, // takes care of the old Transformation as well.
        ///<summary>The art of protecting something with magic, ex. Ward against fire or metal</summary>
        Wards,
        ///<summary>The art of alterating dimensions with magic, like pocket dimensions and bags of holding</summary>
        Dimensionalism,
        ///<summary>The art of conjuring things with magic</summary>
        Conjuration,
        ///<summary>The art of ilusion, of creating light, of affecting the senses</summary>
        Illuminism,
        ///<summary>The art of healing, is limited by the caster own medical knowlodge</summary>
        MedicalMagic,
        ///<summary>The art of mind reading, of implating thought and deleting memories</summary>
        MindMagic,
        ///<summary>The art of the soul, of splicing souls, creating undead and becoming more</summary>
        SoulMagic,
        ///<summary>The art of the body,
        ///of making permanent arcane alterations to the body and sacrificial power</summary>
        BloodMagic
    }
}