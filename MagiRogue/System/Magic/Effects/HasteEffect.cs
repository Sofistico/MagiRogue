﻿using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Magic.Effects
{
    public class HasteEffect : ISpellEffect
    {
        private int turnApllied;
        private float previousSpeed;
        private Stat currentStats;
        private bool isHasted;

        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }

        public float HastePower { get; set; }
        public int TurnToRemove { get; set; }

        public HasteEffect(SpellAreaEffect areaOfEffect, float hastePower, int turns,
            DamageType spellDamageType = DamageType.Force)
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            HastePower = hastePower;
            TurnToRemove = turns;
        }

        public void ApplyEffect(Point target, Stat casterStats)
        {
            switch (AreaOfEffect)
            {
                case SpellAreaEffect.Self:
                    if (TurnToRemove == 0)
                        GameLoop.World.GetTime.TurnPassed -= GetTime_TurnPassed;
                    if (isHasted)
                    {
                        GameLoop.UIManager.MessageLog.Add("Can only have one haste effect per time");
                        break;
                    }
                    currentStats = casterStats;
                    previousSpeed = casterStats.Speed;
                    casterStats.Speed += HastePower;
                    turnApllied = GameLoop.World.GetTime.Turns;
                    TurnToRemove += turnApllied;
                    isHasted = true;

                    GameLoop.World.GetTime.TurnPassed += GetTime_TurnPassed;

                    break;

                case SpellAreaEffect.Target:
                    break;

                case SpellAreaEffect.Ball:
                    break;

                case SpellAreaEffect.Shape:
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

        private void GetTime_TurnPassed(object sender, Time.TimeDefSpan e)
        {
            if (e.Seconds == TurnToRemove)
            {
                currentStats.Speed = previousSpeed;
                TurnToRemove = 0;
                isHasted = false;
            }
        }
    }
}