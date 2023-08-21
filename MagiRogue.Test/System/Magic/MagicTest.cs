using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MagiRogue.Test.System.Magic
{
    public class MagicTest
    {
        private MagicManager weakMagic;
        private MagicManager mediumMagic;
        private MagicManager strongMagic;

        [Fact]
        public void TestCasting()
        {
            List<SpellBase> spellBase = new List<SpellBase>(TestSpellsNoEffect());

            weakMagic = new MagicManager()
            {
                ShapingSkill = 5,
                KnowSpells = spellBase
            };

            mediumMagic = new MagicManager()
            {
                ShapingSkill = 10,
                KnowSpells = spellBase
            };

            strongMagic = new MagicManager()
            {
                ShapingSkill = 15,
                KnowSpells = spellBase
            };
            Soul soul = new Soul()
            {
                WillPower = 10,
                CurrentMana = 100
            };

            Actor weakSpellCaster = new Actor("Test1", Color.Black, Color.Black, 't', Point.None)
            {
                Magic = weakMagic,
                Soul = soul,
                //Stats = testStats
            };

            Actor mediumSpellCaster = new Actor("Test2", Color.Black, Color.Black, 't', Point.None)
            {
                Magic = mediumMagic,
                Soul = soul
                //Stats = testStats
            };

            Actor strongSpellCaster = new Actor("Test3", Color.Black, Color.Black, 't', Point.None)
            {
                Magic = strongMagic,
                Soul = soul
                //Stats = testStats
            };

            List<bool> canWeakCast = new();
            List<bool> canMediumCast = new();
            List<bool> canStrongCast = new();

            foreach (var item in weakMagic.KnowSpells)
            {
                canWeakCast.Add(item.CanCast(weakMagic, weakSpellCaster));
            }
            foreach (var item in mediumMagic.KnowSpells)
            {
                canMediumCast.Add(item.CanCast(mediumMagic, mediumSpellCaster));
            }
            foreach (var item in strongMagic.KnowSpells)
            {
                canStrongCast.Add(item.CanCast(strongMagic, strongSpellCaster));
            }

            Assert.True((canWeakCast.Count(a => a).Equals(1))
                && (canMediumCast.Count(a => a).Equals(2))
                && (canStrongCast.Count(a => a).Equals(4)));
        }

        private static List<SpellBase> TestSpellsNoEffect()
        {
            SpellBase missile = new SpellBase("magic_missile",
                 "Magic Missile",
                ArtMagic.Projection, 5, manaCost: 1.0f)
            { Proficiency = 1 };

            SpellBase cure = new SpellBase("cure_test", "Cure Test",
                ArtMagic.BloodMagic, 0, 1, 4)
            { Proficiency = 1 };

            SpellBase haste = new SpellBase("haste_self", "Haste",
                ArtMagic.Dimensionalism, 0, 1, 7)
            { Proficiency = 1 };

            SpellBase mageSight = new SpellBase("mage_sight", "Mage Sight",
                ArtMagic.Divination, 0, 1, 8)
            { Proficiency = 1 };

            return new List<SpellBase>()
            {
                missile,
                cure,
                haste,
                mageSight
            };
        }
    }
}