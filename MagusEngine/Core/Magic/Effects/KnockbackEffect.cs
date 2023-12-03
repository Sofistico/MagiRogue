﻿using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;

namespace MagusEngine.Core.Magic.Effects
{
    public class KnockbackEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public string SpellDamageTypeId { get; set; } = "blunt";
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; }
        public int BaseDamage { get; set; }
        public EffectType EffectType { get; set; } = EffectType.KNOCKBACK;
        public bool CanMiss { get; set; }
        public string? EffectMessage { get; set; }

        // meters per second
        public double PushForceInMPS { get; set; }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            var entity = caster.CurrentMap.GetEntityAt<MagiEntity>(target);
            if (entity is null && PushForceInMPS == 0)
                return;
            var force = PhysicsManager.CalculateNewton2Law(entity.Weight, PushForceInMPS);
            // calculate on many meters it will slide

            // then add friction
            var forceAfterFriction = PhysicsManager.CalculateFrictionToMovement(0.15, force);

            // then calculate damage as base damage + forceAfterFriction(energy not lost to friction)
            var damage = PhysicsManager.CalculateStrikeForce(entity.Weight, forceAfterFriction) + BaseDamage;
            // the acceleration isn't the same, and the meters is more the velocity of the object, since the formula would be:
            // V =  a * t which t is time, and spell resolution happens in a second or less after casting, then this simplification should logicaly work!
            int meters = (int)PushForceInMPS;
            BodyPart? bp = null;
            for (int i = 0; i < meters; i++)
            {
                if (entity is Actor actor)
                    bp = actor.GetAnatomy().Limbs.GetRandomItemFromList();
                CombatUtils.DealDamage(damage, entity, GetDamageType(), limbAttacked: bp);
            }
        }

        public DamageType? GetDamageType()
        {
            return DataManager.QueryDamageInData(SpellDamageTypeId);
        }
    }
}
