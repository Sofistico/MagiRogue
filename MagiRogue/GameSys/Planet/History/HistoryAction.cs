﻿using MagiRogue.Data;
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
        private readonly AccumulatedHistory history;
        private readonly List<Item> items;

        #endregion Properties

        #region Ctor

        public HistoryAction(HistoricalFigure historicalFigure,
            int year,
            List<Civilization> civs,
            WorldTile[,] tiles,
            List<HistoricalFigure> otherFigures,
            List<Site> sites,
            AccumulatedHistory history,
            List<Entities.Item> items)
        {
            this.figure = historicalFigure;
            this.year = year;
            this.civs = civs;
            this.tiles = tiles;
            this.otherFigures = otherFigures;
            this.sites = sites;
            this.history = history;
            this.items = items;
        }

        #endregion Ctor

        #region Actions

        public void Act()
        {
            #region Non-returning actions

            // everyone trains theirs focus!
            // check if unit has any hardworking bone in it's body or just plain old chance!
            if (figure.CheckForHardwork() || Mrn.OneIn(3))
            {
                figure.TrainAbilityFocus();
            }

            if (figure.SpecialFlags.Contains(SpecialFlag.MagicUser))
            {
                GenerateMagicalResourcesForSite();
            }

            //romance and interfigure stuff!
            if (figure.CheckForRomantic())
            {
                RomanceSomeoneInsideSameSite();
            }

            if (figure.IsMarried() && Mrn.OneIn(5))
            {
                TryForBaby();
            }

            if (figure.CheckForFriendship())
            {
                GetANewFriend();
            }

            #endregion Non-returning actions

            #region Returning actions

            // if from hell!
            if (figure.MythWho != MythWho.None)
            {
                SimulateMythStuff();
                return;
            }
            // do Pet and animals do stuff?

            if (figure.CheckForAnyStudious())
            {
                LearnNewDiscoveriesKnowToTheSite();
                return;
            }

            if (figure.CheckForWanderlust())
            {
                WanderAndSettleSomewhere();
                return;
            }

            if (figure.CheckForProlificStudious())
            {
                bool willResearchThisSeason = Mrn.OneIn(2);
                if (willResearchThisSeason)
                {
                    DecideWhatToResearch();
                    DoResearchIfPossible();
                    if (figure.AnxiousInRegardsToActivity)
                    {
                        DoSometingReckless();
                    }
                    return;
                }
            }

            // wizardy stuff will be here as well!
            if (figure.SpecialFlags.Contains(SpecialFlag.MagicUser) && figure.CheckForLoneniss())
            {
                if (!figure.SpecialFlags.Contains(SpecialFlag.BuiltTower))
                {
                    BuildATower();
                    return;
                }
            }

            #endregion Returning actions
        }

        private void WanderAndSettleSomewhere()
        {
            // if figure is a noble somewhere, it shouldn't wander!
            if (figure.NobleTitles.Count >= 0)
                return;
            // one in 5 chance to settle somewhere else
            if (Mrn.OneIn(5))
            {
                bool changedCiv = false;
                // one in 10 to migrate to another civ
                if (Mrn.OneIn(10))
                {
                    changedCiv = ChangeFigureCiv(figure, year, civs);
                }
                int civId = figure.GetMemberCivId();
                Civilization currentCiv = civs.FirstOrDefault(i => i.Id == civId);
                if (currentCiv.Sites.Count > 1 || changedCiv)
                {
                    var siteId = figure.GetCurrentStayingSiteId(currentCiv.Sites);
                    var result = siteId.HasValue ? currentCiv.Sites
                        .Where(i => i.Id != siteId.Value)
                        .ToList()
                        .GetRandomItemFromList() : currentCiv.Sites.GetRandomItemFromList();
                    ChangeFigureStayingSite(figure, result, year);
                    foreach (FamilyNode family in figure.FamilyLink.Family)
                    {
                        if (family.IsCloseFamily())
                        {
                            var fig = family.Figure;
                            ChangeFigureStayingSite(fig, result, year);
                            if (changedCiv)
                                ChangeFigureCiv(fig, year, civs);
                        }
                    }
                }
            }
        }

        private static bool ChangeFigureCiv(HistoricalFigure figure, int year, List<Civilization> civs)
        {
            CivRelation prevRelation = figure.RelatedCivs.FirstOrDefault(i => i.GetIfMember());
            HistoricalFigure.RemovePreviousCivRelationAndSetNew(prevRelation, RelationType.ExMember);
            var civ = civs.GetRandomItemFromList();
            figure.AddNewRelationToCiv(civ.Id, RelationType.Member);
            figure.AddLegend($"the {figure.Name} has migrated to the {civ.Name}", year);
            civ.AddLegend($"{figure.Name} joined as a member of {civ.Name}", year);
            return true;
        }

        private static void ChangeFigureStayingSite(HistoricalFigure figure, Site result, int year)
        {
            figure.ChangeLivingSite(result.Id);
            string changedLoc = StringFromChangingSiteLoc(figure, result);
            figure.AddLegend(changedLoc, year);
            result.AddLegend(changedLoc, year);
        }

        private static string StringFromChangingSiteLoc(HistoricalFigure figure, Site result)
        {
            return $"the {figure.Name} has changed {figure.PronoumPossesive()} living location to {result.Name}!";
        }

        private void GenerateMagicalResourcesForSite()
        {
            Site site = GetCurrentlyStayingSite();
            if (site is not null)
            {
                site.MagicalResources += 10;
            }
        }

        private void TryForBaby()
        {
            HistoricalFigure spouse = GetSpouse();
            if (spouse is null)
                return;
            if (CompatibleGenderForBabies(spouse) && (!figure.Pregnant || !spouse.Pregnant))
            {
                figure.MakeBabyWith(spouse);
            }
        }

        private bool CompatibleGenderForBabies(HistoricalFigure spouse)
        {
            var allowedGenders = new Sex[]
            {
                Sex.Male,
                Sex.Female
            };
            if (allowedGenders.Contains(spouse.HFGender)
                && allowedGenders.Contains(figure.HFGender))
            {
                return spouse.HFGender != figure.HFGender;
            }
            return false;
        }

        private HistoricalFigure GetSpouse()
        {
            int? idfigure = figure.GetRelatedHfSpouseId();
            if (!idfigure.HasValue)
                return null;
            return otherFigures.FirstOrDefault(i => i.Id == idfigure.Value);
        }

        private void BuildATower()
        {
            int pop = Mrn.Normal2D6Dice;
            WorldTile rngTile = new WorldTile();
            FigureCreatesNewSite(pop, SiteType.Tower, ref rngTile);
            figure.AddNewFlag(SpecialFlag.BuiltTower);
        }

        private void FigureCreatesNewSite(int popNmbr, SiteType siteType, ref WorldTile rngTile)
        {
            bool colidable = true;

            while (colidable)
            {
                rngTile = tiles.Transform2DTo1D().GetRandomItemFromList();
                colidable = !rngTile.Collidable;
            }

            Site site = new Site(rngTile.Position, $"Tower of {figure.Name}", new Population(popNmbr, figure.Body.Anatomy.Race))
            {
                SiteLeader = figure,
                SiteType = siteType,
                DiscoveriesKnow = new List<Discovery>(figure.DiscoveriesKnow),
            };

            OnNewSiteCreated(rngTile, site);
            figure.ChangeLivingSite(site.Id);
        }

        private void OnNewSiteCreated(WorldTile rngTile, Site site)
        {
            rngTile.SiteInfluence = site;
            sites.Add(site);
        }

        private Site GetCurrentlyStayingSite()
        {
            int? site = figure.GetCurrentStayingSiteId(sites);
            if (site.HasValue)
            {
                return sites.FirstOrDefault(i => i.Id == site.Value);
            }
            return null;
        }

        private void GetANewFriend()
        {
            Site site = GetFigureStayingSiteIfAny();
            if (site is not null && !figure.IsMarried())
            {
                var peopleInside = GetAllFiguresStayingInSiteIfAny(site.Id);
                if (peopleInside.Count >= 0)
                {
                    var randomPerson = peopleInside.GetRandomItemFromList();
                    figure.MakeFriend(randomPerson);
                }
            }
        }

        private void RomanceSomeoneInsideSameSite()
        {
            Site site = GetFigureStayingSiteIfAny();
            if (site is not null && !figure.IsMarried())
            {
                var peopleInside = GetAllFiguresStayingInSiteIfAny(site.Id);
                int aceptableDiferenceAge;
                bool isAdult = figure.IsAdult();
                if (isAdult)
                {
                    aceptableDiferenceAge = Math.Max(figure.Body.Anatomy.GetRaceAdulthoodAge(),
                        figure.Body.GetCurrentAge() / 2);
                }
                else
                {
                    // kids can't marry!
                    aceptableDiferenceAge = 999;
                }
                var peopleInARangeOfAgeCloseAndPredispost = peopleInside.Where(i =>
                    (i.Body.GetCurrentAge() >= aceptableDiferenceAge) && i.CheckForRomantic()).ToList();
                if (peopleInARangeOfAgeCloseAndPredispost.Count >= 0)
                {
                    var randomPerson = peopleInARangeOfAgeCloseAndPredispost.GetRandomItemFromList();
                    figure.Marry(randomPerson);
                    StringBuilder anotherB = new StringBuilder($"the {figure.Name} married with {randomPerson.Name}");
                    StringBuilder bb = new StringBuilder($"the {randomPerson.Name} married with {figure.Name}");
                    figure.AddLegend(anotherB.ToString(), year);
                    randomPerson.AddLegend(bb.ToString(), year);
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

        private void LearnNewDiscoveriesKnowToTheSite()
        {
            int? currentSiteId = figure.GetCurrentStayingSiteId(sites);
            if (currentSiteId.HasValue)
            {
                int familiarityBonus = 0;
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
            var currentSite = figure.GetCurrentStayingSiteId(sites);
            Site site = currentSite.HasValue ? sites.Find(i => i.Id == currentSite.Value) : null;
            return site;
        }

        private Site GetFigureStayingSiteIfAny(HistoricalFigure hf)
        {
            var currentSite = hf.GetCurrentStayingSiteId(sites);
            Site site = currentSite.HasValue ? sites.Find(i => i.Id == currentSite.Value) : null;
            return site;
        }

        private bool GetFigureIsStayingOnSiteId(int siteId, HistoricalFigure hf)
        {
            var currentSiteId = hf.GetCurrentStayingSiteId(sites);
            bool isStaying = currentSiteId.HasValue && siteId == currentSiteId.Value;
            return isStaying;
        }

        private List<HistoricalFigure> GetAllFiguresStayingInSiteIfAny(int figureSiteId)
        {
            var list = new List<HistoricalFigure>();
            foreach (var item in otherFigures)
            {
                if (GetFigureIsStayingOnSiteId(figureSiteId, item))
                {
                    list.Add(item);
                }
            }
            return list;
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
                site.DiscoveriesKnow.Add(Discovery.ReturnDiscoveryFromResearch(gift, figure, site));
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

        #endregion Actions
    }
}