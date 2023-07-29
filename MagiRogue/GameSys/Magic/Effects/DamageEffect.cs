using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.Entities.Core;
using MagiRogue.Utils;
using Newtonsoft.Json;

namespace MagiRogue.GameSys.Magic.Effects
{
    /// <summary>
    /// Basic damage effect, can determine if it auto hits or if it heals
    /// </summary>
    public class DamageEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageTypes SpellDamageType { get; set; }
        public int BaseDamage { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; } = false;
        public EffectType EffectType { get; set; } = EffectType.DAMAGE;
        public bool IsHealing { get; set; }
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }

        [JsonConstructor]
        public DamageEffect(int dmg,
            SpellAreaEffect areaOfEffect,
            DamageTypes spellDamageType,
            bool canMiss = false,
            bool isHeal = false,
            int radius = 0,
            bool isResistable = false)
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
            BaseDamage = MagicManager.CalculateSpellDamage(caster, spellCasted);

            MagiEntity poorGuy = GameLoop.GetCurrentMap().GetEntityAt<MagiEntity>(target);

            if ((poorGuy == GameLoop.GetCurrentMap().ControlledEntitiy || poorGuy is Player)
                && AreaOfEffect is not SpellAreaEffect.Ball)
            {
                poorGuy = null;
            }

            if (poorGuy is null)
            {
                return;
            }

            CombatUtils.ResolveSpellHit(poorGuy, caster, spellCasted, this);
        }

        private void HealEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            BaseDamage = MagicManager.CalculateSpellDamage(caster, spellCasted);

            if (AreaOfEffect is SpellAreaEffect.Self)
                CombatUtils.ApplyHealing(BaseDamage, caster, SpellDamageType);
            else
            {
                Actor happyGuy = GameLoop.GetCurrentMap().GetEntityAt<Actor>(target);

                if (happyGuy == null)
                {
                    return;
                }

                if (happyGuy.CanBeAttacked)
                {
                    GameLoop.AddMessageLog("You can't heal what can't take damage");
                    return;
                }

                CombatUtils.ApplyHealing(BaseDamage, happyGuy, SpellDamageType);
            }
        }
    }
}