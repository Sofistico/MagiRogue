using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Core.Magic.Effects
{
    public class MEssionEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public string SpellDamageTypeId { get; set; } = null!;
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; }
        public int BaseDamage { get; set; }
        public EffectType EffectType { get; set; }
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }
        public string? EffectMessage { get; set; }
        public int Volume { get; set; }
        public bool IgnoresWall { get; set; }

        public void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            throw new NotImplementedException();
        }

        public DamageType? GetDamageType()
        {
            throw new NotImplementedException();
        }
    }
}
