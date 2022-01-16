using MagiRogue.Data.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeOfLimb
    {
        Head,
        Torso,
        Arm,
        Leg,
        Foot,
        Hand,
        Tail,
        Wing,
        Neck
    }

    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Limb
    {
        private int limbHp;
        private double weight;
        private Limb _connectedLimb;

        [DataMember]
        public int LimbHp
        {
            get
            {
                if (limbHp > MaxLimbHp)
                {
                    return MaxLimbHp;
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
                else
                    limbHp = value;
            }
        }

        [DataMember]
        public int MaxLimbHp { get; set; }

        [DataMember]
        public double LimbWeight
        {
            get { return weight; }

            set
            {
                weight = value * LimbMaterial.Density;
            }
        }

        [DataMember]
        public bool Attached { get; set; }

        /// <summary>
        /// Marks if the limb is right, left, or center.
        /// </summary>
        [DataContract]
        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public enum LimbOrientation
        { Right, Left, Center }

        /// <summary>
        /// Marks if the limb is right, left, or center, this is the property.
        /// </summary>
        [DataMember]
        public LimbOrientation Orientation { get; set; }

        [DataMember]
        public string LimbName { get; set; }

        [DataMember]
        public TypeOfLimb TypeLimb { get; set; }

        [DataMember]
        public MaterialTemplate LimbMaterial { get; set; }

        [DataMember]
        public Limb? ConnectedTo
#nullable disable
        {
            get
            {
                if (Attached)
                    return _connectedLimb;
                else
                    return null;
            }

            set
            {
                _connectedLimb = value;
            }
        }

        /// <summary>
        /// This class creates a limb for a body.
        /// </summary>
        /// <param name="limbType">The type of the limb, if its a arm or a leg or etc...</param>
        /// <param name="limbHp">the total hp of the limb</param>
        /// <param name="maxLimbHp">the max limb hp that it can recover</param>
        /// <param name="limbWeight">The total weight of the limb</param>
        /// <param name="limbName">The name of the limb</param>
        /// <param name="orientation">If it's in the center, left or right of the body</param>
        /// <param name="materialID">The id to define the material, if needeed look at the material definition json\n
        /// Defaults to "flesh"</param>
#nullable enable

        public Limb(TypeOfLimb limbType, int limbHp, int maxLimbHp,
            double limbWeight, string limbName, LimbOrientation orientation, Limb? connectedTo,
            string materialID = "flesh")
#nullable disable
        {
            LimbMaterial = System.Physics.PhysicsManager.SetMaterial(materialID);
            TypeLimb = limbType;
            MaxLimbHp = maxLimbHp;
            LimbHp = limbHp;
            LimbWeight = limbWeight;
            // Defaults to true
            Attached = true;
            LimbName = limbName;
            Orientation = orientation;
            ConnectedTo = connectedTo;
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"{nameof(Limb)} : {LimbName}, {TypeLimb}");
            }
        }
    }
}