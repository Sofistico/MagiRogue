using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic.Interfaces;

namespace MagusEngine.Core.Magic.Effects
{
    public class LightEffect : SpellEffectBase
    {
        public string? EffectMessage { get; set; }
        public int Duration { get; set; }

        public LightEffect()
        {
            EffectType = EffectType.LIGHT;
        }

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            throw new System.NotImplementedException();
        }
    }
}
