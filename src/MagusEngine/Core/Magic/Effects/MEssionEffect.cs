using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic.Interfaces;

namespace MagusEngine.Core.Magic.Effects
{
    public class MEssionEffect : SpellEffectBase
    {
        public string? EffectMessage { get; set; }

        public MEssionEffect()
        {
            EffectType = EffectType.MEMISSION;
        }

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            throw new NotImplementedException();
        }
    }
}
