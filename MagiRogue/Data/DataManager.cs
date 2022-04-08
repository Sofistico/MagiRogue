﻿using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using MagiRogue.System.Magic;
using MagiRogue.System.Tiles;
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
        private static readonly string _appDomain = AppDomain.CurrentDomain.BaseDirectory;

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

        public static readonly IReadOnlyList<TileBase> ListOfTiles =
            GetSourceTree<TileBase>(@".\Data\Tiles\*.json");

        private static IReadOnlyList<T> GetSourceTree<T>(string wildCard)
        {
            string originalWildCard = wildCard;

            string pattern = Path.GetFileName(originalWildCard);
            string realDir = originalWildCard[..^pattern.Length];

            // Get absolutepath
            string absPath = Path.GetFullPath(Path.Combine(_appDomain, realDir));

            string[] files = Directory.GetFiles(absPath, pattern, SearchOption.TopDirectoryOnly);

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

        public static TileBase QueryTileInData(string tileName)
            => ListOfTiles.FirstOrDefault(t => t.Name.Equals(tileName)).Copy();

        public static Actor QueryActorInData(string actorId)
            => ListOfActors.FirstOrDefault(a => a.ID.Equals(actorId));

        public static Item QueryItemInData(string itemId)
            => ListOfItems.FirstOrDefault(i => i.Id.Equals(itemId));
    }
}