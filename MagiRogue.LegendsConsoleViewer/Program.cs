using MagiRogue.GameSys.Planet;

namespace MagiRogue.LegendsConsoleViewer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // test stuff!
            //
            PlanetGenerator planetGenerator = new PlanetGenerator();
            int yearsToGenerate = 100;
            var planet = planetGenerator.CreatePlanet(100, 100, yearsToGenerate, 15);
            foreach (var site in planet.WorldHistory.AllSites)
            {
                foreach (var item in site.SiteLegends)
                {
                    Console.WriteLine(item.ToString());
                }
            }
            foreach (var civ in planet.WorldHistory.Civs)
            {
                foreach (var item in civ.Legends)
                {
                    Console.WriteLine(item.ToString());
                }
            }
            foreach (var hf in planet.WorldHistory.Figures)
            {
                foreach (var item in hf.Legends)
                {
                    Console.WriteLine(item.ToString());
                }
            }
            foreach (var item in planet.WorldHistory.ImportantItems)
            {
                foreach (var legend in item.Legends)
                {
                    Console.WriteLine(legend.ToString());
                }
            }
            foreach (var myth in planet.WorldHistory.Myths)
            {
                Console.WriteLine(myth.ToString());
            }
        }
    }
}