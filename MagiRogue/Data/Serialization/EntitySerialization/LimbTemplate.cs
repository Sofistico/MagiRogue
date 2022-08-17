using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class LimbJsonConverter : JsonConverter<Limb>
    {
        public override Limb? ReadJson(JsonReader reader,
            Type objectType, Limb? existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            serializer.NullValueHandling = NullValueHandling.Ignore;
            return serializer.Deserialize<LimbTemplate>(reader);
        }

        public override void WriteJson(JsonWriter writer,
            Limb? value, JsonSerializer serializer)
        {
            serializer.NullValueHandling = NullValueHandling.Ignore;

            LimbTemplate template = (LimbTemplate)value;
            serializer.Serialize(writer, template);
        }
    }

    [DataContract]
    [Serializable]
    public class LimbTemplate
    {
        [DataMember]
        public TypeOfLimb LimbType { get; set; }

        [DataMember]
        public double MaxLimbHp { get; set; }

        [DataMember]
        public double LimbHp { get; set; }

        [DataMember]
        public double LimbWeight { get; set; }

        [DataMember]
        public string LimbName { get; set; }

        [DataMember]
        public BodyPartOrientation LimbOrientation { get; set; }

        [DataMember]
        public string LimbMaterialId { get; set; }

        [DataMember]
        public bool Attached { get; private set; }

        [DataMember]
        public string? ConnectedToId { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public bool Broken { get; set; } = false;

        [DataMember]
        public BodyPartFunction BodyPartFunction { get; set; }

        [DataMember]
        public double RateOfHeal { get; set; }

        [DataMember]
        public double RelativeSize { get; set; }

        // Here will be a class designed to reliable build different types of anatomys, for use in a future dictionary
        // static class for anatomies

        public LimbTemplate()
        {
        }

        public LimbTemplate(TypeOfLimb limbType,
            int maxLimbHp,
            double limbHp,
            double limbWeight,
            string limbName,
            BodyPartOrientation limbOrientation,
            string limbMaterialId,
            bool attached,
            string? connectsTo)
        {
            LimbType = limbType;
            MaxLimbHp = maxLimbHp;
            LimbHp = limbHp;
            LimbWeight = limbWeight;
            LimbName = limbName;
            LimbOrientation = limbOrientation;
            LimbMaterialId = limbMaterialId;
            Attached = attached;
            ConnectedToId = connectsTo;
        }

        public LimbTemplate Copy()
        {
            LimbTemplate copy = new LimbTemplate()
            {
                Attached = this.Attached,
                Broken = this.Broken,
                ConnectedToId = this.ConnectedToId,
                Id = this.Id,
                LimbHp = this.LimbHp,
                LimbMaterialId = this.LimbMaterialId,
                LimbName = this.LimbName,
                LimbOrientation = this.LimbOrientation,
                LimbType = this.LimbType,
                LimbWeight = this.LimbWeight,
                MaxLimbHp = this.MaxLimbHp,
                BodyPartFunction = this.BodyPartFunction,
                RateOfHeal = this.RateOfHeal,
                RelativeSize = this.RelativeSize,
            };

            return copy;
        }

        public static implicit operator Limb(LimbTemplate template)
        {
            if (template is null)
                return null;
            Limb limb = new Limb(template.LimbType,
                template.LimbName,
                template.LimbOrientation,
                template.ConnectedToId,
                template.LimbMaterialId,
                template.BodyPartFunction
                );
            limb.Id = template.Id;
            limb.BodyPartHp = template.LimbHp;
            limb.MaxBodyPartHp = (int)template.MaxLimbHp;
            limb.Broken = template.Broken;
            limb.RateOfHeal = template.RateOfHeal;
            limb.RelativeVolume = template.RelativeSize;

            return limb;
        }

        public static implicit operator LimbTemplate(Limb limb)
        {
            LimbTemplate template = new LimbTemplate(limb.TypeLimb,
                limb.MaxBodyPartHp,
                limb.BodyPartHp,
                limb.BodyPartWeight,
                limb.BodyPartName,
                limb.Orientation,
                limb.MaterialId,
                limb.Attached,
                limb.ConnectedTo);
            template.Id = limb.Id;
            template.Broken = limb.Broken;
            template.BodyPartFunction = limb.BodyPartFunction;
            template.RateOfHeal = limb.RateOfHeal;

            return template;
        }
    }
}