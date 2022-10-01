using GoRogue.Pathing;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.Planet.History
{
    public sealed class AccumulatedHistory
    {
        private PlanetMap planetData;
        private List<Site> sitesWithNoCiv = new();

        #region Consts

        private const int wealthToCreateRoad = 100;
        private const int wealthToCreateNewSite = 1000;

        #endregion Consts

        public List<HistoricalFigure> Figures { get; set; }
        public List<Civilization> Civs { get; set; }
        public List<Site> AllSites { get; set; } = new();
        public List<Myth> Myths { get; set; } = new();
        public int Year { get; set; }
        public long TicksSinceCreation { get; set; }

        public AccumulatedHistory()
        {
            Figures = new List<HistoricalFigure>();
            Civs = new List<Civilization>();
        }

        public AccumulatedHistory(List<HistoricalFigure> figures, List<Civilization> civs, int year)
        {
            Figures = figures;
            Civs = civs;
            Year = year;
        }

        public void RunHistory(List<Civilization> civilizations, int yearToGameBegin,
            PlanetMap planet, WorldTile[,] tiles)
        {
            Civs = civilizations;
            planetData = planet;
            bool firstYearOnly = true;
            MythGenerator mythGenerator = new MythGenerator();
            Myths = mythGenerator.GenerateMyths(
                Data.DataManager.ListOfRaces.ToList(),
                Figures,
                planet);

            if (firstYearOnly)
            {
                // stuff that will only happen in the first year!
                // after that won't really happen anymore!
                FirstYearOnlyInteractions(civilizations.ToArray());
                firstYearOnly = false;
            }

            while (Year < yearToGameBegin)
            {
                List<Site> civSite = Civs.Select(i => i.Sites).ToList().ReturnListListTAsListT();
                AllSites.AddRange(civSite);
                AllSites.AddRange(sitesWithNoCiv);

                int season = 1; // 4 actions per year, representing the four seasons
                while (season <= 4)
                {
                    // simulate historical figures stuff
                    HistoricalFigureSimulation(tiles, AllSites);

                    // simulate civ stuff
                    CivilizationSimulation(tiles);

                    // other sites that aren't tied to a Civ/Hf/etc...
                    NoCivSitesSimulation(tiles);

                    season++; // another season passes...
                }
                AgeEveryone();
                AllSites.Clear();
                Year++;
            }
        }

        private void AgeEveryone()
        {
            foreach (var figure in Figures)
            {
                if (figure.Body.Anatomy.CheckIfDiedByAge())
                {
                    figure.KillIt(Year);
                }
                else
                    figure.Body.Anatomy.AgeBody();
            }
        }

        private void NoCivSitesSimulation(WorldTile[,] tiles)
        {
            if (AllSites.Count < 1)
                return;
            foreach (Site site in AllSites)
            {
            }
        }

        private void HistoricalFigureSimulation(WorldTile[,] tiles, List<Site> sites)
        {
            if (Figures.Count < 1)
                return;

            foreach (HistoricalFigure figure in Figures)
            {
                if (figure.IsAlive)
                    figure.HistoryAct(Year, tiles, Civs, Figures, sites);
                else
                {
                    figure.CleanupIfNotImportant(Year);
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

                if (i < Civs.Count - 1)
                {
                    var otherCivs = Civs.Where(i => i != civ);
                    // interaction between civs here!
                    foreach (Civilization nextCiv in otherCivs)
                    {
                        if (civ.Wealth > wealthToCreateRoad)
                        {
                            if (BuildRoadsToFriends(civ, nextCiv, tiles))
                                civ.Wealth -= wealthToCreateRoad;
                        }

                        if (civ[nextCiv.Id].Relation is not RelationType.Enemy
                            || civ[nextCiv.Id].Relation is RelationType.War)
                        {
                            // trade of various types!!
                            if (civ[nextCiv.Id].RoadBuilt.HasValue && civ[nextCiv.Id].RoadBuilt.Value)
                            {
                                civ.AddToTradingList(nextCiv);
                                civ.TradeWithOtherCiv(nextCiv);
                            }
                        }

                        if (civ[nextCiv.Id].Relation is RelationType.War)
                        {
                            //do war!
                        }
                    }
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

        private void CheckForTerritoryConflict(Civilization civ)
        {
            foreach (var otherCiv in Civs)
            {
                if (civ.Territory.Any(p => otherCiv.Territory.Contains(p)))
                {
                    civ.AddCivToRelations(otherCiv, RelationType.Tension);
                }
            }
        }

        private static void ClaimNewTerritory(Civilization civ)
        {
            IEnumerable<Point> getAllPointsInsideTerritory = civ.Territory.ToList();
            IEnumerable<Point> getAllPointsSites = civ.Sites.Select(o => o.WorldPos);
            int propagationLimit = 10; // the civ can only extend by ten tiles from each site!
            foreach (Point point in getAllPointsInsideTerritory)
            {
                var directions = point.GetDirectionPoints();
                for (int i = 0; i < directions.Length; i++)
                {
                    Point direction = directions[i];
                    (Point closestPoint, double distance) = direction.FindClosestAndReturnDistance(getAllPointsSites);

                    if (distance <= propagationLimit)
                    {
                        civ.Territory.Add(direction);
                    }
                }
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
                    Distance dis = new Distance();
                    var distance = dis.Calculate(civ.Sites[0].WorldPos,
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

        private int SimulateSiteAndReturnRevenue(WorldTile[,] tiles,
            int totalRevenueYear, Site site, Civilization civ)
        {
            site.CreateNewBuildings();
            totalRevenueYear += site.MundaneResources;
            site.GenerateMundaneResources();
            site.SimulatePopulationGrowth(tiles[site.WorldPos.X, site.WorldPos.Y]);
            site.SimulateTradeBetweenItsRoads(civ);
            if (site.CheckIfSiteHasCurrentLeaderOrDiedAndRemoveIt(Year))
            {
                site.AddNewLeader(civ, Year);
            }
            site.SimulateResearchPropagation(civ, tiles);
            return totalRevenueYear;
        }

        private void CreateNewSiteIfPossible(WorldTile[,] tiles, Civilization civ = null)
        {
            Site site;
            if (civ is not null)
            {
                int migrants = GameLoop.GlobalRand.NextInt(10, 100);
                Site rngSettl = civ.Sites.GetRandomItemFromList();
                rngSettl.Population -= migrants;
                Point pos = rngSettl.WorldPos.GetPointNextTo();
                site = new Site(pos,
                    civ.RandomSiteFromLanguageName(),
                    migrants,
                    civ.Id);
                site.SetId();
                WorldTile tile = tiles[pos.X, pos.Y];
                tile.SiteInfluence = site;
                civ.AddSiteToCiv(site);
            }
            else
            {
                site = new Site();
                WorldTile tile = tiles.Transform2DTo1D().GetRandomItemFromList();
                tile.SiteInfluence = site;
                site.WorldPos = tile.Position;
                site.MundaneResources = (int)tile.GetResources();
                sitesWithNoCiv.Add(site);
            }
            AllSites.Add(site);
        }

        private bool BuildRoadsToFriends(Civilization civ, Civilization friend, WorldTile[,] tiles)
        {
            if (!civ.PossibleWorldConstruction.Contains(WorldConstruction.Road))
                return false;

            if (civ.Relations.Any(i => i.CivRelatedId.Equals(friend.Id)
                && i.Relation is RelationType.Friendly && i.RoadBuilt.HasValue))
                return false;
            if (civ[friend.Id].RoadBuilt.HasValue)
                return false;

            var territory = civ.Territory[0];
            var friendTerr = friend.Territory[0];
            if (tiles.Length < 0)
                return false;
            Path path = planetData.AssocietatedMap.
                        AStar.ShortestPath(territory,
                        friendTerr);

            //TODO: Make sure that the roads don't colide with water and/or go away from water
            if (path.Length > 50)
            {
                return false;
            }
            var tile = tiles[territory.X, territory.Y];
            var closestCityTile = tiles[friendTerr.X, friendTerr.Y];
            FindPathToCityAndCreateRoad(tile, closestCityTile);
            civ[friend.Id].RoadBuilt = true;
            return true;
        }

        private void FindPathToCityAndCreateRoad(WorldTile tile, WorldTile closestCityTile)
        {
            Road road = new()
            {
                RoadId = SequentialIdGenerator.RoadId
            };

            if (tile.HeightType == HeightType.DeepWater || tile.HeightType == HeightType.ShallowWater)
                return;

            if (tile.Road != null)
                road = tile.Road;

            // Found the city
            if (tile == closestCityTile)
                return;

            // Shouldn't appear, but who knows
            // Still need to know what i will do with it
            /*if (tile.HeightType == HeightType.River)
                return;*/

            Point cityPoint = closestCityTile.Position;
            Point roadPoint = tile.Position;
            var direction = Direction.GetDirection(roadPoint, cityPoint);

            if (direction == Direction.Up)
            {
                WorldTile worldTile = tile.Top;
                if (worldTile.SiteInfluence is not null)
                {
                    worldTile.SiteInfluence.Roads.Add(road);
                    return;
                }
                worldTile.Road = road;
                worldTile.Road.AddTileToList(worldTile);
                worldTile.Road.RoadDirectionInPos.TryAdd(worldTile.Position, WorldDirection.Top);

                FindPathToCityAndCreateRoad(worldTile, closestCityTile);
            }
            if (direction == Direction.Down)
            {
                WorldTile worldTile = tile.Bottom;
                if (worldTile.SiteInfluence is not null)
                {
                    worldTile.SiteInfluence.Roads.Add(road);

                    return;
                }
                worldTile.Road = road;
                worldTile.Road.AddTileToList(worldTile);
                worldTile.Road.RoadDirectionInPos.TryAdd(worldTile.Position, WorldDirection.Bottom);

                FindPathToCityAndCreateRoad(worldTile, closestCityTile);
            }
            if (direction == Direction.Left)
            {
                WorldTile worldTile = tile.Left;
                if (worldTile.SiteInfluence is not null)
                {
                    worldTile.SiteInfluence.Roads.Add(road);

                    return;
                }

                worldTile.Road = road;
                worldTile.Road.AddTileToList(worldTile);
                worldTile.Road.RoadDirectionInPos.TryAdd(worldTile.Position, WorldDirection.Left);

                FindPathToCityAndCreateRoad(worldTile, closestCityTile);
            }
            if (direction == Direction.Right)
            {
                WorldTile worldTile = tile.Right;
                if (worldTile.SiteInfluence is not null)
                {
                    worldTile.SiteInfluence.Roads.Add(road);

                    return;
                }

                worldTile.Road = road;
                worldTile.Road.AddTileToList(worldTile);
                worldTile.Road.RoadDirectionInPos.TryAdd(worldTile.Position, WorldDirection.Right);

                FindPathToCityAndCreateRoad(worldTile, closestCityTile);
            }
            if (direction == Direction.UpLeft)
            {
                WorldTile worldTile = tile.TopLeft;
                if (worldTile.SiteInfluence is not null)
                {
                    worldTile.SiteInfluence.Roads.Add(road);

                    return;
                }

                worldTile.Road = road;
                worldTile.Road.AddTileToList(worldTile);
                worldTile.Road.RoadDirectionInPos.TryAdd(worldTile.Position, WorldDirection.BottomLeft);

                FindPathToCityAndCreateRoad(worldTile, closestCityTile);
            }
            if (direction == Direction.UpRight)
            {
                WorldTile worldTile = tile.TopRight;
                if (worldTile.SiteInfluence is not null)
                {
                    worldTile.SiteInfluence.Roads.Add(road);

                    return;
                }

                worldTile.Road = road;
                worldTile.Road.AddTileToList(worldTile);
                worldTile.Road.RoadDirectionInPos.TryAdd(worldTile.Position, WorldDirection.TopRight);

                FindPathToCityAndCreateRoad(worldTile, closestCityTile);
            }
            if (direction == Direction.DownLeft)
            {
                WorldTile worldTile = tile.BottomLeft;
                if (worldTile.SiteInfluence is not null)
                {
                    worldTile.SiteInfluence.Roads.Add(road);

                    return;
                }

                worldTile.Road = road;
                worldTile.Road.AddTileToList(worldTile);
                worldTile.Road.RoadDirectionInPos.TryAdd(worldTile.Position, WorldDirection.BottomLeft);

                FindPathToCityAndCreateRoad(worldTile, closestCityTile);
            }
            if (direction == Direction.DownRight)
            {
                WorldTile worldTile = tile.BottomRight;
                if (worldTile.SiteInfluence is not null)
                {
                    worldTile.SiteInfluence.Roads.Add(road);

                    return;
                }

                worldTile.Road = road;
                worldTile.Road.AddTileToList(worldTile);
                worldTile.Road.RoadDirectionInPos.TryAdd(worldTile.Position, WorldDirection.BottomRight);

                FindPathToCityAndCreateRoad(worldTile, closestCityTile);
            }
            if (direction == Direction.None)
            {
                // Should theoritically never happen, but who knows!
                return;
            }
        }
    }
}