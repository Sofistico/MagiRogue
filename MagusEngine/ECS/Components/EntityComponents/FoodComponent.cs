using Arquimedes.Enumerators;

namespace MagusEngine.ECS.Components.EntityComponents
{
    public sealed class FoodComponent
    {
        public static FoodComponent None => new(Food.None);
        public static FoodComponent Herbivore => new(Food.Herbivore);
        public static FoodComponent Carnivore => new(Food.Carnivore);
        public static FoodComponent Omnivere => new(Food.Omnivere);

        public Food FoodType { get; set; }

        private FoodComponent(Food foodType)
        {
            FoodType = foodType;
        }
    }
}
