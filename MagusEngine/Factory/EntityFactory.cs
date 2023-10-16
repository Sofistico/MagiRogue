using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Entities.StarterScenarios;
using MagusEngine.Core.Magic;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.ECS.Components.ActorComponents;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Factory
{
    /// <summary>
    /// Utility method for easy creating an actor and item, along with any possible function and extra.
    /// </summary>
    public static class EntityFactory
    {
        private static Actor? actorField;

        public static Actor ActorCreator(HistoricalFigure figure, Point pos)
        {
            var actor = ActorCreator(pos,
                figure.GetRaceId(),
                figure.Body.GetCurrentAge(),
                figure.Body.Anatomy.Gender);
            actor.HistoryId = figure.Id;

            return actor;
        }

        public static Actor ActorCreator(Point position, string raceId,
            string actorName, int actorAge, Sex sex)
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
            actorField = actor;
            SetupBodySoulAndMind(race, actor, actorAge, sex);

            return actor;
        }

        public static Actor ActorCreator(Point position, string raceId, Sex sex, AgeGroup age)
        {
            Race race = DataManager.QueryRaceInData(raceId);
            int glyph = GlyphHelper.GlyphExistInDictionary(race.RaceGlyph) ?
                GlyphHelper.GetGlyph(race.RaceGlyph) : race.RaceGlyph;

            Actor actor = new Actor(race.RaceName,
                race.ReturnForegroundColor(),
                race.ReturnBackgroundColor(),
                glyph,
                position)
            {
                Description = race.Description,
            };
            actorField = actor;
            SetupBodySoulAndMind(race, actor, race.GetAgeFromAgeGroup(age), sex);

            return actor;
        }

        public static Actor ActorCreator(Point position, string raceId, int actorAge, Sex sex)
        {
            Race race = DataManager.QueryRaceInData(raceId);
            int glyph = GlyphHelper.GlyphExistInDictionary(race.RaceGlyph) ?
                GlyphHelper.GetGlyph(race.RaceGlyph) : race.RaceGlyph;

            Actor actor = new Actor(race.RaceName,
                race.ReturnForegroundColor(),
                race.ReturnBackgroundColor(),
                glyph,
                position)
            {
                Description = race.Description,
            };
            actorField = actor;
            SetupBodySoulAndMind(race, actor, actorAge, sex);

            return actor;
        }

        private static Player PlayerCreatorFromActor(Actor actor,
            string scenarioId, Sex sex)
        {
            Scenario scenario = DataManager.QueryScenarioInData(scenarioId)
                ?? throw new ApplicationException($"Scenario {scenarioId} was null!");
            actor.GetAnatomy().Gender = sex;
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
                var queriedSpell = DataManager.QuerySpellInData(str);
                if (queriedSpell is null)
                {
                    var strArray = str.Split('_', ':');
                    if (strArray[0].Contains("any"))
                    {
                        var enumConverted = Enum.Parse<SpellContext>(strArray[1].FirstLetterUpper());
                        var levelOfSpell = int.Parse(strArray[2]);

                        var spells = DataManager.ListOfSpells.Where(i =>
                            i.Context?.Contains(enumConverted) == true && i.SpellLevel == levelOfSpell).ToList();
                        queriedSpell = spells.GetRandomItemFromList();
                    }
                }
                actor.Magic.AddToSpellList(queriedSpell);
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
            anatomy.CurrentAge = anatomy.Race.GetAgeFromAgeGroup(scenario.AgeGroup);

            mind.Inteligence += scenario.Inteligence;
            mind.Precision += scenario.Precision;

            soul.WillPower += scenario.WillPower;
            soul.InitialMana(mind.Inteligence, actor.GetAnatomy().Race);
            soul.MaxMana += scenario.MaxMana;
            soul.CurrentMana = soul.MaxMana;
            soul.BaseManaRegen += scenario.ManaRegen;
        }

        public static Player PlayerCreatorFromActor(Actor actor, Scenario scenario,
            Sex sex)
        {
            return PlayerCreatorFromActor(actor, scenario.Id, sex);
        }

        private static void SetupAbilities(Actor actor, List<AbilityTemplate> abilities)
        {
            foreach (Ability item in abilities)
            {
                actor.Mind.AddAbilityToDictionary(item);
            }
        }

        private static void SetupBodySoulAndMind(Race race, Actor actor, int actorAge, Sex sex)
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

            anatomy.FullSetup(actor, race, actorAge, sex, finalVolume);

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
            soul.InitialMana(mind.Inteligence, race);

            magic.InnateMagicResistance = race.BaseMagicResistance;

            if (anatomy.AllBPs.Any(i => i.Tissues.Any(i => i.Material.Type == MaterialType.Meat)))
            {
                actor.AddComponents(new FoodComponent(Food.Carnivore));
            }
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

        public static Player PlayerCreatorFromZero(Point pos, string race, string name, Sex sex,
            string scenarioId)
        {
            var scenario = DataManager.QueryScenarioInData(scenarioId);
            var foundRace = DataManager.QueryRaceInData(race);
            int age = foundRace.GetAgeFromAgeGroup(scenario.AgeGroup);
            Actor actor = ActorCreator(pos, race, name, age, sex);
            return PlayerCreatorFromActor(actor, scenarioId, sex);
        }

        public static Player PlayerCreatorFromZeroForTest(Point pos, string race, string name, int age, Sex sex,
            string scenarioId)
        {
            Actor actor = ActorCreator(pos, race, name, age, sex);
            return PlayerCreatorFromActor(actor, scenarioId, sex);
        }

        public static Sex GetRandomSex()
        {
            var sexs = new Sex[] { Sex.Male, Sex.Female };
            return sexs.GetRandomItemFromList();
        }
    }
}