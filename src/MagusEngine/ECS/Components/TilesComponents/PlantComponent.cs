using MagusEngine.Core.Entities;

namespace MagusEngine.ECS.Components.TilesComponents
{
    // should be flyweight, where the plant class data is shared between all plants of the same type and the PlantComponent is the only thing that is unique
    public class PlantComponent
    {
        public Plant Plant { get; set; }
        public int Bundle { get; set; }

        public PlantComponent(Plant plant)
        {
            Plant = plant;
            Bundle = plant.MaxBundle;
        }
    }
}
