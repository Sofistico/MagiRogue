using BenchmarkDotNet.Attributes;
using MagusEngine.Core.MapStuff;
using MagusEngine.Generators.MapGen;

namespace MagiRogue.Benchmarks.Benchs
{
    [MemoryDiagnoser]
    [ThreadingDiagnoser]
    [ExceptionDiagnoser]
    [Config(typeof(AntiVirusFriendlyConfig))]
    public class MapBenchmarks
    {
        [Benchmark]
        public void Get1TestMapWithStuff()
        {
            var map = new MiscMapGen().GenerateTestMap();
        }

        [Benchmark]
        public void Get1TestMapWithoutStuff()
        {
            var map = new Map("Map map", 180, 180);
        }

        //[Benchmark]
        //public void Get10TestMap()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        var map = new MiscMapGen().GenerateTestMap();
        //    }
        //}

        [Benchmark]
        public void Get1TestMapGoRogue()
        {
            var map = new GoRogue.GameFramework.Map(180, 180, 6, SadRogue.Primitives.Distance.Chebyshev);
        }

        //[Benchmark]
        //public void Get100TestMap()
        //{
        //    for (int i = 0; i < 100; i++)
        //    {
        //        var map = new MiscMapGen().GenerateTestMap();
        //    }
        //}
    }
}
