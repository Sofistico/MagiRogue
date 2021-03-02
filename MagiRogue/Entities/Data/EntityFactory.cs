using GoRogue;

namespace MagiRogue.Entities.Data
{
    public static class EntityFactory
    {
        public static Actor ActorCreator(Coord position, ActorTemplate actorTemplate)
        {
            Actor actor =
                new Actor(
                actorTemplate.Foreground,
                actorTemplate.Background,
                actorTemplate.Glyph,
                (int)System.MapLayer.ACTORS,
                position
                )
                {
                    Stats = actorTemplate.Stats,
                    Anatomy = actorTemplate.Anatomy
                };

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

            return item;
        }
    }
}