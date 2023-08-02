using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Utils.Extensions
{
    public static class ArrayUtils
    {
        public static T[] Transform2DTo1D<T>(this T[,] t)
        {
            T[] values = new T[t.Length];
            for (int x = 0; x < t.GetLength(0); x++)
            {
                for (int y = 0; y < t.GetLength(1); y++)
                {
                    values[Point.ToIndex(x, y, t.GetLength(1))] = t[x, y];
                }
            }

            return values;
        }

        // created using https://swimburger.net/blog/dotnet/generic-quick-sort-in-csharp-dotnet
        public static void QuickSort<T>(ref T[] arr, int left, int right) where T : IComparable
        {
            if (left >= right)
            {
                return;
            }
            int par = Partition(ref arr, left, right);
            QuickSort(ref arr, left, par - 1);
            QuickSort(ref arr, par + 1, right);
        }

        private static int Partition<T>(ref T[] arr, int left, int right) where T : IComparable
        {
            T partion = arr[right];
            // stack items smaller than partition from left to right
            int swapIndex = left;
            for (int i = left; i < right; i++)
            {
                T item = arr[i];
                if (item.CompareTo(partion) <= 0)
                {
                    (arr[i], arr[swapIndex]) = (arr[swapIndex], item);
                    swapIndex++;
                }
            }

            // put the partition after all the smaller items
            (arr[right], arr[swapIndex]) = (arr[swapIndex], partion);

            return right;
        }
    }
}