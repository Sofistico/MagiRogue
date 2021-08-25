using MagiRogue.Data;
using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System;

namespace MagiRogue.System.Magic.Effects
{
    /// <summary>
    /// Basic damage effect, can determine if it auto hits or if it heals
    /// </summary>
    public class DamageEffect : IDamageSpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }
        public int BaseDamage { get; set; }
        public int Radius { get; set; }
        public bool TargetsTile { get; set; } = false;
        public EffectTypes EffectType { get; set; } = EffectTypes.DAMAGE;
        public bool IsHealing { get; set; }
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }

        public DamageEffect(int dmg, SpellAreaEffect areaOfEffect, DamageType spellDamageType, bool canMiss = false,
            bool isHeal = false, int radius = 0, bool isResistable = false)
        {
            BaseDamage = dmg;
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            IsHealing = isHeal;
            Radius = radius;
            CanMiss = canMiss;
            IsResistable = isResistable;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            switch (AreaOfEffect)
            {
                case SpellAreaEffect.Self:
                    HealEffect(Point.None, caster, spellCasted);
                    break;

                default:
                    if (!IsHealing)
                        DmgEff(target, caster, spellCasted);
                    else
                        HealEffect(target, caster, spellCasted);
                    break;
            }
        }

        private void DmgEff(Point target, Actor caster, SpellBase spellCasted)
        {
            BaseDamage = Magic.CalculateSpellDamage(caster.Stats, spellCasted);

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

            ResolveHit(poorGuy, caster, spellCasted);
        }

        private void ResolveResist(Entity poorGuy, Actor caster, SpellBase spellCasted)
        {
            int luck = GoRogue.DiceNotation.Dice.Roll("1d20");
            if (Magic.PenetrateResistance(spellCasted, caster, poorGuy, luck))
            {
                CombatUtils.DealDamage(BaseDamage, poorGuy, SpellDamageType);
            }
            else
                GameLoop.UIManager.MessageLog.Add($"{poorGuy.Name} resisted the effects of {spellCasted.SpellName}");
        }

        private void ResolveHit(Entity poorGuy, Actor caster, SpellBase spellCasted)
        {
            if (!CanMiss)
            {
                ResolveResist(poorGuy, caster, spellCasted);
            }
            else
            {
                int diceRoll = GoRogue.DiceNotation.Dice.Roll($"1d20 + {caster.Stats.Precision}");
                if (poorGuy is Actor actor && diceRoll >= actor.Stats.Protection)
                {
                    ResolveResist(poorGuy, caster, spellCasted);
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

            BaseDamage = Magic.CalculateSpellDamage(casterStats, spellCasted);

            if (AreaOfEffect is SpellAreaEffect.Self)
                CombatUtils.ApplyHealing(BaseDamage, casterStats, SpellDamageType);
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

                CombatUtils.ApplyHealing(BaseDamage, happyGuy.Stats, SpellDamageType);
            }
        }
    }
}