using GoRogue.Pathing;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Civ;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.Planet.History
{
    public class AccumulatedHistory
    {
        private int roadId;
        private PlanetMap planetData;

        #region Consts

        private const int wealthToCreateRoad = 100;
        private const int wealthToCreateNewSettlement = 1000;

        #endregion Consts

        public List<HistoricalFigure> Figures { get; set; }
        public List<Civilization> Civs { get; set; }
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

        public void RunHistory(Civilization[] civilizations, int yearToGameBegin,
            PlanetMap planet, WorldTile[,] tiles)
        {
            Civs = civilizations.ToList();
            planetData = planet;
            bool firstYearOnly = true;
            MythGenerator mythGenerator = new MythGenerator();
            Myths = mythGenerator.GenerateMyths(
                Data.DataManager.ListOfRaces.ToList(),
                Figures,
                planet);

            while (Year < yearToGameBegin)
            {
                for (int i = 0; i < Civs.Count; i++)
                {
                    var civ = Civs[i];

                    // Historical figure simulation here:
                    civ.SimulateImportantStuff();

                    if (i < Civs.Count - 1)
                    {
                        var otherCivs = Civs.Where(i => i != civ);
                        // interaction between civs here!
                        foreach (Civilization nextCiv in otherCivs)
                        {
                            // stuff that will only happen in the first year!
                            // after that won't really happen anymore!
                            if (firstYearOnly)
                            {
                                civ.SetupInitialHistoricalFigures();
                                if (civ.Tendency == nextCiv.Tendency)
                                {
                                    civ.AddCivToRelations(nextCiv, RelationType.Friendly);
                                }
                                else
                                {
                                    civ.AddCivToRelations(nextCiv, RelationType.Neutral);
                                }
                            }
                            if (civ.Wealth > wealthToCreateRoad)
                            {
                                if (BuildRoadsToFriends(civ, nextCiv, tiles))
                                    civ.Wealth -= wealthToCreateRoad;
                            }
                        }
                    }
                    int totalRevenueYear = 0;

                    if (civ.Wealth > wealthToCreateNewSettlement)
                    {
                        int migrants = GameLoop.GlobalRand.NextInt(10, 100);
                        Settlement rngSettl = civ.Settlements.GetRandomItemFromList();
                        rngSettl.Population -= migrants;
                        Point pos = rngSettl.WorldPos.GetPointNextTo();
                        Settlement settlement = new Settlement(pos,
                            RandomNames.RandomNamesFromLanguage(civ.GetLanguage()),
                            migrants);
                        WorldTile tile = tiles[pos.X, pos.Y];
                        tile.CivInfluence = civ;
                        civ.AddSettlementToCiv(settlement);
                    }

                    // settlement simulation from the Civ here:
                    foreach (Settlement settlement in civ.Settlements)
                    {
                        settlement.CreateNewBuildings();
                        totalRevenueYear += settlement.MundaneResources;
                        settlement.GenerateMundaneResources();
                        settlement.SimulatePopulationGrowth(tiles[settlement.WorldPos.X, settlement.WorldPos.Y]);
                    }
                    civ.Wealth += totalRevenueYear;
                }

                Year++;
                firstYearOnly = false;
            }
        }

        private bool BuildRoadsToFriends(Civilization civ, Civilization friend, WorldTile[,] tiles)
        {
            if (civ.Relations.Any(i => i.OtherCivId.Equals(friend.Id)
                && i.Relation is RelationType.Friendly && i.RoadBuilt))
                return false;
            if (civ[friend.Id].RoadBuilt)
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
                RoadId = roadId++
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
                if (worldTile.CivInfluence is not null)
                {
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
                if (worldTile.CivInfluence is not null)
                {
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
                if (worldTile.CivInfluence is not null)
                {
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
                if (worldTile.CivInfluence is not null)
                {
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
                if (worldTile.CivInfluence is not null)
                {
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
                if (worldTile.CivInfluence is not null)
                {
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
                if (worldTile.CivInfluence is not null)
                {
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
                if (worldTile.CivInfluence is not null)
                {
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