using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class HasteEffect : ISpellEffect
    {
        private double previousSpeed;
        private double currentSpeed;
        private bool isHasted;
        private int turnToRemove;

        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageTypes SpellDamageType { get; set; }
        public int BaseDamage { get; set; }

        public float HastePower { get; set; }
        public int Duration { get; set; }
        public int TurnApplied { get; private set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; } = false;
        public EffectType EffectType { get; set; } = EffectType.HASTE;
        public bool CanMiss { get; set; }

        [JsonConstructor]
        public HasteEffect(SpellAreaEffect areaOfEffect, float hastePower, int duration,
            DamageTypes spellDamageType = DamageTypes.Force)
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            HastePower = hastePower;
            Duration = duration;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            switch (AreaOfEffect)
            {
                case SpellAreaEffect.Self:
                    Haste(caster.Position, spellCasted);
                    break;

                default:
                    Haste(target, spellCasted);
                    break;
            }
        }

        private void Haste(Point target, SpellBase spellCasted)
        {
            double targetStats = Find.CurrentMap.GetEntityAt<Actor>(target).GetActorBaseSpeed();

            // TODO: Refactor this shit
            if (turnToRemove == 0)
                Find.Universe.Time.TurnPassed -= GetTime_TurnPassed;
            if (isHasted)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("Can only have one haste effect per time"));
                return;
            }
            currentSpeed = targetStats;
            previousSpeed = targetStats;
            targetStats += HastePower;
            TurnApplied = Find.Universe.Time.Turns;
            turnToRemove = TurnApplied + Duration;
            isHasted = true;

            Find.Universe.Time.TurnPassed += GetTime_TurnPassed;
        }

        private void GetTime_TurnPassed(object sender, TimeDefSpan e)
        {
            if (e.Seconds >= turnToRemove)
            {
                currentSpeed = previousSpeed;
                turnToRemove = 0;
                isHasted = false;

                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("You feel yourself slowing down"));
                Find.Universe.Time.TurnPassed -= GetTime_TurnPassed;
            }
        }
    }
}