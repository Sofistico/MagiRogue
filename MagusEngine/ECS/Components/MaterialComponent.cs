using MagusEngine.Serialization;

namespace MagusEngine.ECS.Components
{
    public class MaterialComponent
    {
        public string MaterialId { get; set; }
        public MaterialTemplate Material { get; set; }

        public MaterialComponent(string materialId, MaterialTemplate material)
        {
            MaterialId = materialId;
            Material = material;
        }
    }
}
