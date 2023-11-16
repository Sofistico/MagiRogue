using GoRogue.Random;
using MagusEngine.Core.WorldStuff.History;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Utils.Extensions
{
    public static class ListExtensions
    {
        public static List<T> ReturnListListTAsListT<T>(this List<List<T>> list)
        {
            var objList = new List<T>();
            foreach (var subList in list)
            {
                if(subList is null)
                    continue;
                objList.AddRange(subList);
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

        public static T? GetRandomItemFromList<T>(this List<T> list)
        {
            if (list.Count < 1)
                return default;
            int rng = GlobalRandom.DefaultRNG.NextInt(list.Count);
            return list[rng];
        }

        public static T? GetRandomItemFromList<T>(this IEnumerable<T> enumerable)
        {
            if (!enumerable.Any())
                return default;
            _ = enumerable.TryGetNonEnumeratedCount(out int count);
            int rng = GlobalRandom.DefaultRNG.NextInt(count);
            return enumerable.ElementAt(rng);
        }

        public static T GetRandomItemFromList<T>(this IReadOnlyList<T> list)
        {
            int rng = GlobalRandom.DefaultRNG.NextInt(list.Count);
            return list[rng];
        }

        public static void ShuffleAlgorithm<T>(this List<T> enumerable)
        {
            int n = enumerable.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = GlobalRandom.DefaultRNG.NextInt(i + 1);
                (enumerable[j], enumerable[i]) = (enumerable[i], enumerable[j]);
            }
        }

        public static List<T> ShuffleAlgorithmAndTakeN<T>(this List<T> values, int howManyToTake)
        {
            List<T> list = new(values);

            list.ShuffleAlgorithm();

            return list.Take(howManyToTake).ToList();
        }

        public static List<Ruleset> AllFulfilled(this List<Ruleset> rules, HistoricalFigure figure)
        {
            var list = new List<Ruleset>();
            foreach (var rule in rules)
            {
                bool allFulfilled = false;
                if (rule.Triggers.Count > 0)
                {
                    int triggerCount = 0;
                    foreach (var item in rule.Triggers)
                    {
                        if (item.CheckIfFulfilled(figure))
                        {
                            triggerCount++;
                        }
                    }
                    allFulfilled = triggerCount >= rule.Triggers.Count;
                }
                if (allFulfilled)
                    list.Add(rule);
            }

            return list;
        }
    }
}