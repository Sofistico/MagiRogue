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

        public Organ(string name,
            string? connectedTo,
            BodyPartOrientation orientation,
            OrganType organType,
            string materialId) : base(materialId)
        {
            BodyPartName = name;
            InsideOf = connectedTo;
            Orientation = orientation;
            OrganType = organType;
        }

        [JsonConstructor()]
        public Organ(string id,
           string name,
           string? connectedTo,
           BodyPartOrientation orientation,
           OrganType organType,
           string materialId) : base(materialId)
        {
            Id = id;
            BodyPartName = name;
            InsideOf = connectedTo;
            Orientation = orientation;
            OrganType = organType;
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
                MaterialId = this.MaterialId,
                BodyPartMaterial = this.BodyPartMaterial,
                OrganType = this.OrganType,
                BodyPartFunction = this.BodyPartFunction,
                CanHeal = this.CanHeal,
                RateOfHeal = this.RateOfHeal,
                RelativeVolume = this.RelativeVolume,
                Tissues = this.Tissues,
                Volume = this.Volume,
                Working = this.Working,
                Wounds = this.Wounds,
                Insides = this.Insides
            };

            return copy;
        }

        public override void AddWound(Wound wound)
        {
            base.AddWound(wound);
            switch (wound.Severity)
            {
                case InjurySeverity.Inhibited:
                case InjurySeverity.Broken:
                    Working = false;
                    break;

                case InjurySeverity.Pulped:
                    Working = true;
                    break;
            }
        }
    }
}