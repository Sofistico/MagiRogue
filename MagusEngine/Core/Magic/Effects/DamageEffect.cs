using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Services;
using MagusEngine.Systems;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    /// <summary>
    /// Basic damage effect, can determine if it auto hits or if it heals
    /// </summary>
    public class DamageEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public int BaseDamage { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; }
        public EffectType EffectType { get; set; } = EffectType.DAMAGE;
        public bool IsHealing { get; set; }
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }
        public string? EffectMessage { get; set; }
        public string SpellDamageTypeId { get; set; }
        /// <summary>
        /// The volume occupied by the spell, should take into account only the volume that "hits" something, should be in cm3
        /// </summary>
        public int Volume { get; set; }
        public int VelocityMultiplier { get; set; }
        public double PenetrationPercentage { get; set; }

        public DamageEffect()
        {
        }

        [JsonConstructor]
        public DamageEffect(int dmg,
            SpellAreaEffect areaOfEffect,
            string spellDamageTypeId,
            bool canMiss = false,
            bool isHeal = false,
            int radius = 0,
            bool isResistable = false)
        {
            BaseDamage = dmg;
            AreaOfEffect = areaOfEffect;
            SpellDamageTypeId = spellDamageTypeId;
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

            MagiEntity? poorGuy = Find.CurrentMap?.GetEntityAt<MagiEntity>(target);

            if ((poorGuy == Find.CurrentMap?.ControlledEntitiy || poorGuy is Player)
                && AreaOfEffect is not SpellAreaEffect.Ball)
            {
                poorGuy = null;
            }

            if (poorGuy is null)
                return;

            if (!poorGuy.CanBeAttacked)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You can't target this!"));
                return;
            }

            CombatSystem.ResolveSpellHit(poorGuy, caster, spellCasted, this, ReturnAttack());
        }

        private void HealEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            BaseDamage = MagicManager.CalculateSpellDamage(caster, spellCasted);

            if (AreaOfEffect is SpellAreaEffect.Self)
            {
                CombatSystem.ApplyHealing(BaseDamage, caster);
            }
            else
            {
                Actor happyGuy = Find.CurrentMap.GetEntityAt<Actor>(target);

                if (happyGuy == null)
                {
                    return;
                }

                if (happyGuy.CanBeAttacked)
                {
                    Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You can't heal this!"));
                    return;
                }

                CombatSystem.ApplyHealing(BaseDamage, happyGuy);
            }
        }

        public DamageType GetDamageType()
        {
            return DataManager.QueryDamageInData(SpellDamageTypeId);
        }

        public Attack ReturnAttack()
        {
            return new()
            {
                AttackAbility = AbilityCategory.MagicShaping,
                DamageTypeId = SpellDamageTypeId,
                VelocityMultiplier = VelocityMultiplier,
                ContactArea = Volume,
                PenetrationPercentage = PenetrationPercentage,
                PrepareVelocity = 1,
                RecoverVelocity = 1,
                Projectile = AreaOfEffect is not SpellAreaEffect.Struck,
            };
        }
    }
}
