using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MagiRogue.Test.System.Magic
{
    public class MagicTest
    {
        private MagiRogue.GameSys.Magic.MagicManager weakMagic;
        private MagiRogue.GameSys.Magic.MagicManager mediumMagic;
        private MagiRogue.GameSys.Magic.MagicManager strongMagic;
        private readonly Stat testStats;

        public MagicTest()
        {
            Game.Create(1, 1);

            testStats = new Stat();
            testStats.SetAttributes(
                viewRadius: 7,
                health: 10,
                baseHpRegen: 0.1f,
                bodyStat: 1,
                mindStat: 1,
                soulStat: 1,
                baseAttack: 10,
                attackChance: 40,
                protection: 5,
                defenseChance: 20,
                speed: 1.0f,
                _baseManaRegen: 0.1f,
                personalMana: 12
                );
        }

        [Fact]
        public void TestCasting()
        {
            List<SpellBase> spellBase = new List<SpellBase>(TestSpellsNoEffect());

            weakMagic = new MagiRogue.GameSys.Magic.MagicManager()
            {
                ShapingSkill = 5,
                KnowSpells = spellBase
            };

            mediumMagic = new MagiRogue.GameSys.Magic.MagicManager()
            {
                ShapingSkill = 15,
                KnowSpells = spellBase
            };

            strongMagic = new MagiRogue.GameSys.Magic.MagicManager()
            {
                ShapingSkill = 30,
                KnowSpells = spellBase
            };

            Actor weakSpellCaster = new Actor("Test1", Color.Black, Color.Black, 't', Point.None)
            {
                Magic = weakMagic,
                Stats = testStats
            };

            Actor mediumSpellCaster = new Actor("Test2", Color.Black, Color.Black, 't', Point.None)
            {
                Magic = mediumMagic,
                Stats = testStats
            };

            Actor strongSpellCaster = new Actor("Test3", Color.Black, Color.Black, 't', Point.None)
            {
                Magic = strongMagic,
                Stats = testStats
            };

            List<bool> canWeakCast = new();
            List<bool> canMediumCast = new();
            List<bool> canStrongCast = new();

            foreach (var item in weakMagic.KnowSpells)
            {
                canWeakCast.Add(item.CanCast(weakMagic, weakSpellCaster.Stats));
            }
            foreach (var item in mediumMagic.KnowSpells)
            {
                canMediumCast.Add(item.CanCast(mediumMagic, mediumSpellCaster.Stats));
            }
            foreach (var item in strongMagic.KnowSpells)
            {
                canStrongCast.Add(item.CanCast(strongMagic, strongSpellCaster.Stats));
            }

            Assert.True((canWeakCast.Where(a => a == true).Count().Equals(1))
                && (canMediumCast.Where(a => a == true).Count().Equals(2))
                && (canStrongCast.Where(a => a == true).Count().Equals(4)));
        }

        private static List<SpellBase> TestSpellsNoEffect()
        {
            SpellBase missile = new SpellBase("magic_missile",
                 "Magic Missile",
                MagicSchool.Projection, 5, manaCost: 1.0f)
            { Proficiency = 1 };

            SpellBase cure = new SpellBase("cure_test", "Cure Test",
                MagicSchool.MedicalMagic, 0, 1, 4)
            { Proficiency = 1 };

            SpellBase haste = new SpellBase("haste_self", "Haste",
                MagicSchool.Dimensionalism, 0, 1, 7)
            { Proficiency = 1 };

            SpellBase mageSight = new SpellBase("mage_sight", "Mage Sight",
                MagicSchool.Divination, 0, 1, 8)
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