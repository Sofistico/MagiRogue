using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.Entities.StarterScenarios;
using System;
using System.Collections.Generic;

namespace MagiRogue.Data
{
    /// <summary>
    /// Utility method for easy creating an actor and item, along with any possible function and extra.
    /// </summary>
    public static class EntityFactory
    {
        public static Actor ActorCreatorFirstStep(Point position, string raceId,
            string actorName, int actorAge, Gender gender)
        {
            Race race = DataManager.QueryRaceInData(raceId);
            Actor actor = new Actor(actorName,
                race.ReturnForegroundColor(),
                race.ReturnBackgroundColor(),
                race.RaceGlyph,
                position)
            {
                Description = race.Description,
            };
            SetupBodySoulAndMind(race, actor, actorAge, gender);

            return actor;
        }

        public static Player ActorCreatorPlayerCreator(Actor actor, string scenarioId, Gender gender)
        {
            Scenario scenario = DataManager.QueryScenarioInData(scenarioId);
            actor.GetAnatomy().Gender = gender;
            SetupAbilities(actor, scenario.Abilities);
            Player player = Player.ReturnPlayerFromActor(actor);

            return player;
        }

        private static void SetupAbilities(Actor actor, List<AbilityTemplate> abilities)
        {
            foreach (Ability item in abilities)
            {
                actor.Mind.AddAbilityToDictionary(item);
            }
        }

        private static void SetupBodySoulAndMind(Race race, Actor actor, int actorAge, Gender gender)
        {
            Body body = actor.Body;
            Anatomy anatomy = actor.GetAnatomy();
            Mind mind = actor.Mind;
            Soul soul = actor.Soul;

            int volume = race.GetRngVolume(actorAge);

            anatomy.Setup(actor, race, actorAge, gender, volume);

            body.Endurance = race.BaseEndurance;
            body.Strength = race.BaseStrenght;
            body.Toughness = race.BaseToughness;
            body.GeneralSpeed = race.GeneralSpeed;

            mind.Inteligence = race.BaseInt;
            mind.Precision = race.BasePrecision;

            soul.WillPower = race.BaseWillPower;
            soul.BaseManaRegen = race.BaseManaRegen;
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