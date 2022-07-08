﻿using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using System.Linq;

namespace MagiRogue.GameSys.Magic.Effects
{
    public class MageSightEffect : ITimedEffect
    {
        private bool hasMageSight;
        private int turnToRemove;

        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }

        public int Duration { get; set; }
        public int TurnApplied { get; set; }
        public int Radius { get; set; } = 0;
        public bool TargetsTile { get; set; } = false;
        public EffectTypes EffectType { get; set; } = EffectTypes.MAGESIGHT;
        public int BaseDamage { get; set; } = 0;

        public MageSightEffect(int duration)
        {
            Duration = duration;
            SpellDamageType = DamageType.None;
            AreaOfEffect = SpellAreaEffect.Self;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            if (hasMageSight)
            {
                GameLoop.UIManager.MessageLog.Add("You already have your mage sight active");
                return;
            }

            TurnApplied = GameLoop.Universe.Time.Turns;

            foreach (Tiles.NodeTile node in GameLoop.GetCurrentMap().Tiles.Where(t => t is Tiles.NodeTile))
            {
                node.RestoreOriginalAppearence();
            }

            turnToRemove = TurnApplied + Duration;

            hasMageSight = true;
            GameLoop.Universe.Time.TurnPassed += GetTime_TurnPassed;

            GameLoop.UIManager.MessageLog.Add("You can see the unseen now");
        }

        private void GetTime_TurnPassed(object sender, Time.TimeDefSpan e)
        {
            if (e.Seconds >= turnToRemove)
            {
                foreach (Tiles.NodeTile node in GameLoop.GetCurrentMap().Tiles.Where(t => t is Tiles.NodeTile))
                {
                    node.RestoreIllusionComponent();
                }
                turnToRemove = 0;
                hasMageSight = false;

                GameLoop.UIManager.MessageLog.Add("Your eyes sees normally now");
                GameLoop.Universe.Time.TurnPassed -= GetTime_TurnPassed;
            }
        }
    }
}