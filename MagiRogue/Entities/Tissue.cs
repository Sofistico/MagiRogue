using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Physics;

namespace MagiRogue.Entities
{
    public class Tissue
    {
        public string Name { get; set; }
        public MaterialTemplate Material { get; private set; }
        public string MaterialId { get; set; }

        public Tissue(string name, string materialId)
        {
            Name = name;
            MaterialId = materialId;
            Material = PhysicsManager.SetMaterial(materialId);
        }
    }
}