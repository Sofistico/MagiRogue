using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Utils.Extensions
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
    }
}