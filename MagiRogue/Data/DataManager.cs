﻿using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.Entities;
using MagiRogue.Entities.StarterScenarios;
using MagiRogue.GameSys.Magic;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Data
{
    public static class DataManager
    {
        public static readonly IReadOnlyList<ItemTemplate> ListOfItems =
            GetSourceTree<ItemTemplate>(@".\Data\Items\*.json");

        public static readonly IReadOnlyList<MaterialTemplate> ListOfMaterials =
           GetSourceTree<MaterialTemplate>(@".\Data\Materials\*.json");

        public static readonly IReadOnlyList<SpellBase> ListOfSpells =
            GetSourceTree<SpellBase>(@".\Data\Spells\spells_*.json");

        public static readonly IReadOnlyList<Organ> ListOfOrgans =
            GetSourceTree<Organ>(@".\Data\Bodies\organs_*.json");

        public static readonly IReadOnlyList<Limb> ListOfLimbs =
            GetSourceTree<Limb>(@".\Data\Bodies\limb_*.json");

        public static readonly IReadOnlyList<BasicTile> ListOfTiles =
            GetSourceTree<BasicTile>(@".\Data\Tiles\*.json");

        public static readonly IReadOnlyList<Furniture> ListOfFurnitures =
            GetSourceTree<Furniture>(@".\Data\Furniture\*.json");

        public static readonly IReadOnlyList<RoomTemplate> ListOfRooms =
            GetSourceTree<RoomTemplate>(@".\Data\Rooms\*.json");

        // races can be dynamically generated ingame
        public static readonly List<Race> ListOfRaces =
            GetSourceTree<Race>(@".\Data\Races\race_*.json").ToList();

        public static readonly IReadOnlyList<Scenario> ListOfScenarios =
            GetSourceTree<Scenario>(@".\Data\Scenarios\scenarios_*.json");

        public static readonly IReadOnlyList<BodyPlan> ListOfBpPlan =
            GetSourceTree<BodyPlan>(@".\Data\Bodies\body_*.json");

        public static readonly IReadOnlyList<Language> ListOfLanguages =
            GetSourceTree<Language>(@".\Data\Language\language_*.json");

        public static readonly IReadOnlyList<Profession> ListOfProfessions =
            GetSourceTree<Profession>(@".\Data\Professions\profession_*.json");

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

        public static IReadOnlyList<T> GetSourceTree<T>(string wildCard)
        {
            string[] files = FileUtils.GetFiles(wildCard);

            List<List<T>> listTList = new();

            for (int i = 0; i < files.Length; i++)
            {
                listTList.Add(JsonUtils.JsonDeseralize<List<T>>(files[i]));
            }
            List<T> allTList = new();

            foreach (List<T> tList in listTList)
            {
                foreach (T t in tList)
                {
                    allTList.Add(t);
                }
            }

            IReadOnlyList<T> readOnlyList = allTList.AsReadOnly();

            return readOnlyList;
        }

        public static SpellBase QuerySpellInData(string spellId) => ListOfSpells.FirstOrDefault
                (m => m.SpellId.Equals(spellId)).Copy();

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
            => ListOfTiles.FirstOrDefault(t => t.TileId.Equals(tileId)).Copy();

        public static T QueryTileInData<T>(string tileId) where T : TileBase
            => (T)ListOfTiles.FirstOrDefault(t => t.TileId.Equals(tileId)).Copy();

        public static T QueryTileInData<T>(string tileId, Point pos) where T : TileBase
            => (T)ListOfTiles.FirstOrDefault(t => t.TileId.Equals(tileId)).Copy(pos);

        public static Item QueryItemInData(string itemId)
            => ListOfItems.FirstOrDefault(i => i.Id.Equals(itemId));

        public static Furniture QueryFurnitureInData(string furnitureId)
            => ListOfFurnitures.FirstOrDefault(i => i.FurId.Equals(furnitureId)).Copy();

        public static RoomTemplate QueryRoomInData(string roomId)
            => ListOfRooms.FirstOrDefault(i => i.Id.Equals(roomId));

        public static MaterialTemplate QueryMaterial(string id)
            => ListOfMaterials.FirstOrDefault(a => a.Id.Equals(id));

        public static List<BasicTile> QueryTilesInDataWithTrait(Trait trait)
            => ListOfTiles.Where(i => i.HasAnyTrait()
                && i.Traits.Contains(trait)).ToList();

        public static Race QueryRaceInData(string raceId)
            => ListOfRaces.Where(c => c.Id.Equals(raceId)).FirstOrDefault();

        public static Scenario QueryScenarioInData(string scenarioId)
            => ListOfScenarios.Where(c => c.Id.Equals(scenarioId)).FirstOrDefault();

        public static BodyPlan QueryBpPlanInData(string bpPlanId)
            => ListOfBpPlan.Where(c => c.Id.Equals(bpPlanId)).FirstOrDefault();

        public static List<BodyPart> QueryBpsPlansInDataAndReturnBodyParts(string[] bpPlansId)
        {
            List<BodyPart> bps = new();
            BodyPlan bodyPlan = new();
            foreach (var ids in bpPlansId)
            {
                BodyPlan bp = QueryBpPlanInData(ids);
                bodyPlan.BodyParts.AddRange(bp.BodyParts);
            }
            bps.AddRange(bodyPlan.ReturnBodyParts());

            return bps;
        }

        public static Language QueryLanguageInData(string languageId)
            => ListOfLanguages.FirstOrDefault(i => i.Id.Equals(languageId));

        public static Profession QueryProfessionInData(string professionId)
            => ListOfProfessions.FirstOrDefault(i => i.Id.Equals(professionId));

        public static ShapeDescriptor QueryShapeDescInData(string shapeId)
            => ListOfShapes.FirstOrDefault(i => i.Id.Equals(shapeId));

        public static Language RandomLangugage()
            => ListOfLanguages.GetRandomItemFromList();

        public static string RandomRealm()
            => ListOfRealmsName.GetRandomItemFromList();

        public static Race RandomRace()
            => ListOfRaces.GetRandomItemFromList();
    }
}