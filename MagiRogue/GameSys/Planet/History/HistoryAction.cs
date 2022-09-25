using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
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
            if (figure.MythWho != MythWho.None)
            {
                SimulateMythStuff();
            }
            if (figure.SpecialFlags.Contains(SpecialFlag.Wizard))
            {
                MagicalResearch();
            }
        }

        private void SimulateMythStuff()
        {
            // fuck deities!
            if (figure.SpecialFlags.Contains(Data.Enumerators.SpecialFlag.DeityDescended))
            {
                if (CheckForInsurrection())
                    PerformDivineInsurrection();
                if (CheckForDeityGiftGiving())
                {
                    // gift something!
                    DeityGivesGiftToCiv();
                }
                if (CheckForAgressiveInfluence())
                {
                    DeityChangesCivTendency(CivilizationTendency.Aggresive);
                }
                if (CheckForStudiousInfluence())
                {
                    DeityChangesCivTendency(CivilizationTendency.Studious);
                }
            }
        }

        private void MagicalResearch()
        {
            if (CheckForProlificStudious())
            {
                Research research = Data.DataManager.RandomMagicalResearch();
            }
        }

        private void TechnologyResearch()
        {
            if (CheckForProlificStudious())
            {
                Research research = Data.DataManager.RandomNonMagicalResearch();
            }
        }

        private bool CheckForProlificStudious()
        {
            return figure.GetPersonality().Knowledge >= 25
                && (figure.GetPersonality().Perseverance >= 25
                || figure.GetPersonality().HardWork >= 25);
        }

        private bool CheckForStudiousInfluence()
        {
            return figure.GetPersonality().Knowledge >= 25
                && figure.GetPersonality().Perseverance > 0;
        }

        private void DeityChangesCivTendency(CivilizationTendency tendency)
        {
            Civilization civ = GetRelatedCivFromFigure(RelationType.PatronDeity);
            civ.Tendency = tendency;

            figure.AddLegend($"The {figure.Name} changed {figure.PronoumPossesive()} followers to {figure.PronoumPossesive()} own agressive tendencies!", year);
        }

        private bool CheckForAgressiveInfluence()
        {
            return figure.GetPersonality().Power >= 50
                && figure.GetPersonality().Peace <= 0;
        }

        private void DeityGivesGiftToCiv()
        {
            Civilization civ = GetRelatedCivFromFigure(RelationType.PatronDeity);
            int weatlh = GameLoop.GlobalRand.NextInt(100, 500);
            civ.Wealth += weatlh;
            figure.AddLegend($"The {figure.Name} gifted {figure.PronoumPossesive()} followers generated wealth in the value of {weatlh}",
                year);
        }

        private Civilization GetRelatedCivFromFigure(RelationType relationType)
        {
            int civId = figure.RelatedCivs.Find(i => i.Relation == relationType).CivRelatedId;
            Civilization civ = civs.Find(i => i.Id == civId);
            return civ;
        }

        private bool CheckForDeityGiftGiving()
        {
            return figure.GetPersonality().Sacrifice >= 25
                && figure.RelatedCivs.Any(i => i.Relation is RelationType.PatronDeity);
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
            figure.AddNewRelationToCiv(civ.Id, RelationType.PatronDeity);
        }
    }
}