using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using Newtonsoft.Json;

namespace MagusEngine.Core.Entities
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
                Id = Id,
                BodyPartName = BodyPartName,
                InsideOf = InsideOf,
                Orientation = Orientation,
                OrganType = OrganType,
                BodyPartFunction = BodyPartFunction,
                RelativeVolume = RelativeVolume,
                Tissues = new(Tissues),
                Volume = Volume,
                Working = Working,
                Wounds = new(Wounds),
                Insides = new(Insides),
                Embedded = Embedded,
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