﻿using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;

namespace MagiRogue.System.Magic.Effects
{
    public class HasteEffect : ISpellEffect, ITimedEffect
    {
        private float previousSpeed;
        private Stat currentStats;
        private bool isHasted;
        private int turnToRemove;

        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }

        public float HastePower { get; set; }
        public int Turns { get; private set; }
        public int TurnApplied { get; private set; }
        public int Radius { get; set; } = 0;
        public bool TargetsTile => false;

        public HasteEffect(SpellAreaEffect areaOfEffect, float hastePower, int turns,
            DamageType spellDamageType = DamageType.Force)
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            HastePower = hastePower;
            Turns = turns;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            switch (AreaOfEffect)
            {
                case SpellAreaEffect.Self:
                    Haste(target, caster, spellCasted);
                    break;

                case SpellAreaEffect.Target:
                    break;

                case SpellAreaEffect.Ball:
                    break;

                case SpellAreaEffect.Beam:
                    break;

                case SpellAreaEffect.Level:
                    break;

                case SpellAreaEffect.World:
                    break;

                default:
                    break;
            }
        }

        private void Haste(Point target, Actor caster, SpellBase spellCasted)
        {
            Stat casterStats = caster.Stats;

            if (turnToRemove == 0)
                GameLoop.World.GetTime.TurnPassed -= GetTime_TurnPassed;
            if (isHasted)
            {
                GameLoop.UIManager.MessageLog.Add("Can only have one haste effect per time");
                return;
            }
            currentStats = casterStats;
            previousSpeed = casterStats.Speed;
            casterStats.Speed += HastePower;
            TurnApplied = GameLoop.World.GetTime.Turns;
            turnToRemove = TurnApplied + Turns;
            isHasted = true;

            GameLoop.World.GetTime.TurnPassed += GetTime_TurnPassed;
        }

        private void GetTime_TurnPassed(object sender, Time.TimeDefSpan e)
        {
            if (e.Seconds >= turnToRemove)
            {
                currentStats.Speed = previousSpeed;
                turnToRemove = 0;
                isHasted = false;

                GameLoop.UIManager.MessageLog.Add("You feel yourself slowing down");
                GameLoop.World.GetTime.TurnPassed -= GetTime_TurnPassed;
            }
        }
    }
}