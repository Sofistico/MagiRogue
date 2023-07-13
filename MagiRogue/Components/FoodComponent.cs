using MagiRogue.Data.Enumerators;

namespace MagiRogue.Components
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
