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
        #region fields

        // TODO: change all List to dictionaries! and make it all private
        private static bool firstLoad = true;
        private static readonly Dictionary<string, ItemTemplate> _items = GetSourceTreeDict<ItemTemplate>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Material> _materials = GetSourceTreeDict<Material>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, SpellBase> _spells = GetSourceTreeDict<SpellBase>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Organ> _organs = GetSourceTreeDict<Organ>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Limb> _limbs = GetSourceTreeDict<Limb>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Furniture> _furs = GetSourceTreeDict<Furniture>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, RoomTemplate> _rooms = GetSourceTreeDict<RoomTemplate>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Race> _races = GetSourceTreeDict<Race>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Scenario> _scenarios = GetSourceTreeDict<Scenario>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, BodyPlan> _bodyPlans = GetSourceTreeDict<BodyPlan>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Language> _languages = GetSourceTreeDict<Language>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Profession> _professions = GetSourceTreeDict<Profession>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, CultureTemplate> _cultures = GetSourceTreeDict<CultureTemplate>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Research> _researches = GetSourceTreeDict<Research>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Reaction> _reactions = GetSourceTreeDict<Reaction>(@".\Data\Items\items_*");
        private static readonly List<Ruleset> _rules = GetSourceTreeList<Ruleset>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, Plant> _plants = GetSourceTreeDict<Plant>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, DamageType> _damageTypes = GetSourceTreeDict<DamageType>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, TissuePlanTemplate> _tissuePlans = GetSourceTreeDict<TissuePlanTemplate>(@".\Data\Items\items_*");
        private static readonly List<string> _realmsNames = GetSourceTreeList<string>(@".\Data\Items\items_*");
        private static readonly List<string> _magicFounts = GetSourceTreeList<string>(@".\Data\Items\items_*");
        private static readonly List<string> _adjectives = GetSourceTreeList<string>(@".\Data\Items\items_*");
        private static readonly Dictionary<string, ShapeDescriptor> _shapes = GetSourceTreeDict<ShapeDescriptor>(@".\Data\Items\items_*");

        #endregion fields

        #region jsons

        public static Dictionary<string, ItemTemplate> ListOfItems => _items;

        public static Dictionary<string, Material> ListOfMaterials => _materials;

        public static Dictionary<string, SpellBase> ListOfSpells => _spells;

        public static Dictionary<string, Organ> ListOfOrgans => _organs;

        public static Dictionary<string, Limb> ListOfLimbs => _limbs;

        public static Dictionary<string, Furniture> ListOfFurnitures => _furs;

        public static Dictionary<string, RoomTemplate> ListOfRooms => _rooms;

        // races can be dynamically generated ingame
        public static Dictionary<string, Race> ListOfRaces => _races;

        public static Dictionary<string, Scenario> ListOfScenarios => _scenarios;

        public static Dictionary<string, BodyPlan> ListOfBpPlan => _bodyPlans;

        public static Dictionary<string, Language> ListOfLanguages => _languages;

        public static Dictionary<string, Profession> ListOfProfessions => _professions;

        public static Dictionary<string, CultureTemplate> ListOfCultures => _cultures;

        public static Dictionary<string, Research> ListOfResearches => _researches;

        public static Dictionary<string, Reaction> ListOfReactions => _reactions;

        public static List<Ruleset> ListOfRules => _rules;

        public static Dictionary<string, Plant> ListOfPlants => _plants;

        public static Dictionary<string, TissuePlanTemplate> ListOfTissuePlans => _tissuePlans;

        public static Dictionary<string, DamageType> ListOfDamageTypes => _damageTypes;

        #region Descriptors

        public static List<string> ListOfRealmsName => _realmsNames;

        public static List<string> ListOfMagicFounts => _magicFounts;

        public static List<string> ListOfAdjectives => _adjectives;

        public static Dictionary<string, ShapeDescriptor> ListOfShapes => _shapes;

        #endregion Descriptors

        #endregion jsons

        private static Dictionary<string, T> GetSourceTreeDict<T>(string wildCard) where T : IJsonKey => GetSourceTreeList<T>(wildCard).ToDictionary(val => val.Id);

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
            if (ListOfSpells.TryGetValue(spellId, out var spell))
                return spell.Copy(proficiency);
            return null;
        }

        public static Limb? QueryLimbInData(string limbId)
        {
            if (ListOfLimbs.TryGetValue(limbId, out var limb))
                return limb.Copy();
            return null;
        }

        public static Organ? QueryOrganInData(string organId)
        {
            if (ListOfOrgans.TryGetValue(organId, out var organ))
                return organ.Copy();
            return null;
        }

        public static Item? QueryItemInData(string itemId)
        {
            if (ListOfItems.TryGetValue(itemId, out var item))
                return item;
            return null;
        }

        public static Item? QueryItemInData(string itemId, Material material) => QueryItemInData(itemId)?.ConfigureMaterial(material);

        public static Furniture? QueryFurnitureInData(string furnitureId)
        {
            if (ListOfFurnitures.TryGetValue(furnitureId, out var fur))
                return fur;
            return null;
        }

        public static RoomTemplate? QueryRoomInData(string roomId)
        {
            if (ListOfRooms.TryGetValue(roomId, out var room))
                return room;
            return null;
        }

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
                    if (!ListOfMaterials.TryGetValue(mat.InheirtFrom, out var inheirtFrom))
                    {
                        Locator.GetService<MagiLog>().Log($"Material to inheirt from was null! Id: {mat.InheirtFrom}");
                        continue;
                    }

                    inheirtFrom.CopyTo(mat);
                }
            }
            if (ListOfMaterials.TryGetValue(id, out var material))
                return material.Copy();
            return null;
        }

        public static Race? QueryRaceInData(string raceId)
        {
            if (ListOfRaces.TryGetValue(raceId, out var race))
                return race.Clone();
            return null;
        }

        public static Scenario? QueryScenarioInData(string scenarioId)
        {
            if (ListOfScenarios.TryGetValue(scenarioId, out var val))
                return val;
            return null;
        }

        public static BodyPlan? QueryBpPlanInData(string bpPlanId)
        {
            if (ListOfBpPlan.TryGetValue(bpPlanId, out var val))
                return val;
            return null;
        }

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

        public static Language? QueryLanguageInData(string languageId)
        {
            if (ListOfLanguages.TryGetValue(languageId, out var lang))
                return lang;
            return null;
        }

        public static Profession? QueryProfessionInData(string professionId)
        {
            if (ListOfProfessions.TryGetValue(professionId, out var val))
                return val;
            return null;
        }

        public static ShapeDescriptor? QueryShapeDescInData(string shapeId)
        {
            if (ListOfShapes.TryGetValue(shapeId, out var val))
                return val;
            return null;
        }

        public static CultureTemplate? QueryCultureTemplateInData(string cultureId)
        {
            if (ListOfCultures.TryGetValue(cultureId, out var val))
                return val;
            return null;
        }

        public static List<CultureTemplate>? QueryCultureTemplateFromBiome(string biomeId) => ListOfCultures.Values.Where(i => i.StartBiome.Equals(biomeId)).ToList();

        public static Research? QueryResearchInData(string researchId)
        {
            if (ListOfResearches.TryGetValue(researchId, out var val))
                return val;
            return null;
        }

        public static Plant? QueryPlantInData(string plantId)
        {
            if (ListOfPlants.TryGetValue(plantId, out var val))
                return val.Copy();
            return null;
        }

        public static TissuePlanTemplate? QueryTissuePlanInData(string tissuePlanId)
        {
            if (ListOfTissuePlans.TryGetValue(tissuePlanId, out var val))
                return val;
            return null;
        }

        public static DamageType? QueryDamageInData(string dmgId)
        {
            if (ListOfDamageTypes.TryGetValue(dmgId, out var dmg))
                return dmg;
            return null;
        }

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
