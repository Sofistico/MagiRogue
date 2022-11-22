using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Physics;

namespace MagiRogue.Entities
{
    public sealed class Tissue
    {
        public string Name { get; set; }
        public MaterialTemplate Material { get; private set; }
        public string MaterialId { get; set; }

        /// <summary>
        /// the thickness of the layer of the Tissue, should be in cm
        /// </summary>
        public int Thickness { get; set; }

        public Tissue(string name, string materialId)
        {
            Name = name;
            MaterialId = materialId;
            Material = PhysicsManager.SetMaterial(materialId);
        }
    }
}