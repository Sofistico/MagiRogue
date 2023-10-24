using System;

namespace MagusEngine.Utils.Noise.AccidentalNoiseLibrary
{
    public static class Utilities
    {
        public static double Clamp(double value, double low, double high)
        {
            if (value < low) return low;
            if (value > high) return high;
            return value;
        }

        public static int Clamp(int value, int low, int high)
        {
            if (value < low) return low;
            if (value > high) return high;
            return value;
        }

        private static double logPointFive = Math.Log(0.5);
        public static double Bias(double bias, double target)
        {
            return Math.Pow(target, Math.Log(bias) / logPointFive);
        }

        public static double Lerp(double t, double a, double b)
        {
            return a + t * (b - a);
        }

        public static double Gain(double g, double t)
        {
            if (t < 0.50) return Bias(1.00 - g, 2.00 * t) / 2.00;
            return 1.00 - Bias(1.00 - g, 2.00 - 2.00 * t) / 2.00;
        }

        public static double QuinticBlend(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
    }
}
