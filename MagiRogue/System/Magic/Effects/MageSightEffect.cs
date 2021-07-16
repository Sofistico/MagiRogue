using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System.Linq;

namespace MagiRogue.System.Magic.Effects
{
    public class MageSightEffect : ISpellEffect, ITimedEffect
    {
        private bool hasMageSight;
        private int turnToRemove;

        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }

        public int Turns { get; set; }
        public int TurnApplied { get; set; }

        public MageSightEffect(int turnsToRemove)
        {
            Turns = turnsToRemove;
            SpellDamageType = DamageType.None;
            AreaOfEffect = SpellAreaEffect.Self;
        }

        public void ApplyEffect(Point target, Stat casterStats)
        {
            if (hasMageSight)
            {
                GameLoop.UIManager.MessageLog.Add("You already have your mage sight active");
                return;
            }

            TurnApplied = GameLoop.World.GetTime.Turns;

            foreach (Tiles.NodeTile node in GameLoop.World.CurrentMap.Tiles.Where(t => t is Tiles.NodeTile))
            {
                node.RestoreOriginalAppearence();
            }

            turnToRemove = TurnApplied + Turns;

            hasMageSight = true;
            GameLoop.World.GetTime.TurnPassed += GetTime_TurnPassed;

            GameLoop.UIManager.MessageLog.Add("You can see the unseen now");
        }

        private void GetTime_TurnPassed(object sender, Time.TimeDefSpan e)
        {
            if (e.Seconds >= turnToRemove)
            {
                foreach (Tiles.NodeTile node in GameLoop.World.CurrentMap.Tiles.Where(t => t is Tiles.NodeTile))
                {
                    node.RestoreIllusionComponent();
                }
                turnToRemove = 0;
                hasMageSight = false;

                GameLoop.UIManager.MessageLog.Add("Your eyes sees normally now");
                GameLoop.World.GetTime.TurnPassed -= GetTime_TurnPassed;
            }
        }
    }
}