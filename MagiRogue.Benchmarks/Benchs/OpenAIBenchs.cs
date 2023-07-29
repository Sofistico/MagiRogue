using BenchmarkDotNet.Attributes;
using SadRogue.Primitives;

namespace MagiRogue.Benchmarks.Benchs
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [ExceptionDiagnoser]
    [Config(typeof(AntiVirusFriendlyConfig))]
    public class OpenAIBenchs
    {
        [Benchmark]
        public void CompareResultOne()
        {
            int centerX = 10; // replace with the x-coordinate of the center point
            int centerY = 10; // replace with the y-coordinate of the center point
            int radius = 5; // replace with the radius of the circle

            List<Point> pointsInsideCircle = new List<Point>();

            for (double x = centerX - radius; x <= centerX + radius; x += 0.1)
            {
                for (double y = centerY - radius; y <= centerY + radius; y += 0.1)
                {
                    double distance = Math.Sqrt(((x - centerX) * (x - centerX)) + ((y - centerY) * (y - centerY)));
                    if (distance <= radius)
                    {
                        pointsInsideCircle.Add(new Point((int)x, (int)y));
                    }
                }
            }
        }

        [Benchmark]
        public void CompareResultTwo()
        {
            int centerX = 10; // replace with the x-coordinate of the center point
            int centerY = 10; // replace with the y-coordinate of the center point
            int radius = 5; // replace with the radius of the circle

            List<Point> pointsInsideCircle = new List<Point>();

            for (int angle = 0; angle < 360; angle += 1)
            {
                double x = centerX + (radius * Math.Cos(angle));
                double y = centerY + (radius * Math.Sin(angle));

                pointsInsideCircle.Add(new Point((int)x, (int)y));
            }
        }

        [Benchmark]
        public void CompareResultThree()
        {
            int centerX = 10; // replace with the x-coordinate of the center point
            int centerY = 10; // replace with the y-coordinate of the center point
            int radius = 5; // replace with the radius of the circle

            List<Point> pointsInsideCircle = new List<Point>();

            for (int angle = 0; angle < 360; angle++)
            {
                double x = centerX + (radius * Math.Cos(angle));
                double y = centerY + (radius * Math.Sin(angle));

                pointsInsideCircle.Add(new Point((int)x, (int)y));
            }
        }
    }
}
