using GoRogue;

namespace MagiRogue.Entities.Data
{
    /// <summary>
    /// Utility method for easy creating an actor and item, along with any possible function and extra.
    /// </summary>
    public static class EntityFactory
    {
        public static Actor ActorCreator(Coord position, ActorTemplate actorTemplate)
        {
            Actor actor =
                new Actor(
                actorTemplate.Foreground,
                actorTemplate.Background,
                actorTemplate.Glyph,
                position
                )
                {
                    Stats = actorTemplate.Stats,
                    Anatomy = actorTemplate.Anatomy
                };
            actor.Description = actorTemplate.Description;

            return actor;
        }

        public static Item ItemCreator(Coord position, ItemTemplate itemTemplate)
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
                );
            item.Description = itemTemplate.Description;

            return item;
        }
    }
}