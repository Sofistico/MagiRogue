using Arquimedes.Enumerators;
using MagusEngine.Core;
using MagusEngine.Core.MapStuff;
using MagusEngine.Components.TilesComponents;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using MagusEngine.Services.Factory;

namespace MagusEngine.Generators.MapGen
{
    // TODO: Take a closer look
    public class WildernessGenerator : MapGenerator
    {
        #region PlanetMapGenStuff

        public WildernessGenerator()
        {
            //Empty const
        }

        public MagiMap[] GenerateMapWithWorldParam(PlanetMap worldMap, Point posGenerated)
        {
            MagiMap[] maps = new MagiMap[RegionChunk.MAX_LOCAL_MAPS];
            var worldTile = worldMap.AssocietatedMap.GetTileAt<WorldTile>(posGenerated).GetComponent<WorldTile>();
            int settlmentCounter = 0;
            for (int i = 0; i < maps.Length; i++)
            {
                MagiMap completeMap = DetermineBiomeLookForTile(worldTile);
                if (completeMap is not null)
                {
                    ApplyModifierToTheMap(completeMap, worldTile, (uint)i);
                    // Revist when the mind is better working! i really need to sleep my little 🐴
                    settlmentCounter = CreateSitesIfAny(completeMap,
                        worldTile, settlmentCounter);
                    FinishingTouches(completeMap, worldTile);
                    maps[i] = completeMap;
                }
                else
                {
                    throw new Exception("Map was null when generating for the chunck!");
                }
            }

            ConnectMapsInsideChunk(maps);

            return maps;
        }

        /// <summary>
        /// Tweking the final result to be "better"
        /// </summary>
        /// <param name="completeMap"></param>
        /// <param name="worldTile"></param>
        private void FinishingTouches(MagiMap completeMap, WorldTile worldTile)
        {
            // here prune trees
            if (worldTile.Parent.GetComponent<SiteTile>(out var _))
            {
                PruneTrees(completeMap, worldTile);
                MakeRoomsUseful(completeMap);
            }
        }

        private static void PruneTrees(MagiMap completeMap, WorldTile worldTile)
        {
            List<Tile> trees = completeMap.ReturnAllTrees();

            int chanceToRemoveTree = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt
                (worldTile.Parent.GetComponent<SiteTile>().SiteInfluence.ReturnPopNumber()) / 100;
            for (int i = 0; i < trees.Count; i++)
            {
                int rng = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(0, 101);
                if (rng <= chanceToRemoveTree)
                {
                    Point pos = trees[i].Position;
                    var floor = TileFactory.GenericTreeTrunk(pos);
                    completeMap.SetTerrain(floor);
                }
            }
        }

        /// <summary>
        /// If the map has any modifer, like strong magic aura, cities, roads and particulaly civs
        /// Anything that changes the composition of the World Tile map. Trees also spawn here
        /// </summary>
        /// <param name="completeMap"></param>
        /// <param name="worldTile"></param>
        /// <param name="magicI">Serves to add unique seeds to the map</param>
        private void ApplyModifierToTheMap(MagiMap completeMap, WorldTile worldTile, ulong magicI)
        {
            completeMap.SetSeed((uint)seed, (uint)worldTile.Position.X,
                (uint)worldTile.Position.Y, (uint)magicI);
            // investigate later if it's making it possible to properly reproduce a map
            randNum = new(completeMap.Seed);

            switch (worldTile.BiomeType)
            {
                case BiomeType.Sea:
                    return; // No modifer to apply here

                case BiomeType.Desert:
                    PlaceVegetations(completeMap,
                        new Tile(Color.Green, Color.Black, (char)198, false, false, Point.None, "Cactus", "grass"));
                    break;

                case BiomeType.Savanna:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.TropicalRainforest:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.Grassland:
                    PlaceVegetations(completeMap,
                        new Tile(Color.Green, Color.Black, '"', true, true, Point.None, "Shrub", "grass"));
                    break;

                case BiomeType.Woodland:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.SeasonalForest:
                    PlaceGenericTrees(completeMap);

                    break;

                case BiomeType.TemperateRainforest:
                    PlaceGenericTrees(completeMap);

                    break;

                case BiomeType.BorealForest:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.Tundra:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.Ice:
                    break; // null as well

                case BiomeType.Mountain:
                    PutRngFloorTileThere(completeMap, TileFactory.GenericGrass(Point.None));
                    break;

                default:
                    break;
            }

            //if (worldTile.Rivers.Count > 0)
            //{
            //}
            //if (worldTile.Road is not null)
            //{
            //}
            //if (worldTile.MagicalAuraStrength > 7)
            //{
            //}
            /*switch (worldTile.SpecialLandType)
            {
                case SpecialLandType.None:
                    // nothing here
                    break;

                case SpecialLandType.MagicLand:
                    // funky shit full of interisting stuff
                    break;

                default:
                    break;
            }*/
        }

