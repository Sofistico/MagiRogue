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
        public string? ConnectedTo
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

        public bool Broken { get; set; } = false;

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

        public Limb(TypeOfLimb limbType, double limbHp, double maxLimbHp, string limbName,
            BodyPartOrientation orientation, string connectedTo,
            string materialID = "flesh", BodyPartFunction limbFunction = BodyPartFunction.Limb)
        {
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
            LimbFunction = limbFunction;
        }

        public Limb(string id, TypeOfLimb limbType, int limbHp, int maxLimbHp, string limbName, BodyPartOrientation orientation, string? connectedTo,
           string materialID = "flesh")
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
            int size = Size;
            Attached = false;

            return new Item(actor.Appearance.Foreground,
                actor.Appearance.Background,
                limbName,
                253,
                actor.Position,
                size,
                (float)BodyPartWeight,
                materialId: MaterialId
                );
        }
    }
}