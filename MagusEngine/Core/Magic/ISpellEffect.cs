using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagusEngine.Core.Magic
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
        string? EffectMessage { get; set; }
        int Volume { get; set; }
        bool IsResistable { get; set; }

        void ApplyEffect(Point target, Actor caster, SpellBase spellCasted);

        DamageType? GetDamageType();
    }

    public interface IPermEffect
    {
        public int NodeCost { get; set; }

        public ISpellEffect Enchantment { get; set; }
        public Actor Caster { get; set; }

        public void Enchant(int nodesSacrificed);
    }
}
