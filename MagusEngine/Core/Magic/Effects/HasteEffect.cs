using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.ECS.Components.ActorComponents.EffectComponents;
using MagusEngine.Services;
using MagusEngine.Systems;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class HasteEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public string SpellDamageTypeId { get; set; }
        public int BaseDamage { get; set; }

        public float HastePower { get; set; }
        public int Duration { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; } = false;
        public EffectType EffectType { get; set; } = EffectType.HASTE;
        public bool CanMiss { get; set; }
        public string EffectMessage { get; set; }

        [JsonConstructor]
        public HasteEffect(SpellAreaEffect areaOfEffect, float hastePower, int duration, string spellDamageTypeId = "force")
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageTypeId = spellDamageTypeId;
            HastePower = hastePower;
            Duration = duration;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            switch (AreaOfEffect)
            {
                case SpellAreaEffect.Self:
                    Haste(caster);
                    break;

                default:
                    Haste(target);
                    break;
            }
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
                Find.Universe.Time.Turns,
                Find.Universe.Time.Turns + Duration,
                EffectMessage);
            actor.AddComponent(haste);
        }
    }
}
