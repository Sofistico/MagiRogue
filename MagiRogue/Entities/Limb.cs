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
        R_arm,
        R_hand,
        L_arm,
        L_hand,
        R_leg,
        R_foot,
        L_leg,
        L_foot,
        R_FrontalLeg,
        L_FrontalLeg,
        R_FrontalPaw,
        L_FrontalPaw,
        R_BackPaw,
        L_BackPaw,
        Tail,
        R_wing,
        L_wing,
        R_BackLeg,
        L_BackLeg,
    }

    public class Limb
    {
        private readonly TypeOfLimb typeLimb;
        private int limbHp;
        private int maxLimbHp;

        public int LimbHp
        {
            get
            {
                if (limbHp > maxLimbHp)
                {
                    return maxLimbHp;
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
                if (value > maxLimbHp)
                {
                    limbHp = maxLimbHp;
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
        public string LimbName { get; set; }

        public Limb(TypeOfLimb limb, int limbHp, int maxLimbHp, int limbWeight, string limbName)
        {
            typeLimb = limb;

            MaxLimbHp = maxLimbHp;
            LimbHp = limbHp;
            LimbWeight = limbWeight;
            // Defaults to true
            Attached = true;
            LimbName = limbName;
        }

        public TypeOfLimb GetTypeOfLimb() => typeLimb;
    }
}