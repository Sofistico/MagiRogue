using System.Collections.Generic;
using System.Resources;

namespace MagiRogue.Utils
{
    public static class ListExtensions
    {
        public static List<T> ReturnListListTAsListT<T>(this List<List<T>> list)
        {
            var objList = new List<T>();
            foreach (var subList in list)
            {
                foreach (var item in subList)
                {
                    objList.Add(item);
                }
            }
            return objList;
        }

        public static List<List<T>> ReturnListAsTListT<T>(this List<T> list)
        {
            var objList = new List<List<T>>();
            foreach (var item in list)
            {
                var listItem = new List<T>()
                {
                    item
                };
                objList.Add(listItem);
            }

            return objList;
        }

        public static T GetRandomItemFromList<T>(this List<T> list)
        {
            int rng = GameLoop.GlobalRand.NextInt(list.Count);
            return list[rng];
        }

        public static T GetRandomItemFromList<T>(this IReadOnlyList<T> list)
        {
            int rng = GameLoop.GlobalRand.NextInt(list.Count);
            return list[rng];
        }
    }
}