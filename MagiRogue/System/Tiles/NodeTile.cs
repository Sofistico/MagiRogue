using MagiRogue.Entities;
using SadConsole;
using SadRogue.Primitives;
using System;

namespace MagiRogue.System.Tiles
{
    public sealed class NodeTile : TileBase
    {
        private float _mpRecovering;
        private Components.IllusionComponent illusion;

        public float MpPoints { get; set; }

        public int MaxMp { get; private set; }

        public float MpRecovering
        {
            get
            {
                _mpRecovering = NodeStrength;
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

        public ColoredGlyph TrueAppearence { get; private set; }

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
            LastSeenAppereance.CopyAppearanceFrom(TrueAppearence);
            if (GoRogueComponents.Contains<Components.IllusionComponent>())
            {
                illusion = GoRogueComponents.GetFirstOrDefault<Components.IllusionComponent>();
                GoRogueComponents.Remove(Components.IllusionComponent.Tag);
            }
        }

        public void RestoreIllusionComponent()
        {
            GoRogueComponents.Add(illusion, Components.IllusionComponent.Tag);
            LastSeenAppereance.CopyAppearanceFrom
                (GoRogueComponents.GetFirstOrDefault<Components.IllusionComponent>().FakeAppearence);
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
                DestroyTile(BecomeNextTile());
        }

        public void DrainNode(Actor actor)
        {
            if (!IsDepleted)
            {
                int rndDrain = GoRogue.Random.GlobalRandom.DefaultRNG.Next(actor.Magic.ShapingSkill);

                MpPoints -= rndDrain;

                if (MpPoints <= 0)
                {
                    MpPoints = 0;
                    DestroyTile(BecomeNextTile());
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

        public override void DestroyTile(TileBase changeTile, Item itemDropped = null)
        {
            GameLoop.World.Time.TurnPassed -= GetTime_NodeTurnPassed;

            base.DestroyTile(changeTile, itemDropped);
        }

        public void SetUpNodeTurn(Universe world) => world.Time.TurnPassed += GetTime_NodeTurnPassed;

        private void DestroyNodeTurn(Universe world) => world.Time.TurnPassed -= GetTime_NodeTurnPassed;

        private void GetTime_NodeTurnPassed(object sender, Time.TimeDefSpan e)
        {
            if ((e.Minutes + 1) % 5 == 0 && e.Seconds % 60 == 0)
            {
                RestoreMp();
            }
        }

        public Item PickUp()
        {
            DestroyTile(BecomeNextTile());

            DestroyNodeTurn(GameLoop.World);

            return Data.EntityFactory.ItemCreator(Position,
                new Item(Foreground, Background, "Node Vis", Glyph, Position, NodeStrength,
                (float)MaterialOfTile.Density));
        }

        private TileBase BecomeNextTile()
        {
            foreach (Point point in Position.GetDirectionPoints())
            {
                TileBase tile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(point);
                if (LastSeenAppereance.Matches(tile))
                {
                    return tile;
                }
            }

            return null;
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