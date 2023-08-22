using MagusEngine.Serialization;
using MagusEngine.Systems.Physics;

namespace MagusEngine.ECS.Components
{
    public class MaterialComponent
    {
        public string MaterialId { get; set; }
        public MaterialTemplate Material { get; set; }

        public MaterialComponent(string materialId)
        {
            MaterialId = materialId;
            Material = PhysicsManager.SetMaterial(materialId);
        }
    }
}
