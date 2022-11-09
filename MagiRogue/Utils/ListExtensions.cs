using MagiRogue.GameSys.Planet.History;
using ShaiRandom.Distributions.Continuous;
using System.Collections.Generic;
using System.Linq;
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
            if (list.Count < 1)
                return default;
            int rng = GameLoop.GlobalRand.NextInt(list.Count);
            return list[rng];
        }

        public static T GetRandomItemFromList<T>(this IReadOnlyList<T> list)
        {
            int rng = GameLoop.GlobalRand.NextInt(list.Count);
            return list[rng];
        }

        public static void ShuffleAlgorithm<T>(this List<T> enumerable)
        {
            int n = enumerable.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = GameLoop.GlobalRand.NextInt(i + 1);
                (enumerable[j], enumerable[i]) = (enumerable[i], enumerable[j]);
            }
        }

        public static List<T> ShuffleAlgorithmAndTakeN<T>(this List<T> values, int howManyToTake)
        {
            List<T> list = new List<T>(values);

            list.ShuffleAlgorithm();

            return list.Take<T>(howManyToTake).ToList();
        }
    }
}