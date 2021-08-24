using MagiRogue.Entities;
using SadRogue.Primitives;

namespace MagiRogue.Data
{
    /// <summary>
    /// Utility method for easy creating an actor and item, along with any possible function and extra.
    /// </summary>
    public static class EntityFactory
    {
        public static Actor ActorCreator(Point position, ActorTemplate actorTemplate)
        {
            Actor actor =
                new Actor(
                actorTemplate.Name,
                actorTemplate.Foreground,
                actorTemplate.Background,
                actorTemplate.Glyph,
                position
                )
                {
                    Stats = actorTemplate.Stats,
                };
            actor.Description = actorTemplate.Description;
            actor.Material = System.Physics.PhysicsManager.SetMaterial(actorTemplate.MaterialId);

            return actor;
        }

        public static Item ItemCreator(Point position, ItemTemplate itemTemplate)
        {
            Item item =
                new Item
                (
                    itemTemplate.Foreground,
                    itemTemplate.Background,
                    itemTemplate.Name,
                    itemTemplate.Glyph,
                    position,
                    itemTemplate.Size,
                    itemTemplate.Weight,
                    itemTemplate.Condition
                )
                {
                    Description = itemTemplate.Description,
                    Material = System.Physics.PhysicsManager.SetMaterial(itemTemplate.MaterialId)
                };

            return item;
        }
    }
}