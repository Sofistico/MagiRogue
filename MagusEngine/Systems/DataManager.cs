using Arquimedes.Enumerators;
using Arquimedes.Utils;
using MagusEngine.Core;
using MagusEngine.Core.Civ;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Entities.StarterScenarios;
using MagusEngine.Core.Magic;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.Core.WorldStuff.TechRes;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Serialization;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Serialization.MapConverter;
using MagusEngine.Services;
using MagusEngine.Utils.Extensions;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Systems
{
    public static class DataManager
    {
        // TODO: change all List to dictionaries! and make it all private
        private static bool firstLoad = true;

        #region jsons

        public static Dictionary<string, ItemTemplate> ListOfItems => GetSourceTree<ItemTemplate>(@".\Data\Items\items_*");

        public static Dictionary<string, Material> ListOfMaterials => GetSourceTree<Material>(@".\Data\Materials\material_*");

        public static Dictionary<string, SpellBase> ListOfSpells => GetSourceTree<SpellBase>(@".\Data\Spells\spells_*");

        public static Dictionary<string, Organ> ListOfOrgans => GetSourceTree<Organ>(@".\Data\Bodies\organs_*");

        public static Dictionary<string, Limb> ListOfLimbs => GetSourceTree<Limb>(@".\Data\Bodies\limbs_*");

        public static Dictionary<string, Furniture> ListOfFurnitures => GetSourceTree<Furniture>(@".\Data\Furniture\fur_*");

        public static Dictionary<string, RoomTemplate> ListOfRooms => GetSourceTree<RoomTemplate>(@".\Data\Rooms\room_*");

        // races can be dynamically generated ingame
        public static Dictionary<string, Race> ListOfRaces => GetSourceTree<Race>(@".\Data\Races\race_*");

        public static Dictionary<string, Scenario> ListOfScenarios => GetSourceTree<Scenario>(@".\Data\Scenarios\scenarios_*");

        public static Dictionary<string, BodyPlan> ListOfBpPlan => GetSourceTree<BodyPlan>(@".\Data\Bodies\bodies_*.json");

        public static Dictionary<string, Language> ListOfLanguages => GetSourceTree<Language>(@".\Data\Language\language_*");

        public static Dictionary<string, Profession> ListOfProfessions => GetSourceTree<Profession>(@".\Data\Professions\profession_*");

        public static Dictionary<string, CultureTemplate> ListOfCultures => GetSourceTree<CultureTemplate>(@".\Data\Cultures\cultures_*");

        public static Dictionary<string, Research> ListOfResearches => GetSourceTree<Research>(@".\Data\Research\research_*");

        public static Dictionary<string, Reaction> ListOfReactions => GetSourceTree<Reaction>(@".\Data\Reaction\reaction_*");

        public static List<Ruleset> ListOfRules => GetSourceTreeList<Ruleset>(@".\Data\Rules\rules_*");

        public static Dictionary<string, Plant> ListOfPlants => GetSourceTree<Plant>(@".\Data\Plant\plant_*");

        public static Dictionary<string, TissuePlanTemplate> ListOfTissuePlans => GetSourceTree<TissuePlanTemplate>(@".\Data\Bodies\tissue_*");

        public static Dictionary<string, DamageType> ListOfDamageTypes => GetSourceTree<DamageType>(@".\Data\Damage\dmg_*");

        #region Descriptors

        public static List<string> ListOfRealmsName => GetSourceTreeList<string>(@".\Data\Descriptors\realms_*.json");

        public static List<string> ListOfMagicFounts => GetSourceTreeList<string>(@".\Data\Descriptors\magic_fount_*.json");

        public static List<string> ListOfAdjectives => GetSourceTreeList<string>(@".\Data\Descriptors\adjectives_*.json");

        public static Dictionary<string, ShapeDescriptor> ListOfShapes => GetSourceTree<ShapeDescriptor>(@".\Data\Descriptors\shapes_*.json");

        #endregion Descriptors

        #endregion jsons

        private static Dictionary<string, T> GetSourceTree<T>(string wildCard) where T : IJsonKey
        {
            string[] files = FileUtils.GetFiles(wildCard);

            List<List<T>> listTList = [];

            for (int i = 0; i < files.Length; i++)
            {
                var tList = JsonUtils.JsonDeseralize<List<T>>(files[i])!;
                listTList.Add(tList);
            }
            return listTList.ReturnListListTAsListT().ToDictionary(val => val.Id);
        }

        private static List<T> GetSourceTreeList<T>(string wildCard)
        {
            string[] files = FileUtils.GetFiles(wildCard);

            List<List<T>> listTList = [];

            for (int i = 0; i < files.Length; i++)
            {
                var tList = JsonUtils.JsonDeseralize<List<T>>(files[i])!;
                listTList.Add(tList);
            }
            return listTList.ReturnListListTAsListT();
        }

        #region Query

        public static SpellBase? QuerySpellInData(string spellId, double proficiency = 0)
        {
            var spell = ListOfSpells[spellId]?.Copy();
            if (spell is not null)
                spell.Proficiency = proficiency;
            return spell;
        }

        public static Limb QueryLimbInData(string limbId)
        {
            var limb = ListOfLimbs[limbId];
            return limb?.Copy()!;
        }

        public static Organ? QueryOrganInData(string organId)
        {
            var organ = ListOfOrgans[organId];
            return organ?.Copy();
        }

        public static Item? QueryItemInData(string itemId) => ListOfItems[itemId];

        public static Item? QueryItemInData(string itemId, Material material) => QueryItemInData(itemId)?.ConfigureMaterial(material);

        public static Furniture? QueryFurnitureInData(string furnitureId) => ListOfFurnitures[furnitureId].Copy();

        public static RoomTemplate? QueryRoomInData(string roomId) => ListOfRooms[roomId];

        public static Material? QueryMaterial(string id)
        {
            if (firstLoad)
            {
                firstLoad = false;
                var list = ListOfMaterials.Values.Where(i => !string.IsNullOrEmpty(i.InheirtFrom)).ToArray();
                for (int i = 0; i < list.Length; i++)
                {
                    var mat = list[i];
                    if (mat is null)
                        continue;
                    var inheirtFrom = ListOfMaterials[mat.InheirtFrom];
                    if (inheirtFrom is null)
                    {
                        Locator.GetService<MagiLog>().Log($"Material to inheirt from was null! Id: {mat.InheirtFrom}");
                        continue;
                    }

                    inheirtFrom.CopyTo(mat);
                }
            }
            return ListOfMaterials[id];
        }

        public static Race? QueryRaceInData(string raceId) => ListOfRaces[raceId]?.Clone();

        public static Scenario? QueryScenarioInData(string scenarioId) => ListOfScenarios[scenarioId];

        public static BodyPlan? QueryBpPlanInData(string bpPlanId) => ListOfBpPlan[bpPlanId];

        public static List<BodyPart> QueryBpsPlansInDataAndReturnBodyParts(string[] bpPlansId, Race race = null!)
        {
            List<BodyPart> bps = [];
            BodyPlanCollection collection = new();
            foreach (var id in bpPlansId)
            {
                BodyPlan? bp = QueryBpPlanInData(id);
                if (bp != null)
                    collection.BodyPlans.Add(bp);
            }
            bps.AddRange(collection.ExecuteAllBodyPlans(race));

            return bps;
        }

        public static Language? QueryLanguageInData(string languageId) => ListOfLanguages[languageId];

        public static Profession? QueryProfessionInData(string professionId) => ListOfProfessions[professionId];

        public static ShapeDescriptor? QueryShapeDescInData(string shapeId) => ListOfShapes[shapeId];

        public static CultureTemplate? QueryCultureTemplateInData(string cultureId) => ListOfCultures[cultureId];

        public static List<CultureTemplate>? QueryCultureTemplateFromBiome(string biomeId) => ListOfCultures.Values.Where(i => i.StartBiome.Equals(biomeId)).ToList();

        public static Research? QueryResearchInData(string researchId) => ListOfResearches[researchId];

        public static Plant? QueryPlantInData(string plantId) => ListOfPlants[plantId]?.Clone();

        public static TissuePlanTemplate? QueryTissuePlanInData(string tissuePlanId) => ListOfTissuePlans[tissuePlanId];

        public static DamageType? QueryDamageInData(string dmgId) => ListOfDamageTypes[dmgId];

        #endregion Query

        #region rng

        public static Language? RandomLangugage() => ListOfLanguages.GetRandomItemFromList().Value;

        public static string? RandomRealm() => ListOfRealmsName.GetRandomItemFromList();

        public static Race? RandomRace() => ListOfRaces.GetRandomItemFromList().Value;

        public static Research? RandomMagicalResearch() => ListOfResearches.Values.Where(i => i.IsMagical).GetRandomItemFromList();

        public static Research? RandomNonMagicalResearch() => ListOfResearches.Values.Where(i => !i.IsMagical).GetRandomItemFromList();

        #endregion rng

        #region helper methods

        public static List<Reaction> GetProductsByTag(RoomTag tag) => ListOfReactions.Values.Where(i => i.RoomTag.Contains(tag)).ToList();

        public static Material? QueryMaterialWithType(MaterialType typeToMake) => ListOfMaterials.Values.Where(i => i.Type == typeToMake).GetRandomItemFromList();

        public static Material? QueryMaterialWithTrait(Trait trait) => ListOfMaterials?.Values.Where(i => i.ConfersTraits?.Contains(trait) == true).GetRandomItemFromList();

        #endregion helper methods
    }
}
