using MagiRogue.Data.Enumerators;
using MagiRogue.Entities.Core;
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
            OrganType organType) : base()
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
           bool embedded = false) : base()
        {
            Id = id;
            BodyPartName = name;
            InsideOf = connectedTo;
            Orientation = orientation;
            OrganType = organType;
            Embedded = embedded;
        }

        public Organ() : base()
        {
            // Empty!
        }

        public override Organ Copy()
        {
            return new Organ()
            {
                Id = this.Id,
                BodyPartName = this.BodyPartName,
                InsideOf = this.InsideOf,
                Orientation = this.Orientation,
                OrganType = this.OrganType,
                BodyPartFunction = this.BodyPartFunction,
                RelativeVolume = this.RelativeVolume,
                Tissues = new(Tissues),
                Volume = this.Volume,
                Working = this.Working,
                Wounds = new(Wounds),
                Insides = new(Insides),
                Embedded = this.Embedded,
                Category = Category,
            };
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