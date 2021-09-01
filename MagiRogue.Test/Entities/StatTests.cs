using MagiRogue.Entities;
using Xunit;

namespace MagiRogue.Test.Entities
{
    public class StatTests
    {
        [Fact()]
        public void SetAttributesTest()
        {
            Stat testStat = new Stat();

            testStat.SetAttributes(
                viewRadius: 1,
                health: 5,
                baseHpRegen: 1.5f,
                bodyStat: 2,
                mindStat: 2,
                soulStat: 2,
                baseAttack: 3,
                attackChance: 50,
                protection: 1,
                defenseChance: 50,
                speed: 1,
                _baseManaRegen: 1,
                personalMana: 1
                );

            bool testOk = testStat.ViewRadius == 1 & testStat.Health == 5 & testStat.MaxHealth == 5
                & testStat.BaseHpRegen == 1.5f & testStat.BodyStat == 2 & testStat.MindStat == 2
                & testStat.SoulStat == 2 & testStat.Strength == 5 & testStat.BaseAttack == 53 &
                testStat.Protection == 2 & testStat.Defense == 53 &
                testStat.Speed == 1 & testStat.BaseManaRegen == 1 & testStat.PersonalMana == 1;

            Assert.True(testOk);
        }

        [Fact()]
        public void ApplyHpRegenTest()
        {
            Stat test = new Stat
            {
                Health = 5,
                BaseHpRegen = 1,
                MaxHealth = 5
            };

            test.Health -= 5;

            test.ApplyHpRegen();

            Assert.True(test.Health == 1);
        }
    }
}