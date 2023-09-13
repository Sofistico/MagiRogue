using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class MageSightEffect : ISpellEffect
    {
        private bool hasMageSight;
        private int turnToRemove;

        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageTypes SpellDamageType { get; set; }

        public int Duration { get; set; }
        public int TurnApplied { get; set; }
        public int Radius { get; set; } = 0;
        public double ConeCircleSpan { get; set; }

        public bool TargetsTile { get; set; } = false;
        public EffectType EffectType { get; set; } = EffectType.MAGESIGHT;
        public int BaseDamage { get; set; } = 0;
        public bool CanMiss { get; set; }

        [JsonConstructor]
        public MageSightEffect(int duration)
        {
            Duration = duration;
            SpellDamageType = DamageTypes.None;
            AreaOfEffect = SpellAreaEffect.Self;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            if (hasMageSight)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You already have your Sight active"));
                return;
            }

            TurnApplied = Find.Universe.Time.Turns;

            //foreach (Tiles.NodeTile node in Find.CurrentMap.Tiles.Where(t => t is Tiles.NodeTile))
            //{
            //    node.RestoreOriginalAppearence();
            //}

            turnToRemove = TurnApplied + Duration;

            hasMageSight = true;
            Find.Universe.Time.TurnPassed += GetTime_TurnPassed;

            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You can See"));
        }

        private void GetTime_TurnPassed(object sender, TimeDefSpan e)
        {
            if (e.Seconds >= turnToRemove)
            {
                //foreach (Tiles.NodeTile node in Find.CurrentMap.Tiles.Where(t => t is Tiles.NodeTile))
                //{
                //    node.RestoreIllusionComponent();
                //}
                turnToRemove = 0;
                hasMageSight = false;

                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("Your close your Sigth"));
                Find.Universe.Time.TurnPassed -= GetTime_TurnPassed;
            }
        }
    }
}