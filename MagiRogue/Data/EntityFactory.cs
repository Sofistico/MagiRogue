﻿using MagiRogue.Entities;
using SadRogue.Primitives;
using MagiRogue.Data.Serialization;

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
                new(
                actorTemplate.Name,
                actorTemplate.ForegroundBackingField.Color,
                actorTemplate.BackgroundBackingField.Color,
                actorTemplate.Glyph,
                position
                )
                {
                    Stats = actorTemplate.Stats,
                };
            actor.Description = actorTemplate.Description;
            actor.Material = System.Physics.PhysicsManager.SetMaterial(actorTemplate.MaterialId);
            if (actorTemplate.Abilities is not null && actorTemplate.Abilities.Count > 0)
            {
                for (int i = 0; i < actorTemplate.Abilities.Count; i++)
                {
                    var ability = actorTemplate.Abilities[i];
                    actor.AddAbilityToDictionary(ability);
                }
            }

            return actor;
        }

        public static Item ItemCreator(Point position, ItemTemplate itemTemplate)
        {
            Item item =
                new
                (
                    itemTemplate.ForegroundBackingField.Color,
                    itemTemplate.BackgroundBackingField.Color,
                    itemTemplate.Name,
                    itemTemplate.Glyph,
                    position,
                    itemTemplate.Size,
                    itemTemplate.Weight,
                    itemTemplate.Condition
                )
                {
                    Description = itemTemplate.Description,
                    Material = itemTemplate.Material
                };

            return item;
        }
    }
}