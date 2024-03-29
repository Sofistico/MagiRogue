﻿using System;

namespace MagusEngine.Utils
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

        public static double FastRound(double val)
        {
            if (val >= 0)
            {
                return val + 0.5d > 100 ? 100 : val + 0.5d;
            }
            return val - 0.5d;
        }

        public static int GetVolumeOfObject(double densityInGCm, double weightInKg)
        {
            int densityKgM = GetDensityInKgM(densityInGCm);
            return (int)(weightInKg / densityKgM);
        }

        public static double GetWeightWithDensity(double densityInGCm, int volumeInCM3)
        {
            int densityKgM = GetDensityInKgM(densityInGCm);
            return (double)(densityKgM * volumeInCM3 / (double)1000000);
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

        public static double GetInversePercentageBasedOnMax(int current, int max)
        {
            if (current == 0)
                return 100;
            var val = (double)(100 - (current / (double)max * 100));
            return FastRound(val);
        }

        public static double GetPercentageBasedOnMax(double current, double max)
        {
            if (current == 0)
                return 100;
            var val = (double)(current / (double)max * 100);
            return FastRound(val);
        }

        /// <summary>
        /// Gets a value representing the percentage of a given number
        /// </summary>
        /// <param name="number">The number</param>
        /// <param name="percentageToGet">The percentage, should be given in fractions</param>
        /// <returns>Returns the double representing the percentage</returns>
        public static double GetSimplePercentage(int number, double percentageToGet)
        {
            if (percentageToGet <= 0)
                return 0;
            return number * percentageToGet;
        }
    }
}
