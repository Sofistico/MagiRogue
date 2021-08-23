using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;
using MagiRogue.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MagiRogue.System.Magic
{
    /// <summary>
    /// Defines the interface that a new effect will have
    /// </summary>
    public interface ISpellEffect
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SpellAreaEffect AreaOfEffect { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DamageType SpellDamageType { get; set; }
        public int Radius { get; set; }
        public bool TargetsTile { get; set; }
        public int BaseDamage { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EffectTypes EffectType { get; set; }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted);
    }

    public interface IDamageSpellEffect : ISpellEffect
    {
        public bool IsHealing { get; set; }
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }
    }

    public interface IHasteEffect : ITimedEffect
    {
        public float HastePower { get; set; }
    }

    public interface ITimedEffect : ISpellEffect
    {
        /// <summary>
        /// When in the time has the effect been applied
        /// </summary>
        public int TurnApplied { get; }

        /// <summary>
        /// How many turns this effect will be applied to
        /// </summary>
        public int Duration { get; }
    }

    public interface IPermEffect
    {
        public int NodeCost { get; set; }

        public ISpellEffect Enchantment { get; set; }
        public Actor Caster { get; set; }

        public void Enchant(int nodesSacrificed);
    }
}