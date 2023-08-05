using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
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
        public SpellAreaEffect AreaOfEffect { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DamageTypes SpellDamageType { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; }
        public int BaseDamage { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EffectType EffectType { get; set; }
        bool CanMiss { get; set; }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted);
    }

    public interface IPermEffect
    {
        public int NodeCost { get; set; }

        public ISpellEffect Enchantment { get; set; }
        public Actor Caster { get; set; }

        public void Enchant(int nodesSacrificed);
    }
}