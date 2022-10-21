using GoRogue.DiceNotation.Terms;
using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
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
                int yearsToGenerate = 100;
                var planet = planetGenerator.CreatePlanet(100, 100, yearsToGenerate, 15);
                List<object> legends = new();
                List<string> stringToAdd = new();
                foreach (var site in planet.WorldHistory.AllSites)
                {
                    foreach (var item in site.SiteLegends)
                    {
                        Console.WriteLine(item.ToString());
                        stringToAdd.Add(item.ToString());
                    }
                    object obj = new { SiteName = site.Name, Legends = new List<string>(stringToAdd) };
                    object finalObj = new { Sites = obj };
                    legends.Add(finalObj);
                    stringToAdd.Clear();
                }

                foreach (var civ in planet.WorldHistory.Civs)
                {
                    foreach (var item in civ.Legends)
                    {
                        Console.WriteLine(item.ToString());
                        stringToAdd.Add(item.ToString());
                    }
                    legends.Add(new { Civ = new { CivName = civ.Name, Legends = new List<string>(stringToAdd) } });

                    stringToAdd.Clear();
                }

                foreach (var hf in planet.WorldHistory.Figures)
                {
                    foreach (var item in hf.Legends)
                    {
                        Console.WriteLine(item.ToString());
                        stringToAdd.Add(item.ToString());
                    }
                    legends.Add(new { Hf = new { HistoricalFigure = hf.Name, Legends = new List<string>(stringToAdd) } });

                    stringToAdd.Clear();
                }

                foreach (var item in planet.WorldHistory.ImportantItems)
                {
                    foreach (var legend in item.Legends)
                    {
                        Console.WriteLine(legend.ToString());
                        stringToAdd.Add(item.ToString());
                    }
                    legends.Add(new { ItemLegends = new { Items = item.Name, Legends = new List<string>(stringToAdd) } });

                    stringToAdd.Clear();
                }

                foreach (var myth in planet.WorldHistory.Myths)
                {
                    Console.WriteLine(myth.ToString());
                    stringToAdd.Add(myth.ToString());
                }

                legends.Add(new
                {
                    Myths = new { Legend = new List<string>(stringToAdd) }
                });
                stringToAdd.Clear();

                var json = JsonConvert.SerializeObject(legends, Formatting.Indented);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "legends.json", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}