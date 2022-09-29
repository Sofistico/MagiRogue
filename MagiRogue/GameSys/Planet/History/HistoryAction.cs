using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Planet.TechRes;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace MagiRogue.GameSys.Planet.History
{
    public class HistoryAction
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
            if (figure.CheckForAnyStudious())
                LearnNewDiscoveriesKnowToTheSite();
            if (figure.CheckForProlificStudious())
            {
                bool willResearchThisSeason = Mrn.OneIn(3);
                if (willResearchThisSeason)
                {
                    DecideWhatToResearch();
                    DoResearchIfPossible();
                    if (figure.AnxiousInRegardsToActivity)
                    {
                        DoSometingReckless();
                    }
                }
            }
        }

        private void DoSometingReckless()
        {
            Site site = GetFigureStayingSiteIfAny();
            if (site is not null)
            {
                if (figure.CheckForAnger())
                {
                    // murder someone! rob them! do something bad!
                    bool checkMurder = figure.GetPersonality().Anger >= 30;
                    if (checkMurder)
                    {
                        Civilization civ = GetCivFromSite(site);
                        HistoricalFigure deadThing =
                            civ.ImportantPeople
                            .Where(i => i.GetCurrentStayingSiteId() == site.Id)
                            .ToList()
                            .GetRandomItemFromList();
                        figure.TryAttackAndMurder(deadThing, year, $", so that {figure.Pronoum} could get inspiration!");
                        figure.ClearAnxiousness();
                    }
                }
            }
        }

        private Civilization GetCivFromSite(Site site)
        {
            return civs.FirstOrDefault(i => i.Id == site.CivOwnerIfAny.Value);
        }

        private void DecideWhatToResearch()
        {
            if (figure.ResearchTree.CurrentResearchFocus is not null)
                return;
            bool canMagicalResearch = figure.MythWho is MythWho.Wizard;

            var site = GetFigureStayingSiteIfAny();
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
                        figure.DiscoveriesKnow.Add(disc);
                    }
                }
            }
        }

        private void DoResearchIfPossible()
        {
            double resarchPower = 0;
            Site site = GetFigureStayingSiteIfAny();

            if (figure.ResearchTree.CurrentResearchFocus is not null)
            {
                resarchPower += Mrn.Exploding2D6Dice;
                if (site is not null)
                {
                    resarchPower *= (double)((double)site.MagicalResources / 100);
                }
                if (figure.DoResearch(resarchPower, year))
                {
                    figure.CleanupResearch(site, year);
                }
            }
        }

        private Site GetFigureStayingSiteIfAny()
        {
            var currentSite = figure.GetCurrentStayingSiteId();
            Site site = currentSite.HasValue ? sites.Find(i => i.Id == currentSite.Value) : null;
            return site;
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