using MagiRogue.Data.Serialization;
using MagiRogue.System.Physics;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    public class Organ
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? ConnectedTo { get; set; }

        [DataMember]
        public Limb.LimbOrientation Orientation { get; set; }

        [DataMember]
        public OrganType OrganType { get; set; }

        [JsonProperty(Required = Required.Default)]
        public bool Attached { get; set; } = true;

        [DataMember]
        public bool Destroyed { get => OrganHp <= 0; }

        [DataMember]
        public int OrganHp { get; set; }

        [JsonIgnore]
        public MaterialTemplate Material { get; set; }

        [DataMember]
        public string MaterialId { get; set; }

        [DataMember]
        public int MaxOrganHp { get; set; }

        [DataMember]
        public float OrganWeight { get; set; }

        public Organ(string name,
            string? connectedTo,
            Limb.LimbOrientation orientation,
            OrganType organType,
            int organHp,
            string materialId,
            float organWeight)
        {
            Name = name;
            ConnectedTo = connectedTo;
            Orientation = orientation;
            OrganType = organType;
            OrganHp = organHp;
            MaterialId = materialId;
            Material = PhysicsManager.SetMaterial(materialId);
            MaxOrganHp = organHp;
            OrganWeight = organWeight;
        }

        [JsonConstructor()]
        public Organ(string id,
           string name,
           string? connectedTo,
           Limb.LimbOrientation orientation,
           OrganType organType,
           int organHp,
           string materialId,
           float organWeight)
        {
            Id = id;
            Name = name;
            ConnectedTo = connectedTo;
            Orientation = orientation;
            OrganType = organType;
            OrganHp = organHp;
            MaterialId = materialId;
            Material = PhysicsManager.SetMaterial(materialId);
            MaxOrganHp = organHp;
            OrganWeight = organWeight;
        }

        public Organ()
        {
            // Empty!
        }
    }

    public enum OrganType
    {
        Misc,
        Heart,
        Brain,
        Digestive,
        Breather,
        Circulatory,
        Protective,
        Visual,
        Auditory,
        Nerve
    }
}