using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
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
        string? SpellDamageTypeId { get; set; }
        int Radius { get; set; }
        double ConeCircleSpan { get; set; }
        bool TargetsTile { get; set; }
        int BaseDamage { get; set; }
        string EffectType { get; set; }
        bool CanMiss { get; set; }

        /// <summary>
        /// The volume occupied by the spell, should take into account only the volume that "hits" something, should be in cm³
        /// </summary>
        int Volume { get; set; }
        bool IsResistable { get; set; }
        bool IgnoresWall { get; set; }
        string? AnimationId { get; set; }

        void ApplyEffect(Point target, Actor caster, Spell spellCasted);

        DamageType? GetDamageType();
    }
}
