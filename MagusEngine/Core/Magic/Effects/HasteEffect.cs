using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.ECS.Components.EntityComponents.Status;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
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
        public bool IsResistable { get; set; }
        public string? EffectMessage { get; set; }
        public int Volume { get; set; }
        public bool IgnoresWall { get; set; }

        [JsonConstructor]
        public HasteEffect(SpellAreaEffect areaOfEffect, float hastePower, int duration, string spellDamageTypeId = "force")
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageTypeId = spellDamageTypeId;
            HastePower = hastePower;
            Duration = duration;
        }

        public void ApplyEffect(Point target, Actor caster, Spell spellCasted)
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
                Find.Universe.Time.Tick,
                Find.Universe.Time.Tick + (Duration * TimeDefSpan.CentisecondsPerSecond),
                EffectMessage);
            actor.AddComponent(haste);
        }

        public DamageType GetDamageType()
        {
            return DataManager.QueryDamageInData(SpellDamageTypeId);
        }
    }
}
