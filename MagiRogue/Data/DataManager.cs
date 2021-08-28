using MagiRogue.Data.Materials;
using MagiRogue.System.Magic;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MagiRogue.Data
{
    public static class DataManager
    {
        private readonly static string _appDomain = AppDomain.CurrentDomain.BaseDirectory;

        public readonly static IReadOnlyList<ItemTemplate> ListOfItems =
            GetSourceTree<ItemTemplate>(@".\Data\Items\*.json");

        public readonly static IReadOnlyList<MaterialTemplate> ListOfMaterials =
           GetSourceTree<MaterialTemplate>(@".\Data\Materials\*.json");

        public readonly static IReadOnlyList<SpellBase> ListOfSpells = GetSourceTree<SpellBase>(@".\Data\Spells\*.json");

        public readonly static IReadOnlyList<ActorTemplate> ListOfActors = GetSourceTree<ActorTemplate>(@".\Data\Actors\*.json");

        private static IReadOnlyList<T> GetSourceTree<T>(string wildCard)
        {
            string originalWildCard = wildCard;

            string pattern = Path.GetFileName(originalWildCard);
            string realDir = originalWildCard.Substring(0, originalWildCard.Length - pattern.Length);

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
                (m => m.SpellId.Equals(spellId));
    }
}