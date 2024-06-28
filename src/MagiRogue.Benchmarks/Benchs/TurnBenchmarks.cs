using BenchmarkDotNet.Attributes;

namespace MagiRogue.Benchmarks.Benchs
{
    [MemoryDiagnoser]
    [Config(typeof(AntiVirusFriendlyConfig))]
    public class TurnBenchmarks
    {
        [Benchmark]
        public void PassTurn()
        {
        }

        [Benchmark]
        public void Pass10Turns()
        {
        }
    }
}
