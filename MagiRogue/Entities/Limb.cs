using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public enum TypeOfLimb
    {
        Head,
        Torso,
        Arm,
        Leg,
        Foot,
        Hand,
        Tail
    }

    public class Limb
    {
        private int limbHp;

        public int LimbHp
        {
            get
            {
                if (limbHp > MaxLimbHp)
                {
                    return MaxLimbHp;
                }
                else if (limbHp < (LimbWeight * -2))
                {
                    Attached = false;
                    GameLoop.UIManager.MessageLog.Add($"You lost your {LimbName}");
                    return (LimbWeight * -2);
                }
                else
                    return limbHp;
            }
            set
            {
                if (value > MaxLimbHp)
                {
                    limbHp = MaxLimbHp;
                }
                else if (value < (LimbWeight * -2))
                {
                    Attached = false;
                    GameLoop.UIManager.MessageLog.Add($"You lost your {LimbName}");
                    limbHp = (LimbWeight * -2);
                }
                else
                    limbHp = value;
            }
        }
        public int MaxLimbHp { get; set; }
        public int LimbWeight { get; set; }
        public bool Attached { get; set; }

        /// <summary>
        /// Marks if the limb is right or left, being false left and right true.\n Also null means it's in the center
        /// </summary>
        public bool? Orientation { get; set; }
        public string LimbName { get; set; }
        public TypeOfLimb TypeLimb { get; set; }

        public Limb(TypeOfLimb limb, int limbHp, int maxLimbHp, int limbWeight, string limbName, bool? orientation)
        {
            TypeLimb = limb;

            MaxLimbHp = maxLimbHp;
            LimbHp = limbHp;
            LimbWeight = limbWeight;
            // Defaults to true
            Attached = true;
            LimbName = limbName;

            Orientation = orientation;
        }
    }
}