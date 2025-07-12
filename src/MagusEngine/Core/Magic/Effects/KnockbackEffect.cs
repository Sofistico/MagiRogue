using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using SadRogue.Primitives;

namespace MagusEngine.Core.Magic.Effects
{
    public class KnockbackEffect : SpellEffectBase
    {
        public string? EffectMessage { get; set; }

        public KnockbackEffect()
        {
            SpellDamageTypeId = "blunt";
            EffectType = Arquimedes.Enumerators.SpellEffectType.KNOCKBACK.ToString();
        }

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            var entity = caster.CurrentMagiMap.GetEntityAt<MagiEntity>(target);
            var hit = CombatSystem.ResolveSpellHit(entity, caster, spellCasted, this);
            if (hit)
                PhysicsSystem.DealWithPushes(entity, BaseDamage, Direction.GetDirection(entity.Position - caster.Position));
        }
    }
}
