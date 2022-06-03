using MagiRogue.Entities;
using SadRogue.Primitives;
using MagiRogue.Data.Serialization;
using System.Collections.Generic;
using System;
using MagiRogue.Data.Serialization.EntitySerialization;

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
            actor.Material = GameSys.Physics.PhysicsManager.SetMaterial(actorTemplate.MaterialId);
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

        public static List<Limb> BasicHumanoidBody()
        {
            Limb torso = DataManager.QueryLimbInData("humanoid_torso");
            Limb neck = DataManager.QueryLimbInData("humanoid_neck");
            Limb head = DataManager.QueryLimbInData("humanoid_head");
            Limb lArm = DataManager.QueryLimbInData("humanoid_l_arm");
            Limb rArm = DataManager.QueryLimbInData("humanoid_r_arm");
            Limb rLeg = DataManager.QueryLimbInData("humanoid_r_leg");
            Limb lLeg = DataManager.QueryLimbInData("humanoid_l_leg");
            Limb lHand = DataManager.QueryLimbInData("humanoid_l_hand");
            Limb rHand = DataManager.QueryLimbInData("humanoid_r_hand");
            Limb rFoot = DataManager.QueryLimbInData("humanoid_r_foot");
            Limb lFoot = DataManager.QueryLimbInData("humanoid_l_foot");

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

            return limbs;
        }

        public static List<Organ> BasicHumanoidOrgans()
        {
            Organ brain = DataManager.QueryOrganInData("brain");
            Organ heart = DataManager.QueryOrganInData("humanoid_heart");
            Organ spine = DataManager.QueryOrganInData("humanoid_spine");
            Organ stomach = DataManager.QueryOrganInData("humanoid_stomach");
            Organ intestine = DataManager.QueryOrganInData("humanoid_intestine");
            Organ r_lung = DataManager.QueryOrganInData("humanoid_r_lung");
            Organ l_lung = DataManager.QueryOrganInData("humanoid_l_lung");
            Organ r_kidney = DataManager.QueryOrganInData("humanoid_r_kidney");
            Organ l_kidney = DataManager.QueryOrganInData("humanoid_l_kidney");
            Organ r_eye = DataManager.QueryOrganInData("humanoid_r_eye");
            Organ l_eye = DataManager.QueryOrganInData("humanoid_l_eye");

            List<Organ> list = new()
            {
                brain,
                heart,
                spine,
                stomach,
                intestine,
                r_lung,
                l_lung,
                r_kidney,
                l_kidney,
                r_eye,
                l_eye
            };

            return list;
        }
    }
}