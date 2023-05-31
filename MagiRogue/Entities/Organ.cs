using MagiRogue.Data.Enumerators;
using Newtonsoft.Json;

namespace MagiRogue.Entities
{
    public sealed class Organ : BodyPart
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? InsideOf { get; set; }

        public OrganType OrganType { get; set; }

        public bool Embedded { get; set; }

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
           string materialId,
           bool embedded = false) : base(materialId)
        {
            Id = id;
            BodyPartName = name;
            InsideOf = connectedTo;
            Orientation = orientation;
            OrganType = organType;
            Embedded = embedded;
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
                Insides = this.Insides,
                Embedded = this.Embedded,
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
                case InjurySeverity.Pulped:
                    Working = false;
                    break;
            }
        }
    }
}