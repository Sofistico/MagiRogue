using GoRogue.DiceNotation;
using MagiRogue.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.System.Magic
{
    public class Magic
    {
        // Create a magic inspired by Mother of learning
        public List<SpellBase> KnowSpells { get; set; }

        public int ShapingSkills { get; set; }

        public Magic()
        {
            KnowSpells = new List<SpellBase>();
        }

        public int CalculateSpellDamage(Stat entityStats, SpellBase spellCasted)
        {
            int baseDamage = (int)(spellCasted.SpellLevel + entityStats.MindStat + (entityStats.SoulStat * 0.5));

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
        Summoning,
        Illuminism,
        MedicalMagic,
        CombatMagic,
        MindMagic,
        SoulMagic,
        BloodMagic
    }
}