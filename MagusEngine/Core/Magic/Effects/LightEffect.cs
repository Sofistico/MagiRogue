using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;

namespace MagusEngine.Core.Magic.Effects
{
    public class LightEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public string SpellDamageTypeId { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; }
        public int BaseDamage { get; set; }
        public EffectType EffectType { get; set; } = EffectType.LIGHT;
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }
        public string? EffectMessage { get; set; }
        public int Duration { get; set; }
        public int Volume { get; set; }

        public void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            throw new System.NotImplementedException();
        }

        public DamageType? GetDamageType()
        {
            throw new System.NotImplementedException();
        }
    }
}
