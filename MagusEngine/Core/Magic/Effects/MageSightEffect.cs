using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.ECS.Components.MagiObjComponents.Status;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class MageSightEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public string SpellDamageTypeId { get; set; }

        public int Duration { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool IsResistable { get; set; }
        public bool TargetsTile { get; set; }
        public EffectType EffectType { get; set; } = EffectType.MAGESIGHT;
        public int BaseDamage { get; set; } = 0;
        public bool CanMiss { get; set; }
        public string? EffectMessage { get; set; }
        public int Volume { get; set; }
        public bool IgnoresWall { get; set; }

        [JsonConstructor]
        public MageSightEffect(int duration)
        {
            Duration = duration;
            AreaOfEffect = SpellAreaEffect.Self;
        }

        public void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            if (caster.GetComponent(out SightComponent _))
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You already have your Sight active"));
                return;
            }
            SightComponent effect = new(Find.Universe.Time.Tick, Find.Universe.Time.Tick + (Duration * TimeDefSpan.CentisecondsPerSecond), EffectMessage);

            var actor = Find.CurrentMap?.GetEntityAt<Actor>(target);
            actor?.AddComponent(effect);
        }

        public DamageType? GetDamageType()
        {
            return null;
        }
    }
}
