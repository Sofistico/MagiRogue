using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Arquimedes.Data;
using Arquimedes.Enumerators;
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

namespace MagusEngine.Systems
{
    public static class DataManager
    {
        #region fields

        private static bool _firstLoad = true;

        #endregion fields

        #region jsons

        public static KeyedDataRepository<ItemTemplate> ListOfItems { get; } = new(@"./Data/Items/items_*");

        public static KeyedDataRepository<Material> ListOfMaterials { get; } = new(@"./Data/Materials/material_*");

        public static KeyedDataRepository<Spell> ListOfSpells { get; } = new(@"./Data/Spells/spells_*");

        public static KeyedDataRepository<Organ> ListOfOrgans { get; } = new(@"./Data/Bodies/organs_*");

        public static KeyedDataRepository<Limb> ListOfLimbs { get; } = new(@"./Data/Bodies/limbs_*");

        public static KeyedDataRepository<Furniture> ListOfFurnitures { get; } = new(@"./Data/Furniture/fur_*");

        public static KeyedDataRepository<RoomTemplate> ListOfRooms { get; } = new(@"./Data/Rooms/room_*");

        // races can be dynamically generated ingame
        public static KeyedDataRepository<Race> ListOfRaces { get; } = new(@"./Data/Races/race_*");

        public static KeyedDataRepository<Scenario> ListOfScenarios { get; } = new(@"./Data/Scenarios/scenarios_*");

        public static KeyedDataRepository<BodyPlan> ListOfBpPlan { get; } = new(@"./Data/Bodies/bodies_*");

        public static KeyedDataRepository<Language> ListOfLanguages { get; } = new(@"./Data/Language/language_*");

        public static KeyedDataRepository<Profession> ListOfProfessions { get; } = new(@"./Data/Professions/profession_*");

        public static KeyedDataRepository<CultureTemplate> ListOfCultures { get; } = new(@"./Data/Cultures/cultures_*");

        public static KeyedDataRepository<Research> ListOfResearches { get; } = new(@"./Data/Research/research_*");

        public static KeyedDataRepository<Reaction> ListOfReactions { get; } = new(@"./Data/Reaction/reaction_*");

        public static ListDataRepository<Ruleset> ListOfRules { get; } = new(@"./Data/Rules/rules_*");

        public static KeyedDataRepository<Plant> ListOfPlants { get; } = new(@"./Data/Plant/plant_*");

        public static KeyedDataRepository<TissuePlanTemplate> ListOfTissuePlans { get; } = new(@"./Data/Bodies/tissue_*");

        public static KeyedDataRepository<DamageType> ListOfDamageTypes { get; } = new(@"./Data/Damage/dmg_*");

        public static KeyedDataRepository<AnimationBase> ListOfAnimations { get; } = new(@"./Data/Animations/*_animations*");

        #region Descriptors

        public static ListDataRepository<string> ListOfRealmsName { get; } = new ListDataRepository<string>(@"./Data/Descriptors/realms_*");

        public static ListDataRepository<string> ListOfMagicFounts { get; } = new ListDataRepository<string>(@"./Data/Descriptors/fount_*");

        public static ListDataRepository<string> ListOfAdjectives { get; } = new ListDataRepository<string>(@"./Data/Descriptors/adjectives_*");

        public static KeyedDataRepository<ShapeDescriptor> ListOfShapes { get; } = new KeyedDataRepository<ShapeDescriptor>(@"./Data/Descriptors/shapes_*");

        #endregion Descriptors

        #endregion jsons

        #region Query

        public static Spell? QuerySpellInData(string spellId, double proficiency = 0)
        {
            return ListOfSpells.Query(spellId)?.Copy(proficiency);
        }

        public static Limb? QueryLimbInData(string limbId)
        {
            return ListOfLimbs.Query(limbId)?.Copy();
        }

        public static Organ? QueryOrganInData(string organId)
        {
            return ListOfOrgans.Query(organId)?.Copy();
        }

        public static Item? QueryItemInData(string itemId)
        {
            return ListOfItems.Query(itemId);
        }

        public static Item? QueryItemInData(string itemId, Material material) => QueryItemInData(itemId)?.ConfigureMaterial(material);

        public static Item? QueryItemInData(string itemId, Material material, Point point) => QueryItemInData(itemId)?.ConfigureMaterial(material).ConfigurePoint(point);

