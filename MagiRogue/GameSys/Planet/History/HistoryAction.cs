using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.Planet.History
{
    public class HistoryAction
    {
        private readonly HistoricalFigure historicalFigure;
        private readonly int year;
        private readonly List<Civilization> civs;
        private readonly WorldTile[,] tiles;

        public Legend Legend { get; set; }

        public HistoryAction(HistoricalFigure historicalFigure, int year,
            List<Civilization> civs,
            WorldTile[,] tiles)
        {
            this.historicalFigure = historicalFigure;
            this.year = year;
            this.civs = civs;
            this.tiles = tiles;
        }

        public void Act()
        {
            if (historicalFigure.MythWho.HasValue)
            {
                SimulateMythStuff();
            }
        }

        private void SimulateMythStuff()
        {
            if (historicalFigure.SpecialFlags.Contains(Data.Enumerators.SpecialFlag.DeityDescended))
            {
                CheckForAndPerformDivineInsurrection();
            }
        }

        private void CheckForAndPerformDivineInsurrection()
        {
            if (historicalFigure.GetPersonality().Power >= 25)
            {
                Civilization civ = civs.GetRandomItemFromList();
                (Noble ruler, HistoricalFigure noKingAnymore) = civ.GetRulerNoblePosition();

                civ.RemoveNoble(ruler, noKingAnymore, year,
                    $"because the {historicalFigure.MythWho} assumed control of the civilization!");
                civ.AppointNewNoble(ruler, historicalFigure, year,
                    "because it took it's position as the divine ruler of the civilization!");
            }
        }
    }
}