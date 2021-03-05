using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MagiRogue.Entities.Data
{
    [DataContract]
    [Serializable]
    public class LimbTemplate
    {
        [DataMember]
        public TypeOfLimb LimbType { get; set; }
        [DataMember]
        public int MaxLimbHp { get; set; }
        [DataMember]
        public int LimbHp { get; set; }
        [DataMember]
        public double LimbWeight { get; set; }
        [DataMember]
        public string LimbName { get; set; }
        [DataMember]
        public Limb.LimbOrientation LimbOrientation { get; set; }
        [DataMember]
        public string LimbMaterialId { get; set; }

        // Here will be a class designed to reliable build different types of anatomys, for use in a future dictionary
        // static class for anatomies

        public LimbTemplate()
        {
        }

        public static List<Limb> BasicHumanoidBody(Actor actor)
        {
            List<Limb> limbs = new List<Limb>()
            {
                new Limb(TypeOfLimb.Head,10, 10, 6, $"{actor.Name}'s Head", Limb.LimbOrientation.Center),
                new Limb(TypeOfLimb.Torso,15, 15, 8.47, $"{actor.Name}'s Torso", Limb.LimbOrientation.Center ),
                new Limb(TypeOfLimb.Arm,7, 7, 4, $"{actor.Name}'s Left Arm", Limb.LimbOrientation.Left),
                new Limb(TypeOfLimb.Arm, 7, 7, 4, $"{actor.Name}'s Right Arm", Limb.LimbOrientation.Right),
                new Limb(TypeOfLimb.Leg, 7, 7, 6, $"{actor.Name}'s Right Leg", Limb.LimbOrientation.Right),
                new Limb(TypeOfLimb.Leg,7, 7, 6, $"{actor.Name}'s Left Leg", Limb.LimbOrientation.Left),
                new Limb(TypeOfLimb.Hand,4, 7, 6, $"{actor.Name}'s Left Hand", Limb.LimbOrientation.Left),
                new Limb(TypeOfLimb.Hand,4, 7, 6, $"{actor.Name}'s Right Hand", Limb.LimbOrientation.Right),
                new Limb(TypeOfLimb.Foot,4, 7, 6, $"{actor.Name}'s Right Foot", Limb.LimbOrientation.Right),
                new Limb(TypeOfLimb.Foot,4, 7, 6, $"{actor.Name}'s Left Foot", Limb.LimbOrientation.Left),
            };

            return limbs;
        }
    }
}