        public static Furniture? QueryFurnitureInData(string furnitureId)
        {
            return ListOfFurnitures.Query(furnitureId)?.Copy();
        }

        public static RoomTemplate? QueryRoomInData(string roomId)
        {
            return ListOfRooms.Query(roomId);
        }

        public static Material? QueryMaterial(string id)
        {
            if (_firstLoad)
            {
                _firstLoad = false;
                var list = ListOfMaterials.GetEnumerableCollection().Where(static i => !string.IsNullOrEmpty(i.InheirtFrom)).ToArray();
                for (int i = 0; i < list.Length; i++)
                {
                    var mat = list[i];
                    if (mat is null)
                        continue;
                    Material? inheirtFrom = ListOfMaterials.Query(mat.InheirtFrom!);
                    if (inheirtFrom is null)
                    {
                        Locator.GetService<MagiLog>().Log($"Material to inheirt from was null! Id: {mat.InheirtFrom}");
                        continue;
                    }

                    inheirtFrom.CopyTo(mat);
                }
            }
            return ListOfMaterials.Query(id);
        }

        public static Race? QueryRaceInData(string raceId)
        {
            return ListOfRaces.Query(raceId);
        }

        public static Scenario? QueryScenarioInData(string scenarioId)
        {
            return ListOfScenarios.Query(scenarioId);
        }

        public static BodyPlan? QueryBpPlanInData(string bpPlanId)
        {
            return ListOfBpPlan.Query(bpPlanId);
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
            return ListOfLanguages.Query(languageId);
        }

        public static Profession? QueryProfessionInData(string professionId)
        {
            return ListOfProfessions.Query(professionId);
        }

        public static ShapeDescriptor? QueryShapeDescInData(string shapeId)
        {
            return ListOfShapes.Query(shapeId);
        }

        public static CultureTemplate? QueryCultureTemplateInData(string cultureId)
        {
            return ListOfCultures.Query(cultureId);
        }

        public static List<CultureTemplate>? QueryCultureTemplateFromBiome(string biomeId) => [.. ListOfCultures.GetEnumerableCollection().Where(i => i.StartBiome.Equals(biomeId))];

        public static Research? QueryResearchInData(string researchId)
        {
            return ListOfResearches.Query(researchId);
        }

        public static Plant? QueryPlantInData(string plantId)
        {
            return ListOfPlants.Query(plantId);
        }

        public static TissuePlanTemplate? QueryTissuePlanInData(string tissuePlanId)
        {
            return ListOfTissuePlans.Query(tissuePlanId);
        }

        public static DamageType? QueryDamageInData(string dmgId)
        {
            return ListOfDamageTypes.Query(dmgId);
        }

        public static AnimationBase? QueryAnimationInData(string animationId)
        {
            return ListOfAnimations.Query(animationId);
        }

        #endregion Query

        #region rng

        public static Language? RandomLangugage() => ListOfLanguages.GetEnumerableCollection().GetRandomItemFromCollection();

        public static string? RandomRealm() => ListOfRealmsName.GetEnumerableCollection().GetRandomItemFromCollection();

        public static Race? RandomRace() => ListOfRaces.GetEnumerableCollection().GetRandomItemFromCollection();

        public static Research? RandomMagicalResearch() => ListOfResearches.GetEnumerableCollection().Where(static i => i.IsMagical).GetRandomItemFromCollection();

        public static Research? RandomNonMagicalResearch() => ListOfResearches.GetEnumerableCollection().Where(static i => !i.IsMagical).GetRandomItemFromCollection();

        #endregion rng

        #region helper methods

        public static List<Reaction> GetProductsByTag(RoomTag tag) => [.. ListOfReactions.GetEnumerableCollection().Where(i => i.RoomTag.Contains(tag))];

        public static Material? QueryMaterialWithType(MaterialType typeToMake) => ListOfMaterials.GetEnumerableCollection().Where(i => i.Type == typeToMake).GetRandomItemFromCollection();

        public static Material? QueryMaterialWithTrait(Trait trait) => ListOfMaterials?.GetEnumerableCollection().Where(i => i.ConfersTraits?.Contains(trait) == true).GetRandomItemFromCollection();

        #endregion helper methods
    }
}
