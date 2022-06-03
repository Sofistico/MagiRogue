using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Utils
{
    public static class MathMagi
    {
        public static float ReturnPositive(float nmb)
        {
            float positive;
            if (nmb < 0)
                positive = nmb * -1;
            else
                positive = nmb;
            return positive;
        }

        /// <summary>
        /// Method to return only the positive module.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int Mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        public static double Round(double x)
        {
            return Math.Round(x, 2, MidpointRounding.AwayFromZero);
        }

        public static float Round(float x)
        {
            return MathF.Round(x, 2, MidpointRounding.AwayFromZero);
        }
    }
}