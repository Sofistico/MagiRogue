using MagiRogue.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Serialization
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
        public int MaxLimbHp { get; set; }

        [DataMember]
        public int LimbHp { get; set; }

        [DataMember]
        public double LimbWeight { get; set; }

        [DataMember]
        public string LimbName { get; set; }

        [DataMember]
        public Limb.LimbOrientation LimbOrientation { get; set; }

        [DataMember]
        public string LimbMaterialId { get; set; }

        [DataMember]
        public bool Attached { get; private set; }

        [DataMember]
        public string? ConnectedToId { get; private set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public bool Broken { get; set; } = false;

        // Here will be a class designed to reliable build different types of anatomys, for use in a future dictionary
        // static class for anatomies

        public LimbTemplate()
        {
        }

        [JsonConstructor()]
        public LimbTemplate(TypeOfLimb limbType,
            int maxLimbHp,
            int limbHp,
            double limbWeight,
            string limbName,
            Limb.LimbOrientation limbOrientation,
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

        public static implicit operator Limb(LimbTemplate template)
        {
            Limb limb = new Limb(template.LimbType,
                template.LimbHp,
                template.MaxLimbHp,
                template.LimbWeight,
                template.LimbName,
                template.LimbOrientation,
                template.ConnectedToId,
                template.LimbMaterialId
                );
            limb.Id = template.Id;
            limb.Broken = template.Broken;

            return limb;
        }

        public static implicit operator LimbTemplate(Limb limb)
        {
            LimbTemplate template = new LimbTemplate(limb.TypeLimb,
                limb.MaxLimbHp,
                limb.LimbHp,
                limb.LimbWeight,
                limb.LimbName,
                limb.Orientation,
                limb.LimbMaterial.Id,
                limb.Attached,
                limb.ConnectedTo);
            template.Id = limb.Id;
            template.Broken = limb.Broken;

            return template;
        }
    }
}