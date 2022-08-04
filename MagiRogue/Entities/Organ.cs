using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Physics;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    public class Organ : BodyPart
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? InsideOf { get; set; }

        [DataMember]
        public OrganType OrganType { get; set; }

        [DataMember]
        public bool Destroyed { get => BodyPartHp <= 0; }

        public Organ(string name,
            string? connectedTo,
            BodyPartOrientation orientation,
            OrganType organType,
            int organHp,
            string materialId)
        {
            BodyPartName = name;
            InsideOf = connectedTo;
            Orientation = orientation;
            OrganType = organType;
            BodyPartHp = organHp;
            MaterialId = materialId;
            BodyPartMaterial = PhysicsManager.SetMaterial(materialId);
            MaxBodyPartHp = organHp;
            //BodyPartWeight = organWeight;
        }

        [JsonConstructor()]
        public Organ(string id,
           string name,
           string? connectedTo,
           BodyPartOrientation orientation,
           OrganType organType,
           int organHp,
           string materialId)
        {
            Id = id;
            BodyPartName = name;
            InsideOf = connectedTo;
            Orientation = orientation;
            OrganType = organType;
            BodyPartHp = organHp;
            MaterialId = materialId;
            BodyPartMaterial = PhysicsManager.SetMaterial(materialId);
            MaxBodyPartHp = organHp;
        }

        public Organ()
        {
            // Empty!
        }

        public Organ Copy()
        {
            Organ copy = new Organ()
            {
                Attached = this.Attached,
                Id = this.Id,
                BodyPartName = this.BodyPartName,
                InsideOf = this.InsideOf,
                Orientation = this.Orientation,
                BodyPartHp = this.BodyPartHp,
                MaterialId = this.MaterialId,
                BodyPartMaterial = this.BodyPartMaterial,
                MaxBodyPartHp = this.MaxBodyPartHp,
                //BodyPartWeight = this.BodyPartWeight,
                OrganType = this.OrganType
            };

            return copy;
        }
    }
}