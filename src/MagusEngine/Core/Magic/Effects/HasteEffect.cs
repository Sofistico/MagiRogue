using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Components.EntityComponents.Status;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class HasteEffect : SpellEffectBase
    {
        public float HastePower { get; set; }
        public int Duration { get; set; }
        public string? EffectMessage { get; set; }

        [JsonConstructor]
        public HasteEffect(SpellAreaEffect areaOfEffect, float hastePower, int duration, string spellDamageTypeId = "force")
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageTypeId = spellDamageTypeId;
            HastePower = hastePower;
            Duration = duration;
            EffectType = Arquimedes.Enumerators.SpellEffectType.HASTE.ToString();
        }

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            Haste(target);
        }

        private void Haste(Point target)
        {
            var actor = Find.CurrentMap?.GetEntityAt<Actor>(target);
            if (actor is not null)
                Haste(actor);
        }

        private void Haste(Actor actor)
        {
            if (actor.GetComponent<HasteComponent>(out _))
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("Can only have one haste effect active!"));
                return;
            }
            HasteComponent haste = new(HastePower,
                Find.Universe.Time.Tick,
                Find.Universe.Time.Tick + (Duration * TimeDefSpan.CentisecondsPerSecond),
                EffectMessage ?? "You feel yourself speeding up");
            actor.AddComponent(haste);
        }
    }
}
