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

        public static List<ItemTemplate> ListOfItems => GetSourceTree<ItemTemplate>(@".\Data\Items\items_*");

        public static List<Material> ListOfMaterials => GetSourceTree<Material>(@".\Data\Materials\material_*");

        public static List<SpellBase> ListOfSpells => GetSourceTree<SpellBase>(@".\Data\Spells\spells_*");

        public static List<Organ> ListOfOrgans => GetSourceTree<Organ>(@".\Data\Bodies\organs_*");

        public static List<Limb> ListOfLimbs => GetSourceTree<Limb>(@".\Data\Bodies\limbs_*");

        public static List<Furniture> ListOfFurnitures => GetSourceTree<Furniture>(@".\Data\Furniture\fur_*");

        public static List<RoomTemplate> ListOfRooms => GetSourceTree<RoomTemplate>(@".\Data\Rooms\room_*");

        // races can be dynamically generated ingame
        public static List<Race> ListOfRaces => GetSourceTree<Race>(@".\Data\Races\race_*");

        public static List<Scenario> ListOfScenarios => GetSourceTree<Scenario>(@".\Data\Scenarios\scenarios_*");

        public static List<BodyPlan> ListOfBpPlan => GetSourceTree<BodyPlan>(@".\Data\Bodies\bodies_*.json");

        public static List<Language> ListOfLanguages => GetSourceTree<Language>(@".\Data\Language\language_*");

        public static List<Profession> ListOfProfessions => GetSourceTree<Profession>(@".\Data\Professions\profession_*");

        public static List<CultureTemplate> ListOfCultures => GetSourceTree<CultureTemplate>(@".\Data\Cultures\cultures_*");

        public static List<Research> ListOfResearches => GetSourceTree<Research>(@".\Data\Research\research_*");

        public static List<Reaction> ListOfReactions => GetSourceTree<Reaction>(@".\Data\Reaction\reaction_*");

        public static List<Ruleset> ListOfRules => GetSourceTree<Ruleset>(@".\Data\Rules\rules_*");

        public static List<Plant> ListOfPlants => GetSourceTree<Plant>(@".\Data\Plant\plant_*");

        public static List<TissuePlanTemplate> ListOfTissuePlans => GetSourceTree<TissuePlanTemplate>(@".\Data\Bodies\tissue_*");

        public static List<DamageType> ListOfDamageTypes => GetSourceTree<DamageType>(@".\Data\Damage\dmg_*");

        #region Descriptors

        public static List<string> ListOfRealmsName => GetSourceTree<string>(@".\Data\Descriptors\realms_*.json");

        public static List<string> ListOfMagicFounts => GetSourceTree<string>(@".\Data\Descriptors\magic_fount_*.json");

        public static List<string> ListOfAdjectives => GetSourceTree<string>(@".\Data\Descriptors\adjectives_*.json");

        public static List<ShapeDescriptor> ListOfShapes => GetSourceTree<ShapeDescriptor>(@".\Data\Descriptors\shapes_*.json");

        #endregion Descriptors

        #endregion jsons

        public static List<T> GetSourceTree<T>(string wildCard)
        {
            string[] files = FileUtils.GetFiles(wildCard);

            List<List<T>> listTList = [];

            for (int i = 0; i < files.Length; i++)
            {
                var t = JsonUtils.JsonDeseralize<List<T>>(files[i])!;
                listTList.Add(t);
            }
            return listTList.ReturnListListTAsListT();
        }

        #region Query

        public static SpellBase? QuerySpellInData(string spellId, double proficiency = 0)
        {
            var spell = ListOfSpells.Find(m => m.SpellId.Equals(spellId))?.Copy();
            if (spell is not null)
                spell.Proficiency = proficiency;
            return spell;
        }

        public static Limb QueryLimbInData(string limbId)
        {
            var limb = ListOfLimbs.Find(l => l.Id.Equals(limbId));
            return limb?.Copy()!;
        }

        public static Organ? QueryOrganInData(string organId)
        {
            var organ = ListOfOrgans.Find(o => o.Id.Equals(organId));
            return organ?.Copy();
        }

        public static Item QueryItemInData(string itemId) => ListOfItems.Find(i => i.Id.Equals(itemId));

        public static Item QueryItemInData(string itemId, Material material) => QueryItemInData(itemId).ConfigureMaterial(material);

        public static Furniture? QueryFurnitureInData(string furnitureId) => ListOfFurnitures.Find(i => i.FurId.Equals(furnitureId))?.Copy();

        public static RoomTemplate? QueryRoomInData(string roomId) => ListOfRooms.Find(i => i.Id.Equals(roomId));

        public static Material? QueryMaterial(string id)
        {
            if (firstLoad)
            {
                firstLoad = false;
                var list = ListOfMaterials.Where(i => !string.IsNullOrEmpty(i.InheirtFrom)).ToArray();
                for (int i = 0; i < list.Length; i++)
                {
                    var mat = list[i];
                    var inheirtFrom = ListOfMaterials.Find(i => i.Id.Equals(mat.InheirtFrom));
                    if (inheirtFrom is null)
                    {
                        Locator.GetService<MagiLog>().Log($"Material to inheirt from was null! Id: {mat.InheirtFrom}");
                        continue;
                    }

                    inheirtFrom.CopyTo(mat);
                }
            }
            return ListOfMaterials.Find(a => a.Id.Equals(id));
        }

        public static Race? QueryRaceInData(string raceId) => ListOfRaces.Find(c => c.Id.Equals(raceId))?.Clone();

        public static Scenario? QueryScenarioInData(string scenarioId) => ListOfScenarios.Find(c => c.Id.Equals(scenarioId));

        public static BodyPlan? QueryBpPlanInData(string bpPlanId) => ListOfBpPlan.Find(c => c.Id?.Equals(bpPlanId) == true);

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

        public static Language? QueryLanguageInData(string languageId) => ListOfLanguages.Find(i => i.Id.Equals(languageId));

        public static Profession? QueryProfessionInData(string professionId) => ListOfProfessions.Find(i => i.Id.Equals(professionId));

        public static ShapeDescriptor? QueryShapeDescInData(string shapeId) => ListOfShapes.Find(i => i.Id.Equals(shapeId));

        public static CultureTemplate? QueryCultureTemplateInData(string cultureId) => ListOfCultures.Find(i => i.Id.Equals(cultureId));

        public static List<CultureTemplate>? QueryCultureTemplateFromBiome(string biomeId) => ListOfCultures.FindAll(i => i.StartBiome.Equals(biomeId));

        public static Research? QueryResearchInData(string researchId) => ListOfResearches.Find(i => i.Id.Equals(researchId));

        public static Plant? QueryPlantInData(string plantId) => ListOfPlants.Find(i => i.Id.Equals(plantId))?.Clone();

        public static TissuePlanTemplate? QueryTissuePlanInData(string tissuePlanId) => ListOfTissuePlans.Find(i => i.Id.Equals(tissuePlanId));

        public static DamageType? QueryDamageInData(string dmgId) => ListOfDamageTypes.Find(i => i.Id.Equals(dmgId));

        #endregion Query

        #region rng

        public static Language? RandomLangugage() => ListOfLanguages.GetRandomItemFromList();

        public static string? RandomRealm() => ListOfRealmsName.GetRandomItemFromList();

        public static Race? RandomRace() => ListOfRaces.GetRandomItemFromList();

        public static Research? RandomMagicalResearch() => ListOfResearches.FindAll(i => i.IsMagical).GetRandomItemFromList();

        public static Research? RandomNonMagicalResearch() => ListOfResearches.FindAll(i => !i.IsMagical).GetRandomItemFromList();

        #endregion rng

        #region helper methods

        public static List<Reaction> GetProductsByTag(RoomTag tag)
        {
            return ListOfReactions.FindAll(i => i.RoomTag.Contains(tag));
        }

        public static Material? QueryMaterialWithType(MaterialType typeToMake) => ListOfMaterials.FindAll(i => i.Type == typeToMake).GetRandomItemFromList();

        public static Material? QueryMaterialWithTrait(Trait trait) => ListOfMaterials?.FindAll(i => i.ConfersTraits?.Contains(trait) == true).GetRandomItemFromList();

        #endregion helper methods
    }
}
