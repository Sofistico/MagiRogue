using System.Collections.Generic;
using MagiRogue.Entities;
using MagiRogue.Commands;
using GoRogue;

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