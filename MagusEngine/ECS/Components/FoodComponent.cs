using MagiRogue.Data.Enumerators;

namespace MagusEngine.ECS.Components
{
    public class FoodComponent
    {
        public Food FoodType { get; set; }

        public FoodComponent(Food foodType)
        {
            FoodType = foodType;
        }
    }
}
