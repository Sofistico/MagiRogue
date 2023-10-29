using Arquimedes.Enumerators;
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
        public DamageTypes SpellDamageType { get; set; }

        public int Duration { get; set; }
        public int Radius { get; set; } = 0;
        public double ConeCircleSpan { get; set; }

        public bool TargetsTile { get; set; } = false;
        public EffectType EffectType { get; set; } = EffectType.MAGESIGHT;
        public int BaseDamage { get; set; } = 0;
        public bool CanMiss { get; set; }
        public string EffectMessage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        [JsonConstructor]
        public MageSightEffect(int duration)
        {
            Duration = duration;
            SpellDamageType = DamageTypes.None;
            AreaOfEffect = SpellAreaEffect.Self;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            if (caster.GetComponent(out SightComponent effect))
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You already have your Sight active"));
                return;
            }
            effect = new()
            {
                TurnApplied = Find.Universe.Time.Turns,
                TurnToRemove = effect.TurnApplied + Duration
            };

            var actor = Find.CurrentMap.GetEntityAt<Actor>(target);
            actor.AddComponent(effect);
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You can See", actor == caster));
            //Find.Universe.Time.TurnPassed += GetTime_TurnPassed;
        }

        //private void GetTime_TurnPassed(object sender, TimeDefSpan e)
        //{
        //    if (e.Seconds >= turnToRemove)
        //    {
        //        //foreach (Tiles.NodeTile node in Find.CurrentMap.Tiles.Where(t => t is Tiles.NodeTile))
        //        //{
        //        //    node.RestoreIllusionComponent();
        //        //}
        //        turnToRemove = 0;
        //        hasMageSight = false;

        //        Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("Your close your Sigth"));
        //        Find.Universe.Time.TurnPassed -= GetTime_TurnPassed;
        //    }
        //}
    }
}
