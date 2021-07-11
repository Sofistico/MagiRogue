using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;
using SadConsole;
using MagiRogue.Entities;

namespace MagiRogue.System.Tiles
{
    public class NodeTile : TileBase
    {
        private float _mpRecovering;

        public float MpPoints { get; set; }

        public int MaxMp { get; private set; }

        public float MpRecovering
        {
            get
            {
                _mpRecovering = (int)NodeStrength;
                return _mpRecovering;
            }
        }

        public int NodeStrength { get; set; }

        public bool IsDepleted
        {
            get
            {
                return MpPoints <= 0;
            }
        }

        public ColoredGlyph TrueAppearence;

        public NodeTile(Color foreground, Color background, Point position, int mpPower, int nodeStrength) :
            base(foreground, background, '*', (int)MapLayer.TERRAIN, position,
                "pure_magic", false, name: "Magic Node")
        {
            MpPoints = mpPower;
            MaxMp = mpPower;
            NodeStrength = nodeStrength;

            TrueAppearence = new ColoredGlyph();
            TrueAppearence.CopyAppearanceFrom(this);
        }

        public void RestoreOriginalAppearence()
        {
            CopyAppearanceFrom(TrueAppearence);
            if (GoRogueComponents.Contains<Components.IllusionComponent>())
            {
                GoRogueComponents.Remove("illusion");
            }
        }

        /// <summary>
        /// Restores the mp if the node is not depleted by an amount determined by it's Node Strengt enum
        /// </summary>
        private void RestoreMp()
        {
            if (!IsDepleted && MpPoints <= MaxMp)
            {
                MpPoints = (float)Math.Round(MpPoints + MpRecovering, 1);
            }
            else
                DestroyTile(new TileFloor(Position, "stone"));
        }

        public void DrainNode(Actor actor)
        {
            if (!IsDepleted)
            {
                int rndDrain = GoRogue.Random.GlobalRandom.DefaultRNG.Next(actor.Magic.ShapingSkills);

                MpPoints -= rndDrain;

                if (MpPoints <= 0)
                {
                    MpPoints = 0;
                    DestroyTile(new TileFloor(Position, "stone"));
                }

                GameLoop.UIManager.MessageLog.Add($"{actor.Name} drained {rndDrain} from node!");

                actor.Stats.AmbientMana += rndDrain;
            }
            else
            {
                DestroyTile(new TileFloor(Position, "stone"));

                GameLoop.UIManager.MessageLog.Add("The node here is already empty");
            }
        }

        public void SetUpNodeTurn(World world)
        {
            world.GetTime.TurnPassed += (_, __) =>
            {
                if ((__.Minutes + 1) % 5 == 0 && __.Seconds % 60 == 0)
                {
                    RestoreMp();
                }
            };
        }
    }

    public enum NodeStrength
    {
        Fading = -3,
        Feeble = 1,
        Weak = 2,
        Normal = 0,
        Strong = 3,
        Powerful = 4,
        DemigodLike = 6,
        Godlike = 10
    }
}