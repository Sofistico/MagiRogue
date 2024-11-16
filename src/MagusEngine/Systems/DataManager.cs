using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using Arquimedes.Utils;
using MagusEngine.Core;
using MagusEngine.Core.Animations;
using MagusEngine.Core.Civ;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Entities.StarterScenarios;
using MagusEngine.Core.Magic;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.Core.WorldStuff.TechRes;
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

        private static bool _firstLoad = true;

        #endregion fields

        #region jsons

        public static Dictionary<string, ItemTemplate> ListOfItems { get; } = GetSourceTreeDict<ItemTemplate>(@".\Data\Items\items_*");

        public static Dictionary<string, Material> ListOfMaterials { get; } = GetSourceTreeDict<Material>(@".\Data\Materials\material_*");

        public static Dictionary<string, Spell> ListOfSpells { get; } = GetSourceTreeDict<Spell>(@".\Data\Spells\spells_*");

        public static Dictionary<string, Organ> ListOfOrgans { get; } = GetSourceTreeDict<Organ>(@".\Data\Bodies\organs_*");

        public static Dictionary<string, Limb> ListOfLimbs { get; } = GetSourceTreeDict<Limb>(@".\Data\Bodies\limbs_*");

        public static Dictionary<string, Furniture> ListOfFurnitures { get; } = GetSourceTreeDict<Furniture>(@".\Data\Furniture\fur_*");

        public static Dictionary<string, RoomTemplate> ListOfRooms { get; } = GetSourceTreeDict<RoomTemplate>(@".\Data\Rooms\room_*");

        // races can be dynamically generated ingame
        public static Dictionary<string, Race> ListOfRaces { get; } = GetSourceTreeDict<Race>(@".\Data\Races\race_*");

        public static Dictionary<string, Scenario> ListOfScenarios { get; } = GetSourceTreeDict<Scenario>(@".\Data\Scenarios\scenarios_*");

        public static Dictionary<string, BodyPlan> ListOfBpPlan { get; } = GetSourceTreeDict<BodyPlan>(@".\Data\Bodies\bodies_*");

        public static Dictionary<string, Language> ListOfLanguages { get; } = GetSourceTreeDict<Language>(@".\Data\Language\language_*");

        public static List<Profession> ListOfProfessions { get; } = GetSourceTreeList<Profession>(@".\Data\Professions\profession_*");

        public static Dictionary<string, CultureTemplate> ListOfCultures { get; } = GetSourceTreeDict<CultureTemplate>(@".\Data\Cultures\cultures_*");

        public static Dictionary<string, Research> ListOfResearches { get; } = GetSourceTreeDict<Research>(@".\Data\Research\research_*");

        public static Dictionary<string, Reaction> ListOfReactions { get; } = GetSourceTreeDict<Reaction>(@".\Data\Reaction\reaction_*");

        public static List<Ruleset> ListOfRules { get; } = GetSourceTreeList<Ruleset>(@".\Data\Rules\rules_*");

        public static Dictionary<string, Plant> ListOfPlants { get; } = GetSourceTreeDict<Plant>(@".\Data\Plant\plant_*");

        public static Dictionary<string, TissuePlanTemplate> ListOfTissuePlans { get; } = GetSourceTreeDict<TissuePlanTemplate>(@".\Data\Bodies\tissue_*");

        public static Dictionary<string, DamageType> ListOfDamageTypes { get; } = GetSourceTreeDict<DamageType>(@".\Data\Damage\dmg_*");

        public static Dictionary<string, AnimationBase> ListOfAnimations { get; } = GetSourceTreeDict<AnimationBase>(@".\Data\Animations\*_animations*");

        #region Descriptors

        public static List<string> ListOfRealmsName { get; } = GetSourceTreeList<string>(@".\Data\Descriptors\realms_*");

        public static List<string> ListOfMagicFounts { get; } = GetSourceTreeList<string>(@".\Data\Descriptors\fount_*");

        public static List<string> ListOfAdjectives { get; } = GetSourceTreeList<string>(@".\Data\Descriptors\adjectives_*");

        public static Dictionary<string, ShapeDescriptor> ListOfShapes { get; } = GetSourceTreeDict<ShapeDescriptor>(@".\Data\Descriptors\shapes_*");

        #endregion Descriptors

        #endregion jsons

        private static Dictionary<string, T> GetSourceTreeDict<T>(string wildCard) where T : IJsonKey => GetSourceTreeList<T>(wildCard).ToDictionary(static val => val.Id);

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

        public static Spell? QuerySpellInData(string spellId, double proficiency = 0)
        {
            return ListOfSpells.TryGetValue(spellId, out var spell) ? spell.Copy(proficiency) : null;
        }

        public static Limb? QueryLimbInData(string limbId)
        {
            return ListOfLimbs.TryGetValue(limbId, out var limb) ? limb.Copy() : null;
        }

        public static Organ? QueryOrganInData(string organId)
        {
            return ListOfOrgans.TryGetValue(organId, out var organ) ? organ.Copy() : null;
        }

        public static Item? QueryItemInData(string itemId)
        {
            return ListOfItems.TryGetValue(itemId, out var item) ? (Item?)item : null;
        }

        public static Item? QueryItemInData(string itemId, Material material) => QueryItemInData(itemId)?.ConfigureMaterial(material);

        public static Item? QueryItemInData(string itemId, Material material, Point point) => QueryItemInData(itemId)?.ConfigureMaterial(material).ConfigurePoint(point);

        public static Furniture? QueryFurnitureInData(string furnitureId)
        {
            return ListOfFurnitures.TryGetValue(furnitureId, out var fur) ? fur.Copy() : null;
        }

        public static RoomTemplate? QueryRoomInData(string roomId)
        {
            return ListOfRooms.TryGetValue(roomId, out var room) ? room : null;
        }

        public static Material? QueryMaterial(string id)
        {
            if (_firstLoad)
            {
                _firstLoad = false;
                var list = ListOfMaterials.Values.Where(static i => !string.IsNullOrEmpty(i.InheirtFrom)).ToArray();
                for (int i = 0; i < list.Length; i++)
                {
                    var mat = list[i];
                    if (mat is null)
                        continue;
                    if (!ListOfMaterials.TryGetValue(mat.InheirtFrom!, out var inheirtFrom))
                    {
                        Locator.GetService<MagiLog>().Log($"Material to inheirt from was null! Id: {mat.InheirtFrom}");
                        continue;
                    }

                    inheirtFrom.CopyTo(mat);
                }
            }
            if (ListOfMaterials.TryGetValue(id, out var material))
                return material; // when there is a need to copy the material, for now let's try the flyweight pattern
            return null;
        }

        public static Race? QueryRaceInData(string raceId)
        {
            return ListOfRaces.TryGetValue(raceId, out var race) ? race : null;
        }

        public static Scenario? QueryScenarioInData(string scenarioId)
        {
            return ListOfScenarios.TryGetValue(scenarioId, out var val) ? val : null;
        }

        public static BodyPlan? QueryBpPlanInData(string bpPlanId)
        {
            return ListOfBpPlan.TryGetValue(bpPlanId, out var val) ? val : null;
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
            return ListOfLanguages.TryGetValue(languageId, out var lang) ? lang : null;
        }

        public static Profession? QueryProfessionInData(string professionId)
        {
            return ListOfProfessions.Find(i => i.Id.Equals(professionId));
        }

        public static ShapeDescriptor? QueryShapeDescInData(string shapeId)
        {
            return ListOfShapes.TryGetValue(shapeId, out var val) ? val : null;
        }

        public static CultureTemplate? QueryCultureTemplateInData(string cultureId)
        {
            return ListOfCultures.TryGetValue(cultureId, out var val) ? val : null;
        }

        public static List<CultureTemplate>? QueryCultureTemplateFromBiome(string biomeId) => ListOfCultures.Values.Where(i => i.StartBiome.Equals(biomeId)).ToList();

        public static Research? QueryResearchInData(string researchId)
        {
            return ListOfResearches.TryGetValue(researchId, out var val) ? val : null;
        }

        public static Plant? QueryPlantInData(string plantId)
        {
            return ListOfPlants.TryGetValue(plantId, out var val) ? val : null;
        }

        public static TissuePlanTemplate? QueryTissuePlanInData(string tissuePlanId)
        {
            return ListOfTissuePlans.TryGetValue(tissuePlanId, out var val) ? val : null;
        }

        public static DamageType? QueryDamageInData(string dmgId)
        {
            return ListOfDamageTypes.TryGetValue(dmgId, out var dmg) ? dmg : null;
        }

        public static AnimationBase? QueryAnimationInData(string animationId)
        {
            return ListOfAnimations.TryGetValue(animationId, out var value) ? value : null;
        }

        #endregion Query

        #region rng

        public static Language? RandomLangugage() => ListOfLanguages.GetRandomItemFromCollection().Value;

        public static string? RandomRealm() => ListOfRealmsName.GetRandomItemFromList();

        public static Race? RandomRace() => ListOfRaces.GetRandomItemFromCollection().Value;

        public static Research? RandomMagicalResearch() => ListOfResearches.Values.Where(static i => i.IsMagical).GetRandomItemFromCollection();

        public static Research? RandomNonMagicalResearch() => ListOfResearches.Values.Where(static i => !i.IsMagical).GetRandomItemFromCollection();

        #endregion rng

        #region helper methods

        public static List<Reaction> GetProductsByTag(RoomTag tag) => ListOfReactions.Values.Where(i => i.RoomTag.Contains(tag)).ToList();

        public static Material? QueryMaterialWithType(MaterialType typeToMake) => ListOfMaterials.Values.Where(i => i.Type == typeToMake).GetRandomItemFromCollection();

        public static Material? QueryMaterialWithTrait(Trait trait) => ListOfMaterials?.Values.Where(i => i.ConfersTraits?.Contains(trait) == true).GetRandomItemFromCollection();

        #endregion helper methods
    }
}
