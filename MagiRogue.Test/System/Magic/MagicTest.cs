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
            List<SpellBase> spellBase = new(GetListSpells());
            const int weakShaping = 5;
            const int mediumShaping = 10;
            const int strongShaping = 20;
            weakMagic = new()
            {
                KnowSpells = spellBase
            };

            mediumMagic = new()
            {
                KnowSpells = spellBase
            };

            strongMagic = new()
            {
                KnowSpells = spellBase
            };
            Soul soul = new()
            {
                WillPower = 10,
                CurrentMana = 100
            };
            Ability weakShape = new(AbilityCategory.MagicShaping, weakShaping);
            Ability mediumShape = new(AbilityCategory.MagicShaping, mediumShaping);
            Ability strongShape = new(AbilityCategory.MagicShaping, strongShaping);
            Mind weakMind = new();
            weakMind.AddAbilityToDictionary(weakShape);
            Mind mediumMind = new();
            mediumMind.AddAbilityToDictionary(mediumShape);
            Mind strongMind = new();
            strongMind.AddAbilityToDictionary(strongShape);
            Actor weakSpellCaster = new("Test1", Color.Black, Color.Black, 't', Point.None)
            {
                Magic = weakMagic,
                Soul = soul,
                Mind = weakMind
            };

            Actor mediumSpellCaster = new("Test2", Color.Black, Color.Black, 't', Point.None)
            {
                Magic = mediumMagic,
                Soul = soul,
                Mind = mediumMind
            };

            Actor strongSpellCaster = new("Test3", Color.Black, Color.Black, 't', Point.None)
            {
                Magic = strongMagic,
                Soul = soul,
                Mind = strongMind
            };

            List<bool> canWeakCast = [];
            List<bool> canMediumCast = [];
            List<bool> canStrongCast = [];

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

        private static List<SpellBase> GetListSpells()
        {
            SpellBase missile = new("magic_missile",
                 "Magic Missile",
                ArtMagic.Projection, 5, MagicCost: 1.0f)
            { Proficiency = 1 };

            SpellBase cure = new("cure_test", "Cure Test",
                ArtMagic.BloodMagic, 0, 1, 4)
            { Proficiency = 1 };

            SpellBase haste = new("haste_self", "Haste",
                ArtMagic.Dimensionalism, 0, 1, 7)
            { Proficiency = 1 };

            SpellBase mageSight = new("mage_sight", "Mage Sight",
                ArtMagic.Divination, 0, 1, 8)
            { Proficiency = 1 };

            return
            [
                missile,
                cure,
                haste,
                mageSight
            ];
        }
    }
}
