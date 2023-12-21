using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils;
using SadRogue.Primitives;

namespace MagusEngine.Core.Magic.Effects
{
    public class KnockbackEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public string SpellDamageTypeId { get; set; } = "blunt";
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; }
        public int BaseDamage { get; set; } // does nothing
        public EffectType EffectType { get; set; } = EffectType.KNOCKBACK;
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }
        public string? EffectMessage { get; set; }

        // meters per second
        public double PushForceInMPS { get; set; }
        public int Volume { get; set; }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            var entity = caster.MagiMap.GetEntityAt<MagiEntity>(target);
            var hit = CombatUtils.ResolveSpellHit(entity, caster, spellCasted, this);
            if (hit)
                PhysicsManager.DealWithPushes(entity, PushForceInMPS, Direction.GetDirection(entity.Position - caster.Position), GetDamageType());
        }

        public DamageType? GetDamageType()
        {
            return DataManager.QueryDamageInData(SpellDamageTypeId);
        }
    }
}
