using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;

namespace MagiRogue.System.Magic
{
    /// <summary>
    /// Defines the interface that a new effect will have
    /// </summary>
    public interface ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }
        public int Radius { get; set; }
        public bool TargetsTile { get; }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted);
    }
}