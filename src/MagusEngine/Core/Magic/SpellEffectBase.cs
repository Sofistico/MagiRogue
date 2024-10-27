using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Systems;

namespace MagusEngine.Core.Magic
{
    public abstract class SpellEffectBase : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public int BaseDamage { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; }
        public EffectType EffectType { get; set; }
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }
        public string? SpellDamageTypeId { get; set; }

        /// <summary>
        /// The volume occupied by the spell, should take into account only the volume that "hits" something, should be in cm3
        /// </summary>
        public int Volume { get; set; }
        public bool IgnoresWall { get; set; }
        public string? Animation { get; set; }

        public virtual void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
        }

        public virtual DamageType? GetDamageType()
        {
            if (string.IsNullOrEmpty(SpellDamageTypeId))
            {
                // Handle invalid ID appropriately, e.g., return null
                return null;
            }
            return DataManager.QueryDamageInData(SpellDamageTypeId);
        }

        public virtual void AnimateEffect(Point target, Actor caster, Spell spellCasted)
        {
        }
    }
}
