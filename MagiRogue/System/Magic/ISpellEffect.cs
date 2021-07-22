using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;

namespace MagiRogue.System.Magic
{
    public interface ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }
        public int Radius { get; set; }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted);
    }
}