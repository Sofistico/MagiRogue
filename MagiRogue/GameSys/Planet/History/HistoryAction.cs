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
    public sealed class HistoryAction
    {
        private readonly HistoricalFigure figure;
        private readonly int year;
        private readonly List<Civilization> civs;
        private readonly WorldTile[,] tiles;
        private readonly List<HistoricalFigure> otherFigures;
        private readonly List<Site> sites;

        public HistoryAction(HistoricalFigure historicalFigure,
            int year,
            List<Civilization> civs,
            WorldTile[,] tiles,
            List<HistoricalFigure> otherFigures,
            List<Site> sites)
        {
            this.figure = historicalFigure;
            this.year = year;
            this.civs = civs;
            this.tiles = tiles;
            this.otherFigures = otherFigures;
            this.sites = sites;
        }

        public void Act()
        {
            if (figure.MythWho != MythWho.None)
            {
                SimulateMythStuff();
            }
            if (figure.CheckForProlificStudious())
            {
                LearnNewDiscoveriesKnowToTheSite();
                DecideWhatToResearch();
                DoResearchIfPossible();
            }
        }

        private void DecideWhatToResearch()
        {
            if (figure.CurrentDiscoveryLearning is not null)
                return;
        }

        private void LearnNewDiscoveriesKnowToTheSite()
        {
            if (figure.GetCurrentStayingSiteId().HasValue)
            {
                int familiarityBonus = 0;
                int currentSiteId = figure.GetCurrentStayingSiteId().Value;
                Site currentSite = sites.Find(i => i.Id == currentSiteId);
                if (figure.GetLivingSiteId().HasValue && currentSiteId == figure.GetLivingSiteId().Value)
                    familiarityBonus = Mrn.Exploding2D6Dice * 2;
                for (int i = 0; i < currentSite.DiscoveriesKnow.Count; i++)
                {
                    Discovery disc = currentSite.DiscoveriesKnow[i];
                    if (!figure.DiscoveriesKnow.Any(i => i.Id == disc.Id))
                    {
                        figure.CurrentDiscoveryLearning = new DiscoveryResearch(disc, familiarityBonus);
                    }
                }
            }
        }

        private void DoResearchIfPossible()
        {
            double resarchPower = 0;
            var currentSite = figure.GetCurrentStayingSiteId();
            Site site = currentSite.HasValue ? sites.Find(i => i.Id == currentSite.Value) : null;

            if (figure.CurrentDiscoveryLearning is not null)
            {
                resarchPower += Mrn.Exploding2D6Dice;
                if (site is not null)
                {
                    resarchPower *= (double)((double)site.MagicalResources / 100);
                }

                if (figure.DoResearch(resarchPower))
                {
                    figure.CurrentDiscoveryLearning = null;
                }
            }
        }

        private void SimulateMythStuff()
        {
            // fuck deities!
            if (figure.SpecialFlags.Contains(Data.Enumerators.SpecialFlag.DeityDescended))
            {
                if (figure.CheckForInsurrection())
                    PerformDivineInsurrection();
                if (CheckForDeityGiftGiving())
                {
                    // gift something!
                    DeityGivesGiftToCiv();
                }
                if (figure.CheckForAgressiveInfluence())
                {
                    DeityChangesCivTendency(CivilizationTendency.Aggresive);
                }
                if (CheckForStudiousInfluence())
                {
                    DeityChangesCivTendency(CivilizationTendency.Studious);
                }
            }
        }

        private bool CheckForStudiousInfluence()
        {
            return figure.GetPersonality().Knowledge >= 25
                && figure.GetPersonality().Perseverance > 0;
        }

        private void DeityChangesCivTendency(CivilizationTendency tendency)
        {
            Civilization civ = figure.GetRelatedCivFromFigure(RelationType.PatronDeity, civs);
            civ.Tendency = tendency;

            figure.AddLegend($"The {figure.Name} changed {figure.PronoumPossesive()} followers to {figure.PronoumPossesive()} own agressive tendencies!", year);
        }

        private void DeityGivesGiftToCiv()
        {
            Civilization civ = figure.GetRelatedCivFromFigure(RelationType.PatronDeity, civs);
            int weatlh = GameLoop.GlobalRand.NextInt(100, 500);
            civ.Wealth += weatlh;
            figure.AddLegend($"The {figure.Name} gifted {figure.PronoumPossesive()} followers generated wealth in the value of {weatlh}",
                year);
        }

        private bool CheckForDeityGiftGiving()
        {
            return figure.GetPersonality().Sacrifice >= 25
                && figure.RelatedCivs.Any(i => i.Relation is RelationType.PatronDeity);
        }

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