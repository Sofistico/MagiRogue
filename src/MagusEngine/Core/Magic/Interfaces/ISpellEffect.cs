using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Systems;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagusEngine.Core.Magic.Interfaces
{
    /// <summary>
    /// Defines the interface that a new effect will have
    /// </summary>
    public interface ISpellEffect
    {
        [JsonConverter(typeof(StringEnumConverter))]
        SpellAreaEffect AreaOfEffect { get; set; }
        string SpellDamageTypeId { get; set; }
        int Radius { get; set; }
        double ConeCircleSpan { get; set; }
        bool TargetsTile { get; set; }
        int BaseDamage { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        EffectType EffectType { get; set; }
        bool CanMiss { get; set; }

        /// <summary>
        /// The volume occupied by the spell, should take into account only the volume that "hits" something, should be in cm³
        /// </summary>
        int Volume { get; set; }
        bool IsResistable { get; set; }
        bool IgnoresWall { get; set; }
        string Animation { get; set; }

        void ApplyEffect(Point target, Actor caster, Spell spellCasted);

        DamageType? GetDamageType();
    }

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
        public string SpellDamageTypeId { get; set; }

        /// <summary>
        /// The volume occupied by the spell, should take into account only the volume that "hits" something, should be in cm3
        /// </summary>
        public int Volume { get; set; }
        public bool IgnoresWall { get; set; }
        public string Animation { get; set; }

        public virtual void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
        }

        public virtual DamageType? GetDamageType()
        {
            if (string.IsNullOrEmpty(SpellDamageTypeId))
            {
                // Handle invalid ID appropriately, e.g., return null or throw an exception
                return null;
            }
            return DataManager.QueryDamageInData(SpellDamageTypeId);
        }

        public virtual void AnimateEffect(Point target, Actor caster, Spell spellCasted)
        {
        }
    }
}
