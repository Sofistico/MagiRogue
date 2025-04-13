using BenchmarkDotNet.Attributes;
using MagusEngine.Systems;

namespace MagiRogue.Benchmarks.Benchs
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [ExceptionDiagnoser]
    [Config(typeof(AntiVirusFriendlyConfig))]
    public class DataBenchmarks
    {
        [Benchmark]
        public void DefaultDataManager()
        {
            var i = DataManager.QueryRaceInData("test_race");
        }
    }
}
