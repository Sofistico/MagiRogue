﻿using Arquimedes.Enumerators;
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
    public class DamageEffect : SpellEffectBase
    {
        public bool IsHealing { get; set; }
        public string? EffectMessage { get; set; }
        public int VelocityAttackMultiplier { get; set; }
        public double PenetrationPercentage { get; set; }

        public DamageEffect()
        {
            EffectType = Arquimedes.Enumerators.SpellEffectType.DAMAGE.ToString();
            SpellDamageTypeId = "blunt";
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

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            if (!IsHealing)
                DmgEff(target, caster, spellCasted);
            else
                HealEffect(target, caster, spellCasted);

            base.ApplyEffect(target, caster, spellCasted);
        }

        private void DmgEff(Point target, Actor caster, Spell spellCasted)
        {
            BaseDamage = Magic.CalculateSpellDamage(caster, spellCasted);

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

            //TODO: Make this work later
            // if (GetDamageType().TerrainInteraction != TerrainInteractionEnum.None)
            //     PhysicsSystem.DealWithTileInteraction(BaseDamage, Radius, Volume, GetDamageType();
        }

        private void HealEffect(Point target, Actor caster, Spell spellCasted)
        {
            BaseDamage = Magic.CalculateSpellDamage(caster, spellCasted);

            if (AreaOfEffect is SpellAreaEffect.Self)
            {
                CombatSystem.ApplyHealing(BaseDamage, caster);
            }
            else
            {
                Actor happyGuy = Find.CurrentMap!.GetEntityAt<Actor>(target)!;

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

        private Attack ReturnAttack()
        {
            return new()
            {
                AttackAbility = AbilityCategory.MagicShaping,
                DamageTypeId = SpellDamageTypeId ?? "blunt",
                VelocityAttackMultiplier = VelocityAttackMultiplier,
                ContactArea = Volume,
                PenetrationPercentage = PenetrationPercentage,
                PrepareVelocity = 1,
                RecoverVelocity = 1,
                Projectile = AreaOfEffect is not SpellAreaEffect.Struck,
            };
        }
    }
}
