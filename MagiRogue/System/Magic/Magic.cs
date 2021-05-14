using System.Collections.Generic;
using MagiRogue.Entities;

namespace MagiRogue.System.Magic
{
    public class Magic
    {
        // Create a magic inspired by Mother of learning
        public List<SpellBase> KnowSpells { get; set; }

        public int ShapingSkills { get; set; }

        public Magic()
        {
        }

        public void CastSpell(SpellBase spellCasted, Actor spellCaster)
        {
            if (spellCasted.CanCast(spellCaster.Magic, spellCaster.Stats) && KnowSpells.Contains(spellCasted))
            {
            }
            else
            {
                GameLoop.UIManager.MessageLog.Add($"Coundn't cast the spell {spellCasted.SpellName}");
            }
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