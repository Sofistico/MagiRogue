using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.EntitySerialization;
using System;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Civ
{
    public class Settlement
    {
        public Point WorldPos { get; set; }
        public string Name { get; set; }
        public SettlementSize Size { get; set; }
        public int MilitaryStrenght { get; set; }
        public int Population { get; set; }
        public float MundaneResources { get; set; }
        public float MagicalResources { get; set; }
        public List<ItemTemplate> Nodes { get; set; }
        public int MagicStrenght { get; set; }

        public Settlement()
        {
        }

        public Settlement(Point pos, string name, int totalPopulation)
        {
            WorldPos = pos;
            Name = name;
            Population = totalPopulation;
        }

        public void DefineSettlementSize()
        {
            if (MundaneResources > 100 && MundaneResources < 1000)
            {
                Size = SettlementSize.Small;
            }
            if (MundaneResources > 1000 && MundaneResources < 10000)
            {
                Size = SettlementSize.Medium;
            }
            if (MundaneResources > 10000)
            {
                Size = SettlementSize.Large;
            }
        }
    }
}