using MagusEngine.Core.Entities.Base;
using MagusEngine.Systems;

namespace MagusEngine.Core.WorldStuff.History
{
    public class Population
    {
        public int TotalPopulation { get; set; }
        public string PopulationRaceId { get; set; }

        public Population(int totalPopulation, string populationRaceId)
        {
            TotalPopulation = totalPopulation;
            PopulationRaceId = populationRaceId;
        }

        public Race PopulationRace()
            => DataManager.QueryRaceInData(PopulationRaceId);
    }
}