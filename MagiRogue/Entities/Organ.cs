using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Physics;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    public sealed class Organ : BodyPart
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
            string materialId) : base(materialId)
        {
            BodyPartName = name;
            InsideOf = connectedTo;
            Orientation = orientation;
            OrganType = organType;
            BodyPartHp = organHp;
            MaxBodyPartHp = organHp;
        }

        [JsonConstructor()]
        public Organ(string id,
           string name,
           string? connectedTo,
           BodyPartOrientation orientation,
           OrganType organType,
           int organHp,
           string materialId) : base(materialId)
        {
            Id = id;
            BodyPartName = name;
            InsideOf = connectedTo;
            Orientation = orientation;
            OrganType = organType;
            BodyPartHp = organHp;
            MaxBodyPartHp = organHp;
        }

        public Organ(string materialId = "skin") : base(materialId)
        {
            // Empty!
        }

        public override Organ Copy()
        {
            Organ copy = new Organ()
            {
                Id = this.Id,
                BodyPartName = this.BodyPartName,
                InsideOf = this.InsideOf,
                Orientation = this.Orientation,
                BodyPartHp = this.BodyPartHp,
                MaterialId = this.MaterialId,
                BodyPartMaterial = this.BodyPartMaterial,
                MaxBodyPartHp = this.MaxBodyPartHp,
                OrganType = this.OrganType
            };

            return copy;
        }

        public override void CalculateWound(Wound wound)
        {
            base.CalculateWound(wound);
            switch (wound.Severity)
            {
                case InjurySeverity.Inhibited:
                    Working = false;
                    break;

                case InjurySeverity.Broken:
                    Working = false;
                    break;

                case InjurySeverity.Pulped:
                    Working = true;
                    break;

                default:
                    break;
            }
        }
    }
}