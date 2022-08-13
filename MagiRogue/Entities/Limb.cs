using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    [DataContract]
    [JsonConverter(typeof(Data.Serialization.EntitySerialization.LimbJsonConverter))]
    public class Limb : BodyPart
    {
        private string _connectedLimb;

        [DataMember]
        public TypeOfLimb TypeLimb { get; set; }

        [DataMember]
        public string ConnectedTo
        {
            get
            {
                return _connectedLimb;
            }

            set
            {
                _connectedLimb = value;
            }
        }

        public bool Broken { get; set; } = false;

        public bool Attached { get; set; } = true;

        /// <summary>
        /// This class creates a limb for a body.
        /// </summary>
        /// <param name="limbType">The type of the limb, if its a arm or a leg or etc...</param>
        /// <param name="limbHp">the total hp of the limb</param>
        /// <param name="maxLimbHp">the max limb hp that it can recover</param>
        /// <param name="limbName">The name of the limb</param>
        /// <param name="orientation">If it's in the center, left or right of the body</param>
        /// <param name="materialID">The id to define the material, if needeed look at the material definition json\n
        /// Defaults to "flesh"</param>

        public Limb(TypeOfLimb limbType, string limbName,
            BodyPartOrientation orientation, string connectedTo,
            string materialID = "flesh", BodyPartFunction limbFunction = BodyPartFunction.Limb) : base()
        {
            MaterialId = materialID;
            BodyPartMaterial = GameSys.Physics.PhysicsManager.SetMaterial(materialID);
            TypeLimb = limbType;
            Attached = true;
            BodyPartName = limbName;
            Orientation = orientation;
            ConnectedTo = connectedTo;
            LimbFunction = limbFunction;
        }

        public Limb(string id, TypeOfLimb limbType, int limbHp, int maxLimbHp,
            string limbName, BodyPartOrientation orientation, string connectedTo,
           string materialID = "flesh") : base()
        {
            Id = id;
            MaterialId = materialID;
            BodyPartMaterial = GameSys.Physics.PhysicsManager.SetMaterial(materialID);
            TypeLimb = limbType;
            MaxBodyPartHp = maxLimbHp;
            BodyPartHp = limbHp;
            //BodyPartWeight = limbWeight;
            // Defaults to true
            Attached = true;
            BodyPartName = limbName;
            Orientation = orientation;
            ConnectedTo = connectedTo;
        }

        public Item ReturnLimbAsItem(Actor actor)
        {
            string limbName = actor.Name + "'s " + BodyPartName;
            int size = Volume;
            //Attached = false;

            return new Item(actor.Appearance.Foreground,
                actor.Appearance.Background,
                limbName,
                253,
                actor.Position,
                size,
                materialId: MaterialId
                );
        }

        public override void CalculateWound(Wound wound)
        {
            base.CalculateWound(wound);
            switch (wound.Severity)
            {
                case InjurySeverity.Broken:
                    Broken = true;
                    break;

                case InjurySeverity.Missing:
                    Attached = false;
                    break;

                case InjurySeverity.Pulped:
                    Broken = true;
                    break;

                default:
                    break;
            }
        }
    }
}