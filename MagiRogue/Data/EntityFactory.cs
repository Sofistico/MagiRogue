using MagiRogue.Entities;
using SadRogue.Primitives;
using MagiRogue.Data.Serialization;
using System.Collections.Generic;
using System;

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

        public static List<Limb> BasicHumanoidBody(Actor actor)
        {
            /*var torso = new Limb(
                TypeOfLimb.Torso, 15, 15, 8.47, $"{actor.Name}'s Torso", Limb.LimbOrientation.Center, null);
            var neck = new
                Limb(TypeOfLimb.Neck, 5, 5, 5, $"{actor.Name}'s Neck", Limb.LimbOrientation.Center, torso);
            var head = new
                Limb(TypeOfLimb.Head, 10, 10, 6, $"{actor.Name}'s Head", Limb.LimbOrientation.Center, neck);
            var lArm = new
                Limb(TypeOfLimb.Arm, 7, 7, 4, $"{actor.Name}'s Left Arm", Limb.LimbOrientation.Left, torso);
            var rArm = new
                Limb(TypeOfLimb.Arm, 7, 7, 4, $"{actor.Name}'s Right Arm", Limb.LimbOrientation.Right, torso);
            var rLeg =
                new Limb(TypeOfLimb.Leg, 7, 7, 6, $"{actor.Name}'s Right Leg", Limb.LimbOrientation.Right, torso);
            var lLeg =
                new Limb(TypeOfLimb.Leg, 7, 7, 6, $"{actor.Name}'s Left Leg", Limb.LimbOrientation.Left, torso);
            var lHand =
                new Limb(TypeOfLimb.Hand, 4, 7, 6, $"{actor.Name}'s Left Hand", Limb.LimbOrientation.Left, lArm);
            var rHand = new
                Limb(TypeOfLimb.Hand, 4, 7, 6, $"{actor.Name}'s Right Hand", Limb.LimbOrientation.Right, rArm);
            var rFoot = new
                Limb(TypeOfLimb.Foot, 4, 7, 6, $"{actor.Name}'s Right Foot", Limb.LimbOrientation.Right, rLeg);
            var lFoot = new
                Limb(TypeOfLimb.Foot, 4, 7, 6, $"{actor.Name}'s Left Foot", Limb.LimbOrientation.Left, lLeg);

            List<Limb> limbs = new()
            {
                torso,
                neck,
                head,
                lArm,
                rArm,
                rLeg,
                lLeg,
                lHand,
                rHand,
                rFoot,
                lFoot
            };

            return limbs;*/
            throw new NotImplementedException("WORK HARD SLAVE!");
        }
    }
}