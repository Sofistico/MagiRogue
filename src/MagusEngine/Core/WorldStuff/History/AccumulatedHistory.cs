using Arquimedes.Enumerators;
using GoRogue.Pathing;
using GoRogue.Random;
using MagusEngine.Core.Civ;
using MagusEngine.Core.MapStuff;
using MagusEngine.Components.TilesComponents;
using MagusEngine.Generators;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Core.WorldStuff.History
{
    public sealed class AccumulatedHistory
    {
        #region Private Fields

        private PlanetMap? planetData;

        #endregion Private Fields

        #region Consts

        private const int wealthToCreateNewSite = 250;
        private const int wealthToCreateRoad = 100;

        #endregion Consts

        #region Public Constructors

        public AccumulatedHistory()
        {
            Figures = new List<HistoricalFigure>();
            Civs = new List<Civilization>();
        }

        public AccumulatedHistory(List<HistoricalFigure> figures,
            List<Civilization> civs,
            int year)
        {
            Figures = figures;
            Civs = civs;
            Year = year;
        }

        #endregion Public Constructors

        #region Public Properties

        public List<Site> AllSites { get; set; } = new();
        public List<Civilization> Civs { get; set; }
        public int CreationYear { get; set; }
        public List<HistoricalFigure> Figures { get; set; }
        public List<ItemTemplate> ImportantItems { get; set; } = new();
        public List<Myth> Myths { get; set; } = new();
        public int Year { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void RunHistory(List<Civilization> civilizations, int yearToGameBegin,
            PlanetMap planet, WorldTile[,] tiles)
        {
            Civs = civilizations;
            planetData = planet;
            bool firstYearOnly = true;
            MythGenerator mythGenerator = new MythGenerator();
            Myths = mythGenerator.GenerateMyths(
                DataManager.ListOfRaces.GetEnumerableCollection().ToList(),
                Figures,
                planet);
            PopulateFindValues(tiles);

            if (firstYearOnly)
            {
                // stuff that will only happen in the first year! after that won't really happen anymore!
                FirstYearOnlyInteractions(civilizations.ToArray());
                firstYearOnly = false;

                DefineFiguresToHaveAInitialSite(Figures, Civs);

                List<Site> civSite = Civs.ConvertAll(i => i.Sites).ReturnListListTAsListT();
                AllSites.AddRange(civSite);
            }

            while (Year <= yearToGameBegin)
            {
                for (int season = 1; season <= 4; season++)
                {
                    // simulate historical figures stuff
                    HistoricalFigureSimulation();

                    // simulate civ stuff
                    CivilizationSimulation(tiles);

                    // other sites that aren't tied to a Civ/Hf/etc...
                    NoCivSitesSimulation(tiles);
                }
                AgeEveryone();
                ConceiveAnyChild();
                Year++;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void DefineFiguresToHaveAInitialSite(List<HistoricalFigure> figures,
            List<Civilization> civs)
        {
            foreach (HistoricalFigure figure in figures)
            {
                if (figure.RelatedCivs.Count > 0)
                {
                    var civRelation = figure.RelatedCivs.Find(i => i.GetIfMember());
                    Civilization? civ = civs.Find(i => i.Id == civRelation?.CivRelatedId);
                    figure.AddRelatedSite(civ.Sites[0].Id, SiteRelationTypes.LivesThere);
                    figure.ChangeStayingSite(civ.Sites[0].WorldPos);
                }
            }
        }

        private void AgeEveryone()
        {
            foreach (var figure in Figures)
            {
                if (figure.IsAlive
                    && figure.Body.Anatomy.CheckIfDiedByAge()
                    && figure.MythWho is MythWho.None)
                {
                    figure.KillIt(Year, $"the {figure.Name} died of old age.");
                }
                else
                {
                    figure.Body.Anatomy.AgeBody();
                }
            }

            AgePopsFromSite();
        }

        private void AgePopsFromSite()
        {
            var sites = AllSites.Where(i => i.Population.Count > 0);
            var sitesPop = sites.Select(i => i.Population);
            foreach (var listPops in sitesPop)
            {
                foreach (var pop in listPops)
                {
                    pop.TotalPopulation = pop.TotalPopulation <= 0 ? 0 : pop.TotalPopulation;
                    if (pop.TotalPopulation <= 0)
                        continue;
                    if (pop.PopulationRace().LifespanMin.HasValue
                        && Year >= pop.PopulationRace().LifespanMin.Value)
                    {
                        // about 2.5% die to age every year, if they age that is and the year is
                        // greater than the LifespanMin!
                        double totalLost = (double)(pop.TotalPopulation * (double)0.025);
                        pop.TotalPopulation = (int)MathMagi.Round(pop.TotalPopulation - totalLost);
                    }
                }
            }
        }

        private bool BuildRoadsToFriends(Civilization civ, Civilization friend, WorldTile[,] tiles)
        {
            if (!civ.PossibleWorldConstruction.Contains(WorldConstruction.Road))
                return false;

            if (civ.Relations.Any(i => i.CivRelatedId.Equals(friend.Id)
                && i.Relation is RelationType.Friendly && i.RoadBuilt.HasValue))
            {
                return false;
            }

            if (civ[friend.Id].RoadBuilt.HasValue)
                return false;

            var territory = civ.Territory[0];
            var friendTerr = friend.Territory[0];
            if (tiles.Length == 0)
                return false;
            int totalLineLength = (int)Math.Sqrt(Math.Pow(friendTerr.Y - territory.Y, 2)
                + Math.Pow(friendTerr.X - territory.X, 2));
            //TODO: Make sure that the roads don't colide with water and/or go away from water
            if (totalLineLength > 50)
            {
                return false;
            }

            Path? path = planetData.AssocietatedMap.
                        AStar.ShortestPath(territory,
                        friendTerr);

            var tile = tiles[territory.X, territory.Y];
            var closestCityTile = tiles[friendTerr.X, friendTerr.Y];
            FindPathToCityAndCreateRoad(tile, closestCityTile);
            civ[friend.Id].RoadBuilt = true;
            civ[friend.Id].Relation = RelationType.Neutral;
            return true;
        }

        private void CheckForTerritoryConflict(Civilization civ)
        {
            foreach (var otherCiv in Civs)
            {
                if (civ.Territory.Any(otherCiv.Territory.Contains))
                {
                    civ.AddCivToRelations(otherCiv, RelationType.Tension);
                }
            }
        }

        private void CivilizationSimulation(WorldTile[,] tiles)
        {
            for (int i = 0; i < Civs.Count; i++)
            {
                var civ = Civs[i];

                // civ simulation here, if civ is dead just skip and go to the next:
                if (civ.CheckIfCivIsDead())
                    continue;

                if (civ.CheckIfRulerIsDeadAndReplace(Year))
                    civ.Legends.Add(new Legend($"The {civ.Name} got a new leader!", Year));

                if (i < Civs.Count - 1)
                {
                    var otherCivs = Civs.FindAll(i => i != civ && !i.Dead);
                    int otherCivsCount = otherCivs.Count;
                    // interaction between civs here!
                    for (int x = 0; i < otherCivsCount; i++)
                    {
                        var nextCiv = otherCivs[x];
                        if (civ.Wealth > wealthToCreateRoad && BuildRoadsToFriends(civ, nextCiv, tiles))
                        {
                            civ.Wealth -= wealthToCreateRoad;
                        }

                        if (civ[nextCiv.Id].Relation is not RelationType.Enemy
                            || civ[nextCiv.Id].Relation is not RelationType.War)
                        {
                            // trade of various types!!
                            if (civ[nextCiv.Id].RoadBuilt == true && civ.CivsTradeContains(nextCiv))
                            {
                                civ.AddToTradingList(nextCiv);
                            }
                        }

                        if (civ[nextCiv.Id].Relation is RelationType.War)
                        {
                            //do war!
                        }
                    }
                    civ.TradeWithOtherCiv(otherCivs.ToArray());
                }

                int totalRevenueYear = 0;

                if (civ.Wealth > wealthToCreateNewSite)
                    CreateNewSiteIfPossible(tiles, civ);

                ClaimNewTerritory(civ);

                CheckForTerritoryConflict(civ);

                // Site simulation from the Civ here:
                foreach (Site site in civ.Sites)
                {
                    totalRevenueYear = SimulateSiteAndReturnRevenue(tiles, totalRevenueYear, site, civ);
                }
                civ.Wealth += totalRevenueYear;
            }
        }

        private void ClaimNewTerritory(Civilization civ)
        {
            // this here is making the Civilization grab infinite tiles
            IEnumerable<Point> getAllPointsSites = civ.Sites.Select(o => o.WorldPos);
            const int propagationLimit = 10; // the civ can only extend by ten tiles from each site!
            foreach (Point point in getAllPointsSites)
            {
                var directions = point.GetDirectionPoints();
                for (int i = 0; i < directions.Length; i++)
                {
                    Point direction = directions[i];
                    (Point closestPoint, double distance) = direction.FindClosestAndReturnDistance(getAllPointsSites);
                    if (planetData.AssocietatedMap.CheckForIndexOutOfBounds(direction))
                        continue;
                    if (distance <= propagationLimit)
                    {
                        civ.AddToTerritory(direction);
                    }
                }
            }
        }

        private void ConceiveAnyChild()
        {
            var pregnantFigures = Figures.Where(i => i.Pregnant);
            foreach (var item in pregnantFigures)
            {
                var civ = item.GetRelatedCivFromFigure(RelationType.Member, Civs);
                item.ConceiveChild(civ, Year);
            }
        }

        private void CreateNewSiteIfPossible(WorldTile[,] tiles, Civilization civ = null)
        {
            Site site;
            if (civ is not null)
            {
                int migrantsNmbr = GlobalRandom.DefaultRNG.NextInt(10, 100);
                Site rngSettl = civ.Sites.GetRandomItemFromList();
                var pop = rngSettl.Population.GetRandomItemFromList();
                pop.TotalPopulation -= migrantsNmbr;
                Point pos = rngSettl.WorldPos.GetPointNextTo();
                if (planetData?.AssocietatedMap?.CheckForIndexOutOfBounds(pos) == true)
                {
                    const int tries = 300;
                    for (int currentRty = 0; planetData.AssocietatedMap.CheckForIndexOutOfBounds(pos)
                        && currentRty <= tries; currentRty++)
                    {
                        pos = rngSettl.WorldPos.GetPointNextTo();
                    }
                }
                site = new Site(pos,
                    civ.RandomSiteFromLanguageName(),
                    new Population(migrantsNmbr, pop.PopulationRaceId),
                    civ.Id);
                WorldTile tile = tiles[pos.X, pos.Y];
                tile.Parent.AddComponent<SiteTile>(new(site));
                civ.AddSiteToCiv(site);
            }
            else
            {
                site = new Site();
                WorldTile tile = tiles.Transform2DTo1D().GetRandomItemFromList();
                tile.Parent.AddComponent<SiteTile>(new(site));
                site.WorldPos = tile.Parent.Position;
                site.MundaneResources = (int)tile.GetResources();
            }
            AllSites.Add(site);
        }

        private void FindPathToCityAndCreateRoad(WorldTile tile, WorldTile closestCityTile)
        {
            if (tile.HeightType == HeightType.DeepWater || tile.HeightType == HeightType.ShallowWater)
                return;

            if (!tile.Parent.GetComponent<Road>(out var road))
            {
                road = new()
                {
                    RoadId = SequentialIdGenerator.RoadId
                };
            }

            // Found the city
            if (tile == closestCityTile)
                return;

            // Shouldn't appear, but who knows Still need to know what i will do with it
            /*if (tile.HeightType == HeightType.River)
                return;*/

            Point cityPoint = closestCityTile.Parent.Position;
            Point roadPoint = tile.Parent.Position;
            var direction = Direction.GetDirection(roadPoint, cityPoint);
            if (direction != Direction.None)
            {
                WorldTile worldTile = tile.Directions[direction];
                if (worldTile.Parent.GetComponent<SiteTile>(out var site))
                {
                    site.SiteInfluence.Roads.Add(road);
                    return;
                }
                // redo to take the road from the tile components
                road.RoadDirectionInPos.TryAdd(worldTile.Parent.Position, direction);

                FindPathToCityAndCreateRoad(worldTile, closestCityTile);
            }
        }

        private void FirstYearOnlyInteractions(Civilization[] civilizations)
        {
            foreach (var civ in civilizations)
            {
                var otherCivs = Civs.Where(i => i != civ);
                Figures.AddRange(civ.SetupInitialHistoricalFigures());
                foreach (var nextCiv in otherCivs)
                {
                    var distance = Distance.Euclidean.Calculate(civ.Sites[0].WorldPos,
                        nextCiv.Sites[0].WorldPos);
                    if (distance <= 10)
                    {
                        if (civ.Tendency == nextCiv.Tendency)
                        {
                            civ.AddCivToRelations(nextCiv, RelationType.Friendly);
                        }
                        else
                        {
                            civ.AddCivToRelations(nextCiv, RelationType.Neutral);
                        }
                    }
                    else
                    {
                        civ.AddCivToRelations(nextCiv, RelationType.Unknow);
                    }
                }
            }
        }

        private void HistoricalFigureSimulation()
        {
            if (Figures.Count < 1)
                return;

            foreach (HistoricalFigure figure in Figures)
            {
                if (figure is null)
                    continue;
                if (figure.IsAlive)
                {
                    HistoryAction.Act(figure);
                }
                else
                {
                    figure.CleanupIfNotImportant(Year);
                }
            }
        }

        private void NoCivSitesSimulation(WorldTile[,] tiles)
        {
            if (AllSites.Count < 1)
                return;
            foreach (Site site in AllSites.Where(i => i.CivOwnerIfAny is null))
            {
            }
        }

        private void PopulateFindValues(WorldTile[,] tiles)
        {
            //Find.PopulateValues(Figures,
            //    Civs,
            //    AllSites,
            //    ImportantItems,
            //    this,
            //    tiles);
            Find.PopulateValues(this,
                tiles);
        }

        private int SimulateSiteAndReturnRevenue(WorldTile[,] tiles,
            int totalRevenueYear, Site site, Civilization civ)
        {
            site.CreateNewBuildings();
            site.GenerateMundaneResources();
            site.SimulatePopulationGrowth(tiles[site.WorldPos.X, site.WorldPos.Y]);
            site.SimulateTradeBetweenItsRoads(civ);
            if (site.CheckIfSiteHasCurrentLeaderOrDiedAndRemoveIt(Year))
            {
                site.AddNewLeader(civ, Year);
            }
            site.SimulateResearchPropagation(civ, tiles);
            totalRevenueYear += site.MundaneResources;
            return totalRevenueYear;
        }

        #endregion Private Methods
    }
}
