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
        /// have enough control to properly control the mana, see <see cref="SpellBase.Proficency"/>.
        /// <para> Should be at minimum a 5 to cast the simplest battle spell.</para>
        /// </summary>
        public int ShapingSkills { get; set; }

        /// <summary>
        /// The innate resistance to magic from a being
        /// </summary>
        public int InnateResistance { get; set; }

        public Magic()
        {
            KnowSpells = new List<SpellBase>();
        }

        public static int CalculateSpellDamage(Stat entityStats, SpellBase spellCasted)
        {
            int baseDamage = (int)(spellCasted.Power + spellCasted.SpellLevel
                + entityStats.MindStat + (entityStats.SoulStat * 0.5));

            int rngDmg = Dice.Roll($"{spellCasted.SpellLevel}d{baseDamage}");

            int damageAfterModifiers = (int)(rngDmg * spellCasted.Proficency);

            return damageAfterModifiers;
        }

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