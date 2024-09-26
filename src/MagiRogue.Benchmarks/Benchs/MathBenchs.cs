using BenchmarkDotNet.Attributes;

namespace MagiRogue.Benchmarks.Benchs
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [ExceptionDiagnoser]
    [Config(typeof(AntiVirusFriendlyConfig))]
    public class MathBenchs
    {
        private static double GetPercentageBasedOnMaxOld(double current, double max)
        {
            if (current == 0)
                return 100;
            return (double)Math.Round((double)((double)current / (double)max * 100), 3, MidpointRounding.AwayFromZero);
        }

        private static double GetPercentageBasedOnMaxNew(int current, int max)
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

        [Benchmark]
        public static void GetPercentage1000BasedOnMaxOld()
        {
            for (int i = 0; i < 1000; i++)
            {
                var val = GetPercentageBasedOnMaxOld(1000, 1000);
            }
        }

        [Benchmark]
        public static void GetPercentage1000OldWithDiffers()
        {
            for (int i = 0; i < 1000; i++)
            {
                var val = GetPercentageBasedOnMaxOld(500, 1000);
            }
        }

        [Benchmark]
        public static void GetPercentage1000New()
        {
            for (int i = 0; i < 1000; i++)
            {
                var val = GetPercentageBasedOnMaxNew(1000, 1000);
            }
        }

        [Benchmark]
        public static void GetPercentage1000NewWithDiffers()
        {
            for (int i = 0; i < 1000; i++)
            {
                var val = GetPercentageBasedOnMaxNew(500, 1000);
            }
        }
    }
}
