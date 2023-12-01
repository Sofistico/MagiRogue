using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
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

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            throw new NotImplementedException();
        }

        public DamageType? GetDamageType()
        {
            throw new NotImplementedException();
        }
    }
}
