using BenchmarkDotNet.Attributes;
using MagiRogue.Data;
using MagiRogue.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Benchmarks.Benchs
{
    [MemoryDiagnoser]
    [Config(typeof(AntiVirusFriendlyConfig))]
    public class SaveBenchs
    {
        public static Actor Playa = Player.TestPlayer();
        public static string Json = JsonConvert.SerializeObject(Playa);

        [Benchmark]
        public void SavePlaya()
        {
            _ = JsonConvert.SerializeObject(Playa);
        }

        [Benchmark]
        public void Save10Playa()
        {
            for (int i = 0; i < 10; i++)
            {
                _ = JsonConvert.SerializeObject(Playa);
            }
        }

        [Benchmark]
        public void Save100Playa()
        {
            for (int i = 0; i < 100; i++)
            {
                _ = JsonConvert.SerializeObject(Playa);
            }
        }

        [Benchmark]
        public static void SavePlayaStatic()
        {
            _ = JsonConvert.SerializeObject(Playa);
        }

        [Benchmark]
        public static void Save10PlayaStatic()
        {
            for (int i = 0; i < 10; i++)
            {
                _ = JsonConvert.SerializeObject(Playa);
            }
        }

        [Benchmark]
        public static void Save100PlayaStatic()
        {
            for (int i = 0; i < 100; i++)
            {
                _ = JsonConvert.SerializeObject(Playa);
            }
        }

        [Benchmark]
        public void LoadPlaya()
        {
            var asa = JsonConvert.DeserializeObject<Actor>(Json);
        }

        [Benchmark]
        public static void LoadPlayaStatic()
        {
            var asa = JsonConvert.DeserializeObject<Actor>(Json);
        }

        [Benchmark]
        public void Load10Playa()
        {
            for (int i = 0; i < 10; i++)
            {
                var asa = JsonConvert.DeserializeObject<Actor>(Json);
            }
        }

        [Benchmark]
        public static void Load10PlayaStatic()
        {
            for (int i = 0; i < 10; i++)
            {
                var asa = JsonConvert.DeserializeObject<Actor>(Json);
            }
        }

        [Benchmark]
        public void Load100Playa()
        {
            for (int i = 0; i < 100; i++)
            {
                var asa = JsonConvert.DeserializeObject<Actor>(Json);
            }
        }

        [Benchmark]
        public static void Load100PlayaStatic()
        {
            for (int i = 0; i < 100; i++)
            {
                var asa = JsonConvert.DeserializeObject<Actor>(Json);
            }
        }
    }
}
