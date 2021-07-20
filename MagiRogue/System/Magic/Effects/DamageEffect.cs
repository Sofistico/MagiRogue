using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System;

namespace MagiRogue.System.Magic.Effects
{
    public class DamageEffect : ISpellEffect
    {
        private readonly bool isHealing;

        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }
        public int Damage { get; set; }

        public DamageEffect(int dmg, SpellAreaEffect areaOfEffect, DamageType spellDamageType, bool isHeal = false)
        {
            Damage = dmg;
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            isHealing = isHeal;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            switch (AreaOfEffect)
            {
                case SpellAreaEffect.Self:
                    HealEffect(Point.None, caster, spellCasted);
                    break;

                case SpellAreaEffect.Target:
                    if (!isHealing)
                        DmgEff(target, caster, spellCasted);
                    else
                        HealEffect(target, caster, spellCasted);
                    break;

                case SpellAreaEffect.Ball:
                    break;

                case SpellAreaEffect.Beam:
                    if (!isHealing)
                        DmgEff(target, caster, spellCasted);
                    else
                        HealEffect(target, caster, spellCasted);
                    break;

                case SpellAreaEffect.Level:
                    break;

                case SpellAreaEffect.World:
                    throw new NotImplementedException();

                default:
                    break;
            }
        }

        private void DmgEff(Point target, Actor caster, SpellBase spellCasted)
        {
            Damage = Magic.CalculateSpellDamage(caster.Stats, spellCasted);

            Entity poorGuy = GameLoop.World.CurrentMap.GetEntityAt<Entity>(target);
            if (poorGuy == null)
            {
                return;
            }

            if (poorGuy == GameLoop.World.CurrentMap.ControlledEntitiy || poorGuy is Player)
            {
                poorGuy = GameLoop.World.CurrentMap.GetClosestEntity(poorGuy.Position, 1);
            }

            CombatUtils.DealDamage(Damage, poorGuy, SpellDamageType);
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