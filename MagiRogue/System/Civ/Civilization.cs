using MagiRogue.Data;
using MagiRogue.Data.Serialization;
using MagiRogue.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Civ
{
    public enum CivilizationTendency
    {
        Normal,
        Aggresive,
        Studious,
    }

    public class Civilization
    {
        public string Name { get; set; }
        public Race PrimaryRace { get; set; }
        public int MilitaryStrenght { get; set; }
        public int MagicStrenght { get; set; }
        public int Population { get; set; }
        public CivilizationTendency Tendency { get; set; }
        public float MundaneResources { get; set; }
        public float MagicalResources { get; set; }
        public List<ItemTemplate> Nodes { get; set; }
        public Territory Territory { get; set; }

        public Civilization(string name, Race primaryRace, int population, CivilizationTendency tendency)
        {
            Name = name;
            PrimaryRace = primaryRace;
            Population = population;
            Tendency = tendency;
            Nodes = new();
            Territory = new();
        }
    }
}