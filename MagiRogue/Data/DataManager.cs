using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
            GetSourceTree<SpellBase>(@".\Data\Spells\*.json");

        public static readonly IReadOnlyList<ActorTemplate> ListOfActors =
            GetSourceTree<ActorTemplate>(@".\Data\Actors\*.json");

        public static readonly IReadOnlyList<Organ> ListOfOrgans =
            GetSourceTree<Organ>(@".\Data\Other\organs.json");

        public static readonly IReadOnlyList<LimbTemplate> ListOfLimbs =
            GetSourceTree<LimbTemplate>(@".\Data\Other\body_parts.json");

        public static readonly IReadOnlyList<BasicTile> ListOfTiles =
            GetSourceTree<BasicTile>(@".\Data\Tiles\*.json");

        public static readonly IReadOnlyList<Furniture> ListOfFurnitures =
            GetSourceTree<Furniture>(@".\Data\Furniture\*.json");

        public static readonly IReadOnlyList<RoomTemplate> ListOfRooms =
            GetSourceTree<RoomTemplate>(@".\Data\Rooms\*.json");

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

        public static LimbTemplate QueryLimbInData(string limbId) =>
            ListOfLimbs.FirstOrDefault(l => l.Id.Equals(limbId)).Copy();

        public static Organ QueryOrganInData(string organId)
            => ListOfOrgans.FirstOrDefault(o => o.Id.Equals(organId)).Copy();

        public static TileBase QueryTileInData(string tileId)
            => ListOfTiles.FirstOrDefault(t => t.TileId.Equals(tileId)).Copy();

        public static T QueryTileInData<T>(string tileId) where T : TileBase
            => (T)ListOfTiles.FirstOrDefault(t => t.TileId.Equals(tileId)).Copy();

        public static T QueryTileInData<T>(string tileId, Point pos) where T : TileBase
            => (T)ListOfTiles.FirstOrDefault(t => t.TileId.Equals(tileId)).Copy(pos);

        public static Actor QueryActorInData(string actorId)
            => ListOfActors.FirstOrDefault(a => a.ID.Equals(actorId));

        public static Item QueryItemInData(string itemId)
            => ListOfItems.FirstOrDefault(i => i.Id.Equals(itemId));

        public static Furniture QueryFurnitureInData(string furnitureId)
            => ListOfFurnitures.FirstOrDefault(i => i.FurId.Equals(furnitureId)).Copy();

        public static RoomTemplate QueryRoomInData(string roomId)
            => ListOfRooms.FirstOrDefault(i => i.Id.Equals(roomId));

        public static MaterialTemplate QueryMaterials(string id)
            => ListOfMaterials.FirstOrDefault(a => a.Id.Equals(id));
    }
}