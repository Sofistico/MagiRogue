using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Planet.TechRes;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using MagiRogue.Entities;
using SadConsole.Renderers;
using System.Data;
using MagiRogue.Utils.Extensions;

namespace MagiRogue.GameSys.Planet.History
{
    public sealed class HistoryAction
    {
        #region Properties

        private readonly HistoricalFigure figure;
        private readonly int year;
        private readonly List<Civilization> civs;
        private readonly WorldTile[,] tiles;
        private readonly List<HistoricalFigure> otherFigures;
        private readonly List<Site> sites;
        private readonly List<Item> items;

        #endregion Properties

        #region Ctor

        public HistoryAction(HistoricalFigure historicalFigure,
            int year,
            List<Civilization> civs,
            WorldTile[,] tiles,
            List<HistoricalFigure> otherFigures,
            List<Site> sites,
            List<Entities.Item> items)
        {
            this.figure = historicalFigure;
            this.year = year;
            this.civs = civs;
            this.tiles = tiles;
            this.otherFigures = otherFigures;
            this.sites = sites;
            this.items = items;
        }

        #endregion Ctor

        #region Actions

        public void Act()
        {
            // if from hell!
            //if (figure.MythWho != MythWho.None)
            //{
            //    SimulateMythStuff();
            //    return;
            //}

            #region Non-returning actions

            // everyone trains theirs focus!
            // check if unit has any hardworking bone in it's body or just plain old chance!

            #endregion Non-returning actions

            #region Returning actions

            //if (figure.CheckForProlificStudious())
            //{
            //    bool willResearchThisSeason = Mrn.OneIn(2);
            //    if (willResearchThisSeason)
            //    {
            //        DecideWhatToResearch();
            //        DoResearchIfPossible();
            //        if (figure.AnxiousInRegardsToActivity)
            //        {
            //            DoSometingReckless();
            //        }
            //        return;
            //    }
            //}

            //// wizardy stuff will be here as well!
            //if (figure.SpecialFlags.Contains(SpecialFlag.MagicUser))
            //{
            //    if (!figure.SpecialFlags.Contains(SpecialFlag.BuiltTower) && figure.CheckForLoneniss())
            //    {
            //        BuildATower();
            //        return;
            //    }
            //}

            #endregion Returning actions

#if DEBUG
            figure.DebugNumberOfLostYears++;
#endif
        }

        private static void ChangeFigureStayingSite(HistoricalFigure figureToChange, Site site, int year)
        {
            string changedLoc = StringFromChangingSiteLoc(figureToChange, site);
            figureToChange.ChangeStayingSite(site.WorldPos);
            figureToChange.AddLegend(changedLoc, year);
            site.AddLegend(changedLoc, year);
        }

        private static string StringFromChangingLivingSiteLoc(HistoricalFigure figure, Site result)
        {
            return $"the {figure.Name} has changed {figure.PronoumPossesive()} living location to {result.Name}!";
        }

        private void BuildATower()
        {
            int pop = Mrn.Normal2D6Dice;
            WorldTile rngTile = new WorldTile();
            var site = FigureCreatesNewSite(pop, SiteType.Tower, ref rngTile);
            figure.AddNewFlag(SpecialFlag.BuiltTower);
            ChangeFigureLivingSite(figure, site, year);
            ChangeFigureFamilyLivingSite(figure, false, site);
        }

        private Site FigureCreatesNewSite(int popNmbr, SiteType siteType, ref WorldTile rngTile)
        {
            bool goodLocation = true;

            while (goodLocation)
            {
                rngTile = tiles.Transform2DTo1D().GetRandomItemFromList();
                goodLocation = rngTile.SiteInfluence is null && !rngTile.Collidable;
            }

            Site site = new Site(rngTile.Position,
                $"Tower of {figure.Name}",
                new Population(popNmbr, figure.Body.Anatomy.Race))
            {
                SiteLeader = figure,
                SiteType = siteType,
                DiscoveriesKnow = new List<Discovery>(figure.DiscoveriesKnow),
            };

            OnNewSiteCreated(rngTile,
                site,
                $"the {figure.Name} created the {site.Name}, leaving {figure.PronoumPossesive()} previous home to live on it and continue it's research!",
                $"the {site.Name} was created by {figure.Name} for the continuation of {figure.PronoumPossesive()} research!");
            return site;
        }