        /// <summary>
        /// </summary>
        /// <param name="completeMap"></param>
        /// <param name="worldTile"></param>
        /// <param name="createdSites"></param>
        /// <returns>Returns the number of Site already created on the map</returns>
        /// <exception cref="ApplicationException"></exception>
        private int CreateSitesIfAny(MagiMap completeMap, WorldTile worldTile, int createdSites)
        {
            if (worldTile.Parent.GetComponent<SiteTile>(out var siteTile))
            {
                SiteGenerator city = new();
                var site = siteTile.SiteInfluence;
                if ((int)site.Size == createdSites)
                    return (int)site.Size;
                switch (site.Size)
                {
                    case SiteSize.None:
                        throw new ApplicationException("Room generated was from the default error!");

                    case SiteSize.Small:
                        city.GenerateSmallSite(completeMap,
                            randNum.NextInt(4, 7),
                            randNum.NextInt(4, 7),
                            randNum.NextInt(8, 12),
                            site.Name,
                            site.Buildings);
                        break;

                    case SiteSize.Medium:
                        city.GenerateMediumSite(completeMap,
                            randNum.NextInt(4, 7),
                            randNum.NextInt(4, 7),
                            randNum.NextInt(8, 12),
                            site.Name,
                            site.Buildings);
                        break;

                    case SiteSize.Large:
                        city.GenerateBigSite(completeMap,
                            randNum.NextInt(17, 30),
                            randNum.NextInt(4, 7),
                            randNum.NextInt(8, 12),
                            site.Name,
                            site.Buildings);
                        break;

                    default:
                        throw new ApplicationException("There was an error with the room generated!");
                }

                return ++createdSites;
            }

            return 0;
        }

        private static MagiMap DetermineBiomeLookForTile(WorldTile worldTile)
        {
            if (worldTile is null)
                return null;
            return worldTile.BiomeType switch
            {
                BiomeType.Sea => GenericBiomeMap(worldTile, "h2o"),
                BiomeType.Desert => GenericBiomeMap(worldTile, "sand"),
                BiomeType.Ice => GenericBiomeMap(worldTile, "ice"),
                BiomeType.Mountain => GenericBiomeMap(worldTile, null, MaterialType.Stone, true),
                _ => GenericBiomeMap(worldTile, "dirt"),
            };
        }

        #region BiomeMaps

        /*private static Map GenericMountainMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Terrain.Count; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                Tile tile = TileFactory.GenericStoneFloor(pos);
                PrepareForAnyFloor(tile, map);
            }
            return map;
        }

        private static Map GenericIceMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");
            for (int i = 0; i < map.Terrain.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                Tile tile = DataManager.QueryTileInData("snow_floor");
                tile.Position = pos;
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericTundra(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericBorealForest(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericTemperateRainForest(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericSeasonalForests(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericWoodLands(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericGrassland(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericTropicalRainforest(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericSavannaMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = TileEncyclopedia.GenericGrass(pos);
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericDesertMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = DataManager.QueryTileInData<TileFloor>("t_sand").Copy();
                tile.Position = pos;
                PrepareForAnyFloor(tile, map);
            }

            return map;
        }

        private static Map GenericSeaMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                WaterTile tile = WaterTile.NormalSeaWater(pos);
                FloodWithWaterMap(tile, map);
            }

            return map;
        }
        */

        private static MagiMap GenericBiomeMap(WorldTile worldTile,
            string materialId = null,
            MaterialType typeToMake = MaterialType.None,
            bool useCacheMaterial = false)
        {
            MagiMap map = new MagiMap($"{worldTile.BiomeType}");
            // create a height map in the future
            for (int i = 0; i < map.Terrain.Count; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                Tile tile = TileFactory.CreateTile(pos, TileType.Floor, materialId, typeToMake, useCacheMaterial);
                PrepareForAnyFloor(tile, map);
            }
            if (useCacheMaterial)
                TileFactory.ResetCachedMaterial();
            return map;
        }

        #endregion BiomeMaps

        #endregion PlanetMapGenStuff
    }
}
