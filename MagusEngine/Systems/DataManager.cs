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
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Systems
{
    public static class DataManager
    {
        // TODO: change all IReadOnlyList to dictionaries! and make it all private
        private static bool firstLoad = true;

        #region jsons

        public static readonly IReadOnlyList<ItemTemplate> ListOfItems =
            GetSourceTree<ItemTemplate>(@".\Data\Items\items_*");

        public static readonly List<MaterialTemplate> ListOfMaterials =
            GetSourceTree<MaterialTemplate>(@".\Data\Materials\material_*");

        public static readonly IReadOnlyList<SpellBase> ListOfSpells =
            GetSourceTree<SpellBase>(@".\Data\Spells\spells_*");

        public static readonly IReadOnlyList<Organ> ListOfOrgans =
            GetSourceTree<Organ>(@".\Data\Bodies\organs_*");

        public static readonly IReadOnlyList<Limb> ListOfLimbs =
            GetSourceTree<Limb>(@".\Data\Bodies\limbs_*");

        public static readonly IReadOnlyList<Furniture> ListOfFurnitures =
            GetSourceTree<Furniture>(@".\Data\Furniture\fur_*");

        public static readonly IReadOnlyList<RoomTemplate> ListOfRooms =
            GetSourceTree<RoomTemplate>(@".\Data\Rooms\room_*");

        // races can be dynamically generated ingame
        public static readonly IReadOnlyList<Race> ListOfRaces =
            GetSourceTree<Race>(@".\Data\Races\race_*");

        public static readonly IReadOnlyList<Scenario> ListOfScenarios =
            GetSourceTree<Scenario>(@".\Data\Scenarios\scenarios_*");

        public static readonly IReadOnlyList<BodyPlan> ListOfBpPlan =
            GetSourceTree<BodyPlan>(@".\Data\Bodies\bodies_*.json");

        public static readonly IReadOnlyList<Language> ListOfLanguages =
            GetSourceTree<Language>(@".\Data\Language\language_*");

        public static readonly IReadOnlyList<Profession> ListOfProfessions =
            GetSourceTree<Profession>(@".\Data\Professions\profession_*");

        public static readonly IReadOnlyList<CultureTemplate> ListOfCultures =
            GetSourceTree<CultureTemplate>(@".\Data\Cultures\cultures_*");

        public static readonly IReadOnlyList<Research> ListOfResearches =
            GetSourceTree<Research>(@".\Data\Research\research_*");

        public static readonly IReadOnlyList<Reaction> ListOfReactions =
            GetSourceTree<Reaction>(@".\Data\Reaction\reaction_*");

        public static readonly IReadOnlyList<Ruleset> ListOfRules =
            GetSourceTree<Ruleset>(@".\Data\Rules\rules_*");

        public static readonly IReadOnlyList<Plant> ListOfPlants =
            GetSourceTree<Plant>(@".\Data\Plant\plant_*");

        public static readonly IReadOnlyList<TissuePlanTemplate> ListOfTissuePlans =
            GetSourceTree<TissuePlanTemplate>(@".\Data\Bodies\tissue_*");

        public static readonly IReadOnlyList<DamageType> ListOfDamageTypes =
            GetSourceTree<DamageType>(@".\Data\Damage\dmg_*");

        #region Descriptors

        public static readonly IReadOnlyList<string> ListOfRealmsName =
            GetSourceTree<string>(@".\Data\Descriptors\realms_*.json");
        public static readonly IReadOnlyList<string> ListOfMagicFounts =
            GetSourceTree<string>(@".\Data\Descriptors\magic_fount_*.json");
        public static readonly IReadOnlyList<string> ListOfAdjectives =
            GetSourceTree<string>(@".\Data\Descriptors\adjectives_*.json");
        public static readonly IReadOnlyList<ShapeDescriptor> ListOfShapes =
            GetSourceTree<ShapeDescriptor>(@".\Data\Descriptors\shapes_*.json");

        #endregion Descriptors

        #endregion jsons

        public static IReadOnlyList<T> GetSourceTree<T>(string wildCard, Action<T>? executeOnList = null)
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

            if (executeOnList is not null)
            {
                allTList.ForEach(executeOnList);
            }

            return allTList.AsReadOnly();
        }

        public static List<T> GetSourceTree<T>(string wildCard)
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

            return allTList;
        }

        #region Query

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

        public static Item QueryItemInData(string itemId)
            => ListOfItems.FirstOrDefault(i => i.Id.Equals(itemId));

        public static Furniture QueryFurnitureInData(string furnitureId)
            => ListOfFurnitures.FirstOrDefault(i => i.FurId.Equals(furnitureId))?.Copy();

        public static RoomTemplate QueryRoomInData(string roomId)
            => ListOfRooms.FirstOrDefault(i => i.Id.Equals(roomId));

        public static MaterialTemplate QueryMaterial(string id)
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

        public static DamageType QueryDamageInData(string dmgId)
            => ListOfDamageTypes.FirstOrDefault(i => i.Id.Equals(dmgId));

        #endregion Query

        #region rng

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

        #endregion rng

        #region helper methods

        public static List<Reaction> GetProductsByTag(RoomTag tag)
        {
            return ListOfReactions.Where(i => i.RoomTag.Contains(tag)).ToList();
        }

        public static MaterialTemplate? QueryMaterialWithType(MaterialType typeToMake)
        {
            return ListOfMaterials.Where(i => i.Type == typeToMake).GetRandomItemFromList();
        }

        public static MaterialTemplate QueryMaterialWithTrait(Trait trait)
        {
            return ListOfMaterials.Where(i => i.ConfersTraits?.Contains(trait) == true).GetRandomItemFromList();
        }

        #endregion helper methods
    }
}
