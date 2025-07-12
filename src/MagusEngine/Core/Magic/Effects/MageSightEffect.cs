using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Components.EntityComponents.Status;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class MageSightEffect : SpellEffectBase
    {
        public int Duration { get; set; }
        public string EffectMessage { get; set; }

        [JsonConstructor]
        public MageSightEffect(int duration)
        {
            Duration = duration;
            AreaOfEffect = SpellAreaEffect.Self;
            EffectType = Arquimedes.Enumerators.SpellEffectType.MAGESIGHT.ToString();
        }

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            var actor = Find.CurrentMap?.GetEntityAt<Actor>(target);
            if (actor?.GetComponent(out SightComponent _) == true)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You already have your Sight active"));
                return;
            }
            SightComponent effect = new(Find.Universe.Time.Tick, Find.Universe.Time.Tick + (Duration * TimeDefSpan.CentisecondsPerSecond), EffectMessage);
            actor?.AddComponent(effect);
        }
    }
}
