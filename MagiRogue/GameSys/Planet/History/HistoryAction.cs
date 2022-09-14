using MagiRogue.Data.Enumerators;
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
        private readonly HistoricalFigure figure;
        private readonly int year;
        private readonly List<Civilization> civs;
        private readonly WorldTile[,] tiles;
        private readonly List<HistoricalFigure> otherFigures;

        public HistoryAction(HistoricalFigure historicalFigure, int year,
            List<Civilization> civs,
            WorldTile[,] tiles,
            List<HistoricalFigure> otherFigures)
        {
            this.figure = historicalFigure;
            this.year = year;
            this.civs = civs;
            this.tiles = tiles;
            this.otherFigures = otherFigures;
        }

        public void Act()
        {
            if (figure.MythWho.HasValue)
            {
                SimulateMythStuff();
            }
        }

        private void SimulateMythStuff()
        {
            if (figure.SpecialFlags.Contains(Data.Enumerators.SpecialFlag.DeityDescended))
            {
                if (CheckForInsurrection())
                    PerformDivineInsurrection();
                if (CheckForGiftGiving())
                {
                    // gift something!
                    DeityGivesGiftToCiv();
                }
            }
        }

        private void DeityGivesGiftToCiv()
        {
        }

        private bool CheckForGiftGiving()
        {
            return figure.GetPersonality().Sacrifice >= 25
                && figure.RelatedCivs.Count > 0;
        }

        private bool CheckForInsurrection()
            => figure.GetPersonality().Power >= 25;

        private void PerformDivineInsurrection()
        {
            Civilization civ = civs.GetRandomItemFromList();
            (Noble ruler, HistoricalFigure noKingAnymore) = civ.GetRulerNoblePosition();
            if (noKingAnymore is not null)
            {
                figure.AddRelatedHf(noKingAnymore.Id, HfRelationType.Enemy);

                civ.RemoveNoble(ruler, noKingAnymore, year,
                    $"because the {figure.MythWho} {figure.Name} assumed control of the civilization!");
            }
            civ.AppointNewNoble(ruler, figure, year,
                "because it took it's position as the divine ruler of the civilization!");
        }
    }
}