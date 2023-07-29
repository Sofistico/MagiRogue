using BenchmarkDotNet.Attributes;
using MagiRogue.GameSys;
using MagiRogue.GameSys.MapGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
