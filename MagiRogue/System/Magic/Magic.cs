using GoRogue.DiceNotation;
using MagiRogue.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.System.Magic
{
    /// <summary>
    /// The class that is the manager of the magic system to an entity
    /// </summary>
    public class Magic
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
        public int InnateResistance { get; set; } = 1;

        /// <summary>
        /// How likely it is to penetrate the resistance of another being, the formula should be to win against
        /// the resistance
        /// ((0.3 * Proficiency) + (ShapingSkill * 0.5) + MagicPenetration) + bonusLuck >= InnateResistance * 2
        /// </summary>
        public int MagicPenetration { get; set; } = 1;

        public Magic()
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

        public static bool PenetrateResistance(SpellBase spellCasted, Actor caster, Actor defender, int bonusLuck) =>
            ((0.3 * spellCasted.Proficiency) + (caster.Magic.ShapingSkill * 0.5)
            + caster.Magic.MagicPenetration) + bonusLuck >= defender.Magic.InnateResistance * 2;

        public SpellBase QuerySpell(string spellId)
        {
            return KnowSpells.Where(i => i.SpellId.Equals(spellId)).FirstOrDefault();
        }
    }

    public enum MagicSchool
    {
        Projection,
        Negation,
        Animation,
        Divination,
        Alteration,
        Wards,
        Dimensionalism,
        Conjuration,
        Transformation,
        Illuminism,
        MedicalMagic,
        MindMagic,
        SoulMagic,
        BloodMagic
    }
}