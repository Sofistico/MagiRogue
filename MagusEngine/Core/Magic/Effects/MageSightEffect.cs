﻿using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.ECS.Components.ActorComponents.EffectComponents;
using MagusEngine.Services;
using MagusEngine.Systems;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class MageSightEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public string? SpellDamageTypeId { get; set; }

        public int Duration { get; set; }
        public int Radius { get; set; } = 0;
        public double ConeCircleSpan { get; set; }

        public bool TargetsTile { get; set; } = false;
        public EffectType EffectType { get; set; } = EffectType.MAGESIGHT;
        public int BaseDamage { get; set; } = 0;
        public bool CanMiss { get; set; }
        public string? EffectMessage { get; set; }

        [JsonConstructor]
        public MageSightEffect(int duration)
        {
            Duration = duration;
            AreaOfEffect = SpellAreaEffect.Self;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            if (caster.GetComponent(out SightComponent effect))
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You already have your Sight active"));
                return;
            }
            effect = new(Find.Universe.Time.Turns, Find.Universe.Time.Turns + Duration, EffectMessage);

            var actor = Find.CurrentMap.GetEntityAt<Actor>(target);
            actor.AddComponent(effect);
        }

        public DamageType? GetDamageType()
        {
            return null;
        }
    }
}
