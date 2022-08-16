using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.Entities.StarterScenarios;
using MagiRogue.GameSys.Magic;
using MagiRogue.Utils;
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
            int glyph = GlyphHelper.GlyphExistInDictionary(race.RaceGlyph) ?
                GlyphHelper.GetGlyph(race.RaceGlyph) : race.RaceGlyph;

            Actor actor = new Actor(actorName,
                race.ReturnForegroundColor(),
                race.ReturnBackgroundColor(),
                glyph,
                position)
            {
                Description = race.Description,
            };
            SetupBodySoulAndMind(race, actor, actorAge, gender);

            return actor;
        }

        public static Player PlayerCreatorFromActor(Actor actor,
            string scenarioId, Gender gender)
        {
            Scenario scenario = DataManager.QueryScenarioInData(scenarioId);
            actor.GetAnatomy().Gender = gender;
            SetupScenarioStats(scenario, actor);
            SetupAbilities(actor, scenario.Abilities);
            SetupScenarioMagic(actor, scenario);
            SetupInventory(actor, scenario);
            SetupEquipment(actor, scenario);
            Player player = Player.ReturnPlayerFromActor(actor);

            return player;
        }

        private static void SetupEquipment(Actor actor, Scenario scenario)
        {
        }

        private static void SetupInventory(Actor actor, Scenario scenario)
        {
        }

        private static void SetupScenarioMagic(Actor actor, Scenario scenario)
        {
            actor.Magic.ShapingSkill += scenario.ShapingSkill;
            foreach (var str in scenario.SpellsKnow)
            {
                actor.Magic.KnowSpells.Add(DataManager.QuerySpellInData(str));
            }
        }

        private static void SetupScenarioStats(Scenario scenario, Actor actor)
        {
            Body body = actor.Body;
            Anatomy anatomy = actor.GetAnatomy();
            Mind mind = actor.Mind;
            Soul soul = actor.Soul;

            body.Strength += scenario.Strenght;
            body.Toughness += scenario.Toughness;
            body.Endurance += scenario.Endurance;
            body.InitialStamina();
            body.MaxStamina += scenario.MaxStamina;
            body.Stamina = body.MaxStamina;
            body.StaminaRegen += scenario.StaminaRegen;

            anatomy.FitLevel += scenario.FitLevel;
            anatomy.NormalLimbRegen += scenario.LimbRegen;
            anatomy.CurrentAge = anatomy.GetRace().GetAgeFromAgeGroup(scenario.AgeGroup);

            mind.Inteligence += scenario.Inteligence;
            mind.Precision += scenario.Precision;

            soul.WillPower += scenario.WillPower;
            soul.InitialMana(mind.Inteligence);
            soul.MaxMana += scenario.MaxMana;
            soul.CurrentMana = soul.MaxMana;
            soul.BaseManaRegen += scenario.ManaRegen;
        }

        public static Player PlayerCreatorFromActor(Actor actor, Scenario scenario,
            Gender gender)
        {
            return PlayerCreatorFromActor(actor, scenario.Id, gender);
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
            MagicManager magic = actor.Magic;
            Mind mind = actor.Mind;
            Soul soul = actor.Soul;

            int volume = race.GetRngVolume(actorAge);
            actor.Height = race.GetRandomHeight();
            actor.Length = race.GetRandomLength();
            actor.Broadness = race.GetRandomBroadness();
            int volumeWithHeight = actor.Height > 0 ? MathMagi.CalculateVolumeWithModifier(actor.Height, volume) : volume;
            int volumeWithLenght = actor.Length > 0 ? MathMagi.CalculateVolumeWithModifier(actor.Length, volumeWithHeight) : volumeWithHeight;
            int finalVolume = actor.Broadness > 0 ? MathMagi.CalculateVolumeWithModifier(actor.Broadness, volumeWithLenght) : volumeWithLenght;

            race.SetBodyPlan();
            anatomy.Setup(actor, race, actorAge, gender, finalVolume);

            body.Endurance = race.BaseEndurance;
            body.Strength = race.BaseStrenght;
            body.Toughness = race.BaseToughness;
            body.GeneralSpeed = race.GeneralSpeed;
            body.ViewRadius = race.RaceViewRadius;
            body.InitialStamina();

            mind.Inteligence = race.BaseInt;
            mind.Precision = race.BasePrecision;

            soul.WillPower = race.BaseWillPower;
            soul.BaseManaRegen = race.BaseManaRegen;
            soul.InitialMana(mind.Inteligence);

            magic.InnateMagicResistance = race.BaseMagicResistance;
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

        public static Player PlayerCreatorFromZero(Point pos, string race, string name, Gender gender,
            string scenarioId)
        {
            var scenario = DataManager.QueryScenarioInData(scenarioId);
            var foundRace = DataManager.QueryRaceInData(race);
            int age = foundRace.GetAgeFromAgeGroup(scenario.AgeGroup);
            Actor actor = ActorCreatorFirstStep(pos, race, name, age, gender);
            Player player = PlayerCreatorFromActor(actor, scenarioId, gender);
            return player;
        }

        public static Player PlayerCreatorFromZero(Point pos, string race, string name, int age, Gender gender,
            string scenarioId)
        {
            Actor actor = ActorCreatorFirstStep(pos, race, name, age, gender);
            Player player = PlayerCreatorFromActor(actor, scenarioId, gender);
            return player;
        }
    }
}