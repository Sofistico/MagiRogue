using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    public abstract class BaseHistoryAction
    {
        public List<Ruleset> Rules { get; set; }
        public bool ReturingAction { get; set; }
        public string Legend { get; set; }
        public int Year { get; set; }
        public WorldTile[,] Tiles { get; }

        public BaseHistoryAction(int year, WorldTile[,] tiles)
        {
            Year = year;
            Tiles = tiles;
        }

        public abstract void Act(HistoricalFigure figure); // at minimium an HF is needed!
    }
}
