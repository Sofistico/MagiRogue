using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Exceptions;
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
        public string EffectType { get; set; } = null!; // this can't be null
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }
        public string? SpellDamageTypeId { get; set; }

        /// <summary>
        /// The volume occupied by the spell, should take into account only the volume that "hits" something, should be in cm3
        /// </summary>
        public int Volume { get; set; }
        public bool IgnoresWall { get; set; }
        public string? AnimationId { get; set; }

        public virtual void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
        }

        public virtual DamageType GetDamageType()
        {
            if (string.IsNullOrEmpty(SpellDamageTypeId))
            {
                // Handle invalid ID appropriately, e.g., return null
                throw new NullValueException(nameof(SpellDamageTypeId));
            }
            return DataManager.QueryDamageInData(SpellDamageTypeId) ?? throw new NullValueException("DamageType was null", null);
        }
    }
}
