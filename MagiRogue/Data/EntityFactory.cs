using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using System.Collections.Generic;

namespace MagiRogue.Data
{
    /// <summary>
    /// Utility method for easy creating an actor and item, along with any possible function and extra.
    /// </summary>
    public static class EntityFactory
    {
        public static Actor ActorCreator(Point position, string raceId)
        {
            Race race = DataManager.QueryRaceInData(raceId);
            Actor actor = null;
            //Actor actor =
            //    new(
            //    actorTemplate.Name,
            //    actorTemplate.ForegroundBackingField.Color,
            //    actorTemplate.BackgroundBackingField.Color,
            //    actorTemplate.Glyph,
            //    position
            //    )
            //    {
            //        Description = actorTemplate.Description,
            //        Soul = actorTemplate.Soul,
            //        Mind = actorTemplate.Mind
            //    };
            //if (actorTemplate.Abilities is not null && actorTemplate.Abilities.Count > 0)
            //{
            //    for (int i = 0; i < actorTemplate.Abilities.Count; i++)
            //    {
            //        var ability = actorTemplate.Abilities[i];
            //        actor.Mind.AddAbilityToDictionary(ability);
            //    }
            //}

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
                    itemTemplate.Volume,
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
            Limb upperBody = DataManager.QueryLimbInData("humanoid_upper_body");
            Limb lowerBody = DataManager.QueryLimbInData("humanoid_lower_body");
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
            // temporary
            List<Limb> fingerAndToes = new List<Limb>();
            for (int i = 1; i < 10 + 1; i++)
            {
                Limb rFinger = new Limb(Enumerators.TypeOfLimb.Finger, 1, 1, $"{i} Right Finger",
                    Enumerators.BodyPartOrientation.Right, "humanoid_r_hand");
                Limb lFinger = new Limb(Enumerators.TypeOfLimb.Finger, 1, 1, $"{i} Left Finger",
                    Enumerators.BodyPartOrientation.Left, "humanoid_l_hand");
                Limb rToes = new Limb(Enumerators.TypeOfLimb.Toe, 1, 1, $"{i} Right Toe",
                    Enumerators.BodyPartOrientation.Right, "humanoid_r_foot");
                Limb lToes = new Limb(Enumerators.TypeOfLimb.Toe, 1, 1, $"{i} Left Toe",
                    Enumerators.BodyPartOrientation.Left, "humanoid_l_foot");
                fingerAndToes.Add(rFinger);
                fingerAndToes.Add(lFinger);
                fingerAndToes.Add(rToes);
                fingerAndToes.Add(lToes);
            }

            List<Limb> limbs = new()
            {
                upperBody,
                lowerBody,
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
            limbs.AddRange(fingerAndToes);

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