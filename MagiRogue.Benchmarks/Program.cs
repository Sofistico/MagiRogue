﻿using BenchmarkDotNet.Running;
using MagiRogue.Benchmarks.Benchs;

namespace MagiRogue.Benchmarks
{
    public static class Program
    {
        private static readonly string[] names = new[]
        {
            "Turn",
            "Map",
            "Save"
        };

        private static void Main()
        {
            Console.WriteLine("Choose your benchmark:");
            int res;

            for (int i = 0; i < names.Length; i++)
            {
                Console.WriteLine($"{i} - {names[i]}");
            }

            while (!DealWithInput(out res))
            {
                Console.WriteLine("Put only the specified numbers!");
            }

            switch (res)
            {
                case 0:
                    BenchmarkRunner.Run<TurnBenchmarks>();
                    break;

                case 1:
                    BenchmarkRunner.Run<MapBenchmarks>();
                    break;

                case 2:
                    BenchmarkRunner.Run<SaveBenchs>();
                    break;

                default:
                    break;
            }
        }

        private static bool DealWithInput(out int res)
        {
            var val = int.TryParse(Console.ReadLine(), out res);
            return val && res <= names.Length;
        }
    }
}