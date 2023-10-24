using Arquimedes.Enumerators;

namespace MagusEngine.ECS.Components.ActorComponents
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
