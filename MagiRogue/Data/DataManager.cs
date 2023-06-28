﻿using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.Entities;
using MagiRogue.Entities.Core;
using MagiRogue.Entities.StarterScenarios;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Magic;
using MagiRogue.GameSys.Planet.History;
using MagiRogue.GameSys.Planet.TechRes;
using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Veggies;
using MagiRogue.Utils;
using MagiRogue.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Data
{
    public static class DataManager
    {
        // TODO: change all IReadOnlyList to dictionaries!
        // and make it all private

        #region jsons

        public static readonly IReadOnlyList<ItemTemplate> ListOfItems =
            GetSourceTree<ItemTemplate>(@".\Data\*\items_*");

        public static readonly IReadOnlyList<MaterialTemplate> ListOfMaterials =
            GetSourceTree<MaterialTemplate>(@".\Data\*\material_*");

        public static readonly IReadOnlyList<SpellBase> ListOfSpells =
            GetSourceTree<SpellBase>(@".\Data\*\spells_*");

        public static readonly IReadOnlyList<Organ> ListOfOrgans =
            GetSourceTree<Organ>(@".\Data\*\organs_*");

        public static readonly IReadOnlyList<Limb> ListOfLimbs =
            GetSourceTree<Limb>(@".\Data\*\limbs_*");

        public static readonly IReadOnlyList<BasicTile> ListOfTiles =
            GetSourceTree<BasicTile>(@".\Data\*\tiles_*");

        public static readonly IReadOnlyList<Furniture> ListOfFurnitures =
            GetSourceTree<Furniture>(@".\Data\*\fur_*");

        public static readonly IReadOnlyList<RoomTemplate> ListOfRooms =
            GetSourceTree<RoomTemplate>(@".\Data\*\room_*");

        // races can be dynamically generated ingame
        public static readonly IReadOnlyList<Race> ListOfRaces =
            GetSourceTree<Race>(@".\Data\*\race_*");

        public static readonly IReadOnlyList<Scenario> ListOfScenarios =
            GetSourceTree<Scenario>(@".\Data\*\scenarios_*");

        public static readonly IReadOnlyList<BodyPlan> ListOfBpPlan =
            GetSourceTree<BodyPlan>(@".\Data\*\bodies_*.json");

        public static readonly IReadOnlyList<Language> ListOfLanguages =
            GetSourceTree<Language>(@".\Data\*\language_*");

        public static readonly IReadOnlyList<Profession> ListOfProfessions =
            GetSourceTree<Profession>(@".\Data\*\profession_*");

        public static readonly IReadOnlyList<CultureTemplate> ListOfCultures =
            GetSourceTree<CultureTemplate>(@".\Data\*\cultures_*");

        public static readonly IReadOnlyList<Research> ListOfResearches =
            GetSourceTree<Research>(@".\Data\*\research_*");

        public static readonly IReadOnlyList<Reaction> ListOfReactions =
            GetSourceTree<Reaction>(@".\Data\*\reaction_*");

        public static readonly IReadOnlyList<Ruleset> ListOfRules =
            GetSourceTree<Ruleset>(@".\Data\*\rules_*");

        public static readonly IReadOnlyList<Plant> ListOfPlants =
            GetSourceTree<Plant>(@".\Data\*\plant_*");

        public static readonly IReadOnlyList<TissuePlanTemplate> ListOfTissuePlans =
            GetSourceTree<TissuePlanTemplate>(@".\Data\*\tissue_*");

        #region Descriptors

        public static readonly IReadOnlyList<string> ListOfRealmsName =
            GetSourceTree<string>(@".\Data\*\realms_*.json");
        public static readonly IReadOnlyList<string> ListOfMagicFounts =
            GetSourceTree<string>(@".\Data\*\magic_fount_*.json");
        public static readonly IReadOnlyList<string> ListOfAdjectives =
            GetSourceTree<string>(@".\Data\*\adjectives_*.json");
        public static readonly IReadOnlyList<ShapeDescriptor> ListOfShapes =
            GetSourceTree<ShapeDescriptor>(@".\Data\*\shapes_*.json");

        #endregion Descriptors

        #endregion jsons

        public static IReadOnlyList<T> GetSourceTree<T>(string wildCard)
        {
            string[] files = FileUtils.GetFiles(wildCard);

            List<List<T>> listTList = new();

            for (int i = 0; i < files.Length; i++)
            {
                var t = JsonUtils.JsonDeseralize<List<T>>(files[i]);
                listTList.Add(t);
            }
            List<T> allTList = new();

            foreach (List<T> tList in listTList)
            {
                if (tList is null)
                    continue;
                allTList.AddRange(tList);
            }

            return allTList.AsReadOnly();
        }

        #region Queryes

        public static SpellBase QuerySpellInData(string spellId) => ListOfSpells.FirstOrDefault
                (m => m.SpellId.Equals(spellId))?.Copy();

        public static Limb QueryLimbInData(string limbId)
        {
            var limb = ListOfLimbs.FirstOrDefault(l => l.Id.Equals(limbId), null);
            return limb?.Copy();
        }

        public static Organ? QueryOrganInData(string organId)
        {
            var organ = ListOfOrgans.FirstOrDefault(o => o.Id.Equals(organId), null);
            return organ?.Copy();
        }

        public static TileBase QueryTileInData(string tileId)
            => ListOfTiles.FirstOrDefault(t => t.TileId.Equals(tileId))?.Copy();

        public static T QueryTileInData<T>(string tileId) where T : TileBase
            => (T)ListOfTiles.FirstOrDefault(t => t.TileId.Equals(tileId))?.Copy();

        public static T QueryTileInData<T>(string tileId, Point pos) where T : TileBase
            => (T)ListOfTiles.FirstOrDefault(t => t.TileId.Equals(tileId))?.Copy(pos);

        public static Item QueryItemInData(string itemId)
            => ListOfItems.FirstOrDefault(i => i.Id.Equals(itemId));

        public static Furniture QueryFurnitureInData(string furnitureId)
            => ListOfFurnitures.FirstOrDefault(i => i.FurId.Equals(furnitureId))?.Copy();

        public static RoomTemplate QueryRoomInData(string roomId)
            => ListOfRooms.FirstOrDefault(i => i.Id.Equals(roomId));

        public static MaterialTemplate QueryMaterial(string id)
            => ListOfMaterials.FirstOrDefault(a => a.Id.Equals(id));

        public static List<BasicTile> QueryTilesInDataWithTrait(Trait trait)
            => ListOfTiles.Where(i => i.HasAnyTrait()
                && i.Traits.Contains(trait)).ToList();

        public static Race QueryRaceInData(string raceId)
            => (Race)(ListOfRaces.FirstOrDefault(c => c.Id.Equals(raceId))?.Clone());

        public static Scenario QueryScenarioInData(string scenarioId)
            => ListOfScenarios.FirstOrDefault(c => c.Id.Equals(scenarioId));

        public static BodyPlan QueryBpPlanInData(string bpPlanId)
            => ListOfBpPlan.FirstOrDefault(c => c.Id.Equals(bpPlanId));

        public static List<BodyPart> QueryBpsPlansInDataAndReturnBodyParts(string[] bpPlansId, Race race = null!)
        {
            List<BodyPart> bps = new();
            BodyPlanCollection collection = new();
            foreach (var id in bpPlansId)
            {
                BodyPlan bp = QueryBpPlanInData(id);
                collection.BodyPlans.Add(bp);
            }
            bps.AddRange(collection.ExecuteAllBodyPlans(race));

            return bps;
        }

        public static Language QueryLanguageInData(string languageId)
            => ListOfLanguages.FirstOrDefault(i => i.Id.Equals(languageId));

        public static Profession QueryProfessionInData(string professionId)
            => ListOfProfessions.FirstOrDefault(i => i.Id.Equals(professionId));

        public static ShapeDescriptor QueryShapeDescInData(string shapeId)
            => ListOfShapes.FirstOrDefault(i => i.Id.Equals(shapeId));

        public static CultureTemplate QueryCultureTemplateInData(string cultureId)
            => ListOfCultures.FirstOrDefault(i => i.Id.Equals(cultureId));

        public static List<CultureTemplate> QueryCultureTemplateFromBiome(string biomeId)
            => ListOfCultures.Where(i => i.StartBiome.Equals(biomeId)).ToList();

        public static Research QueryResearchInData(string researchId)
            => ListOfResearches.FirstOrDefault(i => i.Id.Equals(researchId));

        public static Plant QueryPlantInData(string plantId)
            => ListOfPlants.FirstOrDefault(i => i.Id.Equals(plantId))?.Clone();

        public static TissuePlanTemplate QueryTissuePlanInData(string tissuePlanId)
            => ListOfTissuePlans.FirstOrDefault(i => i.Id.Equals(tissuePlanId));

        #endregion Queryes

        public static Language RandomLangugage()
            => ListOfLanguages.GetRandomItemFromList();

        public static string RandomRealm()
            => ListOfRealmsName.GetRandomItemFromList();

        public static Race RandomRace()
            => ListOfRaces.GetRandomItemFromList();

        public static Research RandomMagicalResearch()
            => ListOfResearches.Where(i => i.IsMagical).ToList().GetRandomItemFromList();

        public static Research RandomNonMagicalResearch()
            => ListOfResearches.Where(i => !i.IsMagical).ToList().GetRandomItemFromList();

        public static List<Reaction> GetProductsByTag(RoomTag tag)
        {
            return ListOfReactions.Where(i => i.RoomTag.Contains(tag)).ToList();
        }
    }
}