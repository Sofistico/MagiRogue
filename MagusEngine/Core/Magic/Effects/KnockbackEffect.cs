using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
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
        public int BaseDamage { get; set; } // works as the mass that will collide with the target/s
        public EffectType EffectType { get; set; } = EffectType.KNOCKBACK;
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }  
        public string? EffectMessage { get; set; }
        public int Volume { get; set; }

        public void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            var entity = caster.CurrentMagiMap.GetEntityAt<MagiEntity>(target);
            var hit = CombatSystem.ResolveSpellHit(entity, caster, spellCasted, this);
            if (hit)
                PhysicsSystem.DealWithPushes(entity, BaseDamage, Direction.GetDirection(entity.Position - caster.Position));
        }

        // not used in this case
        public DamageType? GetDamageType() => DataManager.QueryDamageInData(SpellDamageTypeId);
    }
}
