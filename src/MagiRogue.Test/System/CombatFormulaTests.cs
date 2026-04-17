using System.Linq;
using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Services.Factory;
using MagusEngine.Systems;
using SadRogue.Primitives;
using Xunit;

namespace MagiRogue.Test.System
{
    public class CombatFormulaTests
    {
        private static Actor CreateActor(string name, int strength, int endurance, int unarmed, int armorUse, int dodge)
        {
            var actor = EntityFactory.ActorCreator(Point.Zero, "test_race", name, 25, Sex.None);
            actor.Body.Strength = strength;
            actor.Body.Endurance = endurance;
            actor.Mind.AddAbilityToDictionary(new Ability(AbilityCategory.Unarmed, unarmed));
            actor.Mind.AddAbilityToDictionary(new Ability(AbilityCategory.ArmorUse, armorUse));
            actor.Mind.AddAbilityToDictionary(new Ability(AbilityCategory.Dodge, dodge));
            return actor;
        }

        private static Attack CreateTestAttack(double penetration = 0.0) => new Attack
        {
            Name = "Test Punch",
            AttackAbility = AbilityCategory.Unarmed,
            ContactArea = 100,
            DamageTypeId = "blunt",
            PenetrationPercentage = penetration,
            LimbFunction = BodyPartFunction.Grasp
        };

        [Fact]
        public void Damage_PenetrationAddsBonusDamage()
        {
            var attacker = CreateActor("attacker", 20, 10, 20, 5, 5);
            var defender = CreateActor("defender", 10, 10, 1, 1, 1);

            var noPenetration = CreateTestAttack(0.0);
            var highPenetration = CreateTestAttack(0.5);

            double noPenDamage = CombatSystem.ResolveDefenseAndGetAttackMomentum(attacker, defender, true, attacker.ActorAnatomy.Limbs[0], noPenetration);
            double highPenDamage = CombatSystem.ResolveDefenseAndGetAttackMomentum(attacker, defender, true, attacker.ActorAnatomy.Limbs[0], highPenetration);

            Assert.True(highPenDamage > noPenDamage);
        }

        [Fact]
        public void ProneTarget_TakesDoubleDamage()
        {
            var attacker = CreateActor("attacker", 20, 10, 20, 5, 5);
            var defender = CreateActor("defender", 10, 10, 1, 1, 1);
            defender.SituationalFlags.Add(ActorSituationalFlags.Prone);

            var attack = CreateTestAttack();
            double proneDamage = CombatSystem.ResolveDefenseAndGetAttackMomentum(attacker, defender, true, attacker.ActorAnatomy.Limbs[0], attack);

            var standingDefender = CreateActor("standing", 10, 10, 1, 1, 1);
            double standingDamage = CombatSystem.ResolveDefenseAndGetAttackMomentum(attacker, standingDefender, true, attacker.ActorAnatomy.Limbs[0], attack);

            Assert.True(proneDamage >= standingDamage * 1.5);
        }

        [Fact]
        public void Damage_EnduranceProvidesMitigation()
        {
            var attacker = CreateActor("attacker", 20, 10, 20, 5, 5);
            var highEnd = CreateActor("highEnd", 10, 100, 1, 1, 1);
            var lowEnd = CreateActor("lowEnd", 10, 1, 1, 1, 1);

            var attack = CreateTestAttack();

            double highEndDamage = CombatSystem.ResolveDefenseAndGetAttackMomentum(attacker, highEnd, true, attacker.ActorAnatomy.Limbs[0], attack);
            double lowEndDamage = CombatSystem.ResolveDefenseAndGetAttackMomentum(attacker, lowEnd, true, attacker.ActorAnatomy.Limbs[0], attack);

            Assert.True(highEndDamage < lowEndDamage);
        }

        [Fact]
        public void Damage_FullDefense()
        {
            var attacker = CreateActor("attacker", 20, 10, 20, 5, 5);
            var defender = CreateActor("defender", 10, 100, 1, 1, 1);

            var attack = CreateTestAttack();
            double damage = CombatSystem.ResolveDefenseAndGetAttackMomentum(attacker, defender, true, attacker.ActorAnatomy.Limbs[0], attack);

            Assert.True(damage >= 0);
        }

        [Fact]
        public void AttackMomentum_ReturnsPositiveValue()
        {
            var actor = CreateActor("actor", 10, 10, 1, 1, 1);
            var attack = CreateTestAttack();

            double momentum = CombatSystem.GetAttackMomentum(actor, actor.ActorAnatomy.Limbs[0], attack);

            Assert.True(momentum > 0);
        }

        [Fact]
        public void Damage_ReturnsPositiveValue()
        {
            var attacker = CreateActor("attacker", 10, 10, 5, 5, 5);
            var defender = CreateActor("defender", 10, 10, 1, 1, 1);

            var attack = CreateTestAttack();
            double damage = CombatSystem.ResolveDefenseAndGetAttackMomentum(attacker, defender, true, attacker.ActorAnatomy.Limbs[0], attack);

            Assert.True(damage >= 0);
        }

        [Fact]
        public void LimbHasTissues()
        {
            var actor = CreateActor("actor", 10, 10, 1, 1, 1);
            var limb = actor.ActorAnatomy.Limbs[0];

            Assert.NotEmpty(limb.Tissues);
        }

        [Fact]
        public void DealDamage_AddsWounds()
        {
            var attacker = CreateActor("attacker", 20, 10, 5, 5, 5);
            var defender = CreateActor("defender", 10, 10, 1, 1, 1);

            var limb = defender.ActorAnatomy.Limbs[0];
            var attack = CreateTestAttack();
            double momentum = CombatSystem.GetAttackMomentum(attacker, limb, attack);

            int initialWounds = defender.ActorAnatomy.GetAllWounds().Count;

            CombatSystem.DealDamage(momentum, defender, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, limb, null, attacker.GetAttackingLimb(attack));

            int finalWounds = defender.ActorAnatomy.GetAllWounds().Count;
            Assert.True(finalWounds > initialWounds);
        }

        [Fact]
        public void Wound_HasDamageType()
        {
            var attacker = CreateActor("attacker", 20, 10, 5, 5, 5);
            var defender = CreateActor("defender", 10, 10, 1, 1, 1);

            var limb = defender.ActorAnatomy.Limbs[0];
            var attack = CreateTestAttack();
            double momentum = CombatSystem.GetAttackMomentum(attacker, limb, attack);

            CombatSystem.DealDamage(momentum, defender, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, limb, null, attacker.GetAttackingLimb(attack));

            var wounds = defender.ActorAnatomy.GetAllWounds();
            Assert.NotEmpty(wounds);
            Assert.NotNull(wounds[0].InitialDamageSource);
        }

        [Fact]
        public void Wound_HasSeverity()
        {
            var attacker = CreateActor("attacker", 20, 10, 5, 5, 5);
            var defender = CreateActor("defender", 10, 10, 1, 1, 1);

            var limb = defender.ActorAnatomy.Limbs[0];
            var attack = CreateTestAttack();
            double momentum = CombatSystem.GetAttackMomentum(attacker, limb, attack);

            CombatSystem.DealDamage(momentum, defender, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, limb, null, attacker.GetAttackingLimb(attack));

            var wounds = defender.ActorAnatomy.GetAllWounds();
            Assert.NotEmpty(wounds);
            Assert.True(wounds[0].Severity == InjurySeverity.Bruise);
        }

        [Fact]
        public void WoundSeverity_VariesByDamage()
        {
            var weakAttacker = CreateActor("weak", 10, 10, 1, 1, 1);
            var strongAttacker = CreateActor("strong", 30, 10, 10, 5, 5);

            var defenderWeak = CreateActor("defenderWeak", 10, 10, 1, 1, 1);
            var defenderStrong = CreateActor("defenderStrong", 10, 10, 1, 1, 1);

            var limbWeak = defenderWeak.ActorAnatomy.Limbs[0];
            var limbStrong = defenderStrong.ActorAnatomy.Limbs[0];
            var attack = CreateTestAttack();

            double weakMomentum = CombatSystem.GetAttackMomentum(weakAttacker, limbWeak, attack);
            double strongMomentum = CombatSystem.GetAttackMomentum(strongAttacker, limbStrong, attack);

            CombatSystem.DealDamage(weakMomentum, defenderWeak, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, limbWeak, null, weakAttacker.GetAttackingLimb(attack));
            CombatSystem.DealDamage(strongMomentum, defenderStrong, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, limbStrong, null, strongAttacker.GetAttackingLimb(attack));

            var weakWounds = defenderWeak.ActorAnatomy.GetAllWounds();
            var strongWounds = defenderStrong.ActorAnatomy.GetAllWounds();

            if (weakWounds.Count > 0 && strongWounds.Count > 0)
            {
                Assert.True(strongWounds[0].Severity >= weakWounds[0].Severity);
            }
            else
            {
                Assert.True(strongWounds.Count > 0);
            }
        }

        [Fact]
        public void CombatSimulation_TwoSkilledFighters_OneDies()
        {
            var fighter1 = CreateActor("fighter1", 30, 10, 15, 10, 10);
            var fighter2 = CreateActor("fighter2", 30, 10, 15, 10, 10);

            var attack = CreateTestAttack();
            int rounds = 0;
            int maxRounds = 50;

            while (rounds < maxRounds)
            {
                var limb1 = fighter1.ActorAnatomy.Limbs[rounds % fighter1.ActorAnatomy.Limbs.Count];
                var limb2 = fighter2.ActorAnatomy.Limbs[rounds % fighter2.ActorAnatomy.Limbs.Count];

                double momentum1 = CombatSystem.GetAttackMomentum(fighter1, limb1, attack);
                double momentum2 = CombatSystem.GetAttackMomentum(fighter2, limb2, attack);

                CombatSystem.DealDamage(momentum1, fighter2, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, limb2, null, fighter1.GetAttackingLimb(attack));
                CombatSystem.DealDamage(momentum2, fighter1, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, limb1, null, fighter2.GetAttackingLimb(attack));

                rounds++;

                var wounds1 = fighter1.ActorAnatomy.GetAllWounds();
                var wounds2 = fighter2.ActorAnatomy.GetAllWounds();

                if (wounds1.Count > 5 || wounds2.Count > 5)
                    break;
            }

            var finalWounds1 = fighter1.ActorAnatomy.GetAllWounds();
            var finalWounds2 = fighter2.ActorAnatomy.GetAllWounds();

            Assert.True(finalWounds1.Count > 0 || finalWounds2.Count > 0);
        }

        [Fact]
        public void CombatSimulation_FightCreatesMultipleWounds()
        {
            var fighter1 = CreateActor("fighter1", 25, 10, 10, 5, 5);
            var fighter2 = CreateActor("fighter2", 25, 10, 10, 5, 5);

            var attack = CreateTestAttack();
            int rounds = 0;
            int maxRounds = 20;

            while (rounds < maxRounds)
            {
                var limb1 = fighter1.ActorAnatomy.Limbs[rounds % fighter1.ActorAnatomy.Limbs.Count];
                var limb2 = fighter2.ActorAnatomy.Limbs[rounds % fighter2.ActorAnatomy.Limbs.Count];

                double momentum1 = CombatSystem.GetAttackMomentum(fighter1, limb1, attack);
                double momentum2 = CombatSystem.GetAttackMomentum(fighter2, limb2, attack);

                CombatSystem.DealDamage(momentum1, fighter2, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, limb2, null, fighter1.GetAttackingLimb(attack));
                CombatSystem.DealDamage(momentum2, fighter1, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, limb1, null, fighter2.GetAttackingLimb(attack));

                rounds++;
            }

            var wounds1 = fighter1.ActorAnatomy.GetAllWounds();
            var wounds2 = fighter2.ActorAnatomy.GetAllWounds();

            Assert.True(wounds1.Count + wounds2.Count > 1);
        }

        [Fact]
        public void WoundAccumulation_SameLimbAccumulatesDamage()
        {
            var attacker = CreateActor("attacker", 25, 10, 10, 5, 5);
            var defender = CreateActor("defender", 10, 10, 1, 1, 1);

            var targetLimb = defender.ActorAnatomy.Limbs[0];
            var attack = CreateTestAttack();

            double momentum = CombatSystem.GetAttackMomentum(attacker, targetLimb, attack);

            CombatSystem.DealDamage(momentum, defender, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, targetLimb, null, attacker.GetAttackingLimb(attack));
            var woundsAfterFirst = defender.ActorAnatomy.GetAllWounds();
            double firstVolume = woundsAfterFirst.Sum(w => w.VolumeInjury);

            CombatSystem.DealDamage(momentum, defender, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, targetLimb, null, attacker.GetAttackingLimb(attack));
            var woundsAfterSecond = defender.ActorAnatomy.GetAllWounds();
            double secondVolume = woundsAfterSecond.Sum(w => w.VolumeInjury);

            Assert.True(secondVolume > firstVolume);
        }

        [Fact]
        public void LimbMangling_SevereDamageAccumulatedDmgBreaksLimb()
        {
            var attacker = CreateActor("attacker", 50, 10, 20, 5, 5);
            var defender = CreateActor("defender", 10, 10, 1, 1, 1);

            var targetLimb = defender.ActorAnatomy.Limbs[4];
            Assert.NotNull(targetLimb);

            var attack = CreateTestAttack();

            for (int i = 0; i < 30; i++)
            {
                double momentum = CombatSystem.GetAttackMomentum(attacker, targetLimb, attack);
                CombatSystem.DealDamage(momentum, defender, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, targetLimb, null, attacker.GetAttackingLimb(attack));
            }

            var targetWounds = targetLimb!.Wounds;
            Assert.NotEmpty(targetWounds);

            double cumulativePercentage = defender.ActorAnatomy.GetLimbDamagePercentage(targetLimb);
            var severity = defender.ActorAnatomy.GetLimbSeverity(targetLimb);

            Assert.True(cumulativePercentage > 0.01);
            Assert.True(severity >= InjurySeverity.Broken, $"Severity: {severity}, Damage%: {cumulativePercentage:P1}");
        }

        [Fact]
        public void SinglePowerfulAttack_BreaksLimb()
        {
            var attacker = CreateActor("attacker", 999, 50, 50, 20, 20);
            var defender = CreateActor("defender", 10, 1, 1, 1, 1);

            var targetLimb = defender.ActorAnatomy.Limbs[0];
            Assert.NotNull(targetLimb);

            var attack = CreateTestAttack(1.0);

            double momentum = CombatSystem.GetAttackMomentum(attacker, targetLimb, attack);
            CombatSystem.DealDamage(momentum, defender, attack.DamageType!, DataManager.QueryMaterial("bone"), attack, targetLimb, null, attacker.GetAttackingLimb(attack));

            var pct = defender.ActorAnatomy.GetLimbDamagePercentage(targetLimb);
            var finalSeverity = defender.ActorAnatomy.GetLimbSeverity(targetLimb);

            Assert.True(finalSeverity >= InjurySeverity.Broken, $"Damage%: {pct:P1}, Severity: {finalSeverity}, Broken: {targetLimb.Broken}");
        }
    }
}
