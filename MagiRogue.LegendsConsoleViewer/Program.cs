using GoRogue.DiceNotation.Terms;
using MagiRogue.Entities;
using MagiRogue.GameSys.Planet;
using Newtonsoft.Json;

namespace MagiRogue.LegendsConsoleViewer
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                PlanetGenerator planetGenerator = new PlanetGenerator();
                int yearsToGenerate = 10;
                var planet = planetGenerator.CreatePlanet(100, 100, yearsToGenerate, 15);
                List<SimpleJsonForVisualization> legends = new();
                List<string> stringToAdd = new();
                foreach (var site in planet.WorldHistory.AllSites)
                {
                    foreach (var item in site.SiteLegends)
                    {
                        Console.WriteLine(item.ToString());
                        stringToAdd.Add(item.ToString());
                    }
                    legends.Add(SimpleJsonForVisualization.Init(stringToAdd));
                }
                foreach (var civ in planet.WorldHistory.Civs)
                {
                    foreach (var item in civ.Legends)
                    {
                        Console.WriteLine(item.ToString());
                        stringToAdd.Add(item.ToString());
                    }
                    legends.Add(SimpleJsonForVisualization.Init(stringToAdd));
                }

                foreach (var hf in planet.WorldHistory.Figures)
                {
                    foreach (var item in hf.Legends)
                    {
                        Console.WriteLine(item.ToString());
                        stringToAdd.Add(item.ToString());
                    }
                    legends.Add(SimpleJsonForVisualization.Init(stringToAdd));
                }

                foreach (var item in planet.WorldHistory.ImportantItems)
                {
                    foreach (var legend in item.Legends)
                    {
                        Console.WriteLine(legend.ToString());
                        stringToAdd.Add(item.ToString());
                    }
                    legends.Add(SimpleJsonForVisualization.Init(stringToAdd));
                }

                foreach (var myth in planet.WorldHistory.Myths)
                {
                    Console.WriteLine(myth.ToString());
                    stringToAdd.Add(myth.ToString());
                }
                legends.Add(SimpleJsonForVisualization.Init(stringToAdd));
                var json = JsonConvert.SerializeObject(legends, Formatting.Indented);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "legends.json", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public class SimpleJsonForVisualization
        {
            public List<string> GroupedString { get; set; }

            public SimpleJsonForVisualization(List<string> stringToAdd)
            {
                GroupedString = new(stringToAdd);
            }

            internal static SimpleJsonForVisualization Init(List<string> stringToAdd)
            {
                var str = new SimpleJsonForVisualization(stringToAdd);
                stringToAdd.Clear();
                return str;
            }
        }
    }
}