using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System;

namespace MagiRogue.System.Magic.Effects
{
    /// <summary>
    /// Basic damage effect, can determine if it auto hits or if it heals
    /// </summary>
    public class DamageEffect : ISpellEffect
    {
        private readonly bool isHealing;
        private readonly bool _canMiss;

        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }
        public int Damage { get; set; }
        public int Radius { get; set; }
        public bool TargetsTile => false;

        public DamageEffect(int dmg, SpellAreaEffect areaOfEffect, DamageType spellDamageType, bool canMiss = false,
            bool isHeal = false, int radius = 0)
        {
            Damage = dmg;
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            isHealing = isHeal;
            Radius = radius;
            _canMiss = canMiss;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            switch (AreaOfEffect)
            {
                case SpellAreaEffect.Self:
                    HealEffect(Point.None, caster, spellCasted);
                    break;

                default:
                    if (!isHealing)
                        DmgEff(target, caster, spellCasted);
                    else
                        HealEffect(target, caster, spellCasted);
                    break;
            }
        }

        private void DmgEff(Point target, Actor caster, SpellBase spellCasted)
        {
            Damage = Magic.CalculateSpellDamage(caster.Stats, spellCasted);

            Entity poorGuy = GameLoop.World.CurrentMap.GetEntityAt<Entity>(target);

            if ((poorGuy == GameLoop.World.CurrentMap.ControlledEntitiy || poorGuy is Player)
                && AreaOfEffect is not SpellAreaEffect.Ball)
            {
                poorGuy = null;
            }

            if (poorGuy is null)
            {
                return;
            }

            if (!_canMiss)
                CombatUtils.DealDamage(Damage, poorGuy, SpellDamageType);
            else
            {
                int diceRoll = GoRogue.DiceNotation.Dice.Roll($"1d20 + {caster.Stats.Precision}");
                if (poorGuy is Actor actor && diceRoll >= actor.Stats.Defense)
                {
                    CombatUtils.DealDamage(Damage, poorGuy, SpellDamageType);
                }
                else
                {
                    GameLoop.UIManager.MessageLog.Add($"{caster.Name} missed {poorGuy.Name}!");
                }
            }
        }

        private void HealEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            Stat casterStats = caster.Stats;

            Damage = Magic.CalculateSpellDamage(casterStats, spellCasted);

            if (AreaOfEffect is SpellAreaEffect.Self)
                CombatUtils.ApplyHealing(Damage, casterStats, SpellDamageType);
            else
            {
                Actor happyGuy = GameLoop.World.CurrentMap.GetEntityAt<Actor>(target);

                if (happyGuy == null)
                {
                    return;
                }

                if (happyGuy.CanBeAttacked)
                {
                    GameLoop.UIManager.MessageLog.Add("You can't heal what can't take damage");
                    return;
                }

                CombatUtils.ApplyHealing(Damage, happyGuy.Stats, SpellDamageType);
            }
        }
    }
}