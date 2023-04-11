using System;

namespace MagiRogue.Utils
{
    public static class MathMagi
    {
        public static float ReturnPositive(float nmb)
        {
            float positive = nmb < 0 ? nmb * -1 : nmb;
            return positive;
        }

        public static double ReturnPositive(double nmb)
        {
            double positive = nmb < 0 ? nmb * -1 : nmb;
            return positive;
        }

        public static int ReturnPositive(int nmb)
        {
            return nmb < 0 ? nmb * -1 : nmb;
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

        public static int GetVolumeOfObject(double densityInGCm, double weightInKg)
        {
            int densityKgM = GetDensityInKgM(densityInGCm);
            return (int)(weightInKg / densityKgM);
        }

        public static double GetWeightWithDensity(double densityInGCm, int volumeInCM3)
        {
            int densityKgM = GetDensityInKgM(densityInGCm);
            return (double)((densityKgM * volumeInCM3) / (double)1000000);
        }

        public static int GetDensityInKgM(double densityInGCm)
        {
            return (int)(densityInGCm * 1000);
        }

        public static int CalculateVolumeWithModifier(int modifier, int volume)
        {
            return (int)(volume * (modifier / (double)100));
        }

        public static long GetTickByYear(int yearToGameBegin)
        {
            return yearToGameBegin * 3214080000;
        }

        public static double GetPercentageBasedOnMax(int current, int max)
        {
            if (current == 0)
                return 100;
            var val = (double)(current / (double)max * 100);
            if (val >= 0)
            {
                return val + 0.5d;
            }
            return val - 0.5d;
            //return 100 - val;
        }

        public static double GetPercentageBasedOnMax(double current, double max)
        {
            if (current == 0)
                return 100;
            var val = (double)(current / (double)max * 100);
            if (val >= 0)
            {
                return val + 0.5d;
            }
            return val - 0.5d;
        }
    }
}