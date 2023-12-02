using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using System;

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
            var forceAfterFriction = PhysicsManager.CalculateFrictionToMovement(0.2, force);
        }

        public DamageType? GetDamageType()
        {
            return DataManager.QueryDamageInData(SpellDamageTypeId);
        }
    }
}
