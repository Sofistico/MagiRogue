using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History
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
            => Data.DataManager.QueryRaceInData(PopulationRaceId);
    }
}