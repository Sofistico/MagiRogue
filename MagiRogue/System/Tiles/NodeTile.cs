using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;
using SadConsole;

namespace MagiRogue.System.Tiles
{
    public class NodeTile : TileBase
    {
        private float _mpRecovering;

        public int MpPower { get; set; }

        public float MpRecovering
        {
            get
            {
                _mpRecovering = (int)NodeStrength;
                return _mpRecovering;
            }
        }

        public NodeStrength NodeStrength { get; set; }

        public bool IsDepleted
        {
            get
            {
                return MpPower <= 0;
            }
        }

        public NodeTile(Color foreground, Color background, Point position, int mpPower, NodeStrength nodeStrength) :
            base(foreground, background, '*', (int)MapLayer.TERRAIN, position,
                "pure_magic", false, name: "Magic Node")
        {
            MpPower = mpPower;
            NodeStrength = nodeStrength;
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