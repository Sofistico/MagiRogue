using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;

namespace MagiRogue.GameSys.Magic.Effects
{
    public class HasteEffect : IHasteEffect
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
        public int Radius { get; set; } = 0;
        public bool TargetsTile { get; set; } = false;
        public EffectType EffectType { get; set; } = EffectType.HASTE;

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
            double targetStats = GameLoop.GetCurrentMap().GetEntityAt<Actor>(target).GetActorBaseSpeed();

            // TODO: Refactor this shit
            if (turnToRemove == 0)
                GameLoop.Universe.Time.TurnPassed -= GetTime_TurnPassed;
            if (isHasted)
            {
                GameLoop.AddMessageLog("Can only have one haste effect per time");
                return;
            }
            currentSpeed = targetStats;
            previousSpeed = targetStats;
            targetStats += HastePower;
            TurnApplied = GameLoop.Universe.Time.Turns;
            turnToRemove = TurnApplied + Duration;
            isHasted = true;

            GameLoop.Universe.Time.TurnPassed += GetTime_TurnPassed;
        }

        private void GetTime_TurnPassed(object sender, Time.TimeDefSpan e)
        {
            if (e.Seconds >= turnToRemove)
            {
                currentSpeed = previousSpeed;
                turnToRemove = 0;
                isHasted = false;

                GameLoop.AddMessageLog("You feel yourself slowing down");
                GameLoop.Universe.Time.TurnPassed -= GetTime_TurnPassed;
            }
        }
    }
}