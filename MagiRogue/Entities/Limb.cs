using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MagiRogue.Entities
{
    [DataContract]
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

    [DataContract]
    public class Limb
    {
        private int limbHp;

        [DataMember]
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
        [DataMember]
        public int MaxLimbHp { get; set; }
        [DataMember]
        public int LimbWeight { get; set; }
        [DataMember]
        public bool Attached { get; set; }

        /// <summary>
        /// Marks if the limb is right, left, or center.
        /// </summary>
        [DataContract]
        public enum LimbOrientation { Right, Left, Center }

        /// <summary>
        /// Marks if the limb is right, left, or center, this is the property.
        /// </summary>
        [DataMember]
        public LimbOrientation Orientation { get; set; }

        [DataMember]
        public string LimbName { get; set; }

        [DataMember]
        public TypeOfLimb TypeLimb { get; set; }

        public Limb(TypeOfLimb limb, int limbHp, int maxLimbHp, int limbWeight, string limbName, LimbOrientation orientation)
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