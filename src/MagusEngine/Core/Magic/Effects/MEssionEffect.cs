using System;
using MagusEngine.Core.Entities;

namespace MagusEngine.Core.Magic.Effects
{
    public class MEssionEffect : SpellEffectBase
    {
        public string? EffectMessage { get; set; }

        public MEssionEffect()
        {
            EffectType = Arquimedes.Enumerators.SpellEffectType.MEMISSION.ToString();
        }

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            throw new NotImplementedException();
        }
    }
}