        private void OnNewSiteCreated(WorldTile rngTile, Site site,
            string whyFigure = "", string whySite = "")
        {
            rngTile.SiteInfluence = site;
            sites.Add(site);
            if (!string.IsNullOrEmpty(whyFigure))
                figure.AddLegend(whyFigure, year);
            if (!string.IsNullOrEmpty(whySite))
                site.AddLegend(whySite, year);
        }

        private static string ReturnMadeFriendString(HistoricalFigure figure, HistoricalFigure randomPerson)
        {
            return $"the {figure.Name} became friends with {randomPerson.Name}";
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
                            .Where(i => i.GetCurrentStayingSiteId(civ.Sites) == site.Id)
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
            bool canMagicalResearch = figure.SpecialFlags.Contains(SpecialFlag.MagicUser);

            if (figure.ResearchTree is null)
                figure.SetupResearchTree(canMagicalResearch);

            if (figure.ResearchTree.CurrentResearchFocus is not null)
                return;

            figure.ResearchTree.GetNodeForResearch(figure);
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
                    double modifier = (double)((double)site.MundaneResources + 1) / 100;
                    modifier = modifier <= 0 ? 1 : modifier;
                    if (site.SiteType is SiteType.Tower)
                        modifier *= 2; // research is doubly effective on a tower!

                    resarchPower *= modifier;
                    resarchPower = MathMagi.Round(resarchPower);
                }
                if (figure.DoResearch(resarchPower))
                {
                    figure.CleanupResearch(site, year);
                }
            }
        }

        private bool GetFigureIsStayingOnSiteId(int siteId, HistoricalFigure hf)
        {
            var currentSiteId = hf.GetCurrentStayingSiteId(sites);
            bool isStaying = currentSiteId.HasValue && siteId == currentSiteId.Value;
            return isStaying;
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
                if (figure.CheckForStudiousInfluence())
                {
                    DeityChangesCivTendency(CivilizationTendency.Studious);
                }
            }
        }

        private void DeityChangesCivTendency(CivilizationTendency tendency)
        {
            Civilization civ = figure.GetRelatedCivFromFigure(RelationType.PatronDeity, civs);
            if (civ is null || civ.Tendency == tendency)
                return;
            civ.Tendency = tendency;

            figure.AddLegend($"The {figure.Name} changed {figure.PronoumPossesive()} followers to {figure.PronoumPossesive()} own agressive tendencies!", year);
        }

        private void DeityGivesGiftToCiv()
        {
            if (figure.CheckForAnyStudious())
            {
                // deities ignore pre-requisites!
                // TODO: See the impact
                Research gift = DataManager.ListOfResearches.Where(i => i.ValidDeityGift)
                    .ToArray().GetRandomItemFromList();

                Civilization civ = figure.GetRelatedCivFromFigure(RelationType.PatronDeity, civs);
                Site site = civ.Sites.GetRandomItemFromList();
                site.AddDiscovery(Discovery.ReturnDiscoveryFromResearch(gift, figure, site));
            }
            if (figure.CheckForGreed())
            {
                Civilization civ = figure.GetRelatedCivFromFigure(RelationType.PatronDeity, civs);
                int weatlh = GameLoop.GlobalRand.NextInt(100, 500);
                civ.Wealth += weatlh;
                figure.AddLegend($"The {figure.Name} gifted {figure.PronoumPossesive()} followers generated wealth in the value of {weatlh}",
                    year);
            }
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

        public static void Act(HistoricalFigure figure)
        {
            var rules = Find.Rules;
            var fulfilledRules = rules.AllFulfilled(figure); // some list for every fulfilled rule
            fulfilledRules.ShuffleAlgorithm();
            bool acted = false;
            foreach (var rule in fulfilledRules)
            {
                if (rule.AllowMoreThanOneAction || !acted)
                {
                    var act = rule.DoAction(figure);
                    if (act is null)
                        GameLoop.WriteToLog($"For some reason the action was null, here is the action: {rule.RuleFor}");
                    if (!rule.AllowMoreThanOneAction && act.GetValueOrDefault())
                        acted = true;
                }
            }
        }

        #endregion Actions
    }
}