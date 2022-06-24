using MagiRogue.Data;
using MagiRogue.Entities;
using MagiRogue.GameSys.Planet;
using MagiRogue.GameSys.Tiles;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagiRogue.GameSys.MapGen
{
    public class WildernessGenerator : MapGenerator
    {
        #region PlanetMapGenStuff

        public WildernessGenerator()
        {
            //Empty const
        }

        public Map[] GenerateMapWithWorldParam(PlanetMap worldMap, Point posGenerated)
        {
            Map[] maps = new Map[RegionChunk.MAX_LOCAL_MAPS];
            WorldTile worldTile = worldMap.AssocietatedMap.GetTileAt<WorldTile>(posGenerated);

            for (int i = 0; i < maps.Length; i++)
            {
                Map completeMap = DetermineBiomeLookForTile(worldTile);
                if (completeMap is not null)
                {
                    ApplyModifierToTheMap(completeMap, worldTile, (uint)i);
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
        private static void FinishingTouches(Map completeMap, WorldTile worldTile)
        {
            // here prune trees
            if (worldTile.CivInfluence is not null)
            {
                PruneTrees(completeMap, worldTile);
                MakeRoomUseful(completeMap, worldTile);
            }
        }

        private static void MakeRoomUseful(Map completeMap, WorldTile worldTile)
        {
            List<Room> mapRooms = completeMap.Rooms;

            foreach (Room room in mapRooms)
            {
                if (room.RoomRectangle.Area >= 8)
                {
                    int rng = GoRogue.Random
                        .GlobalRandom.DefaultRNG.NextInt(Enum.GetValues(typeof(RoomTag)).Length);
                    room.Tag = (RoomTag)rng;
                    GiveRoomFurniture(room, completeMap);
                }
            }
        }

        private static void GiveRoomFurniture(Room room, Map map)
        {
            switch (room.Tag)
            {
                case RoomTag.Generic:
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_table"), room, map);
                    AddFurnitureAtRandomPos(DataManager.QueryFurnitureInData("wood_chair"), room, map);
                    break;

                case RoomTag.Inn:
                    break;

                case RoomTag.Temple:
                    break;

                case RoomTag.Blacksmith:
                    break;

                case RoomTag.Clothier:
                    break;

                case RoomTag.Alchemist:
                    break;

                case RoomTag.Hovel:
                    break;

                case RoomTag.Abandoned:
                    break;

                case RoomTag.House:
                    break;

                case RoomTag.Throne:
                    break;

                case RoomTag.MeetingPlace:
                    break;

                case RoomTag.Kitchen:
                    break;

                case RoomTag.GenericWorkshop:
                    break;

                case RoomTag.Dinner:
                    break;

                default:
                    break;
            }
        }

        private static void AddFurnitureAtRandomPos(Furniture furniture, Room room, Map map)
        {
            var pos = room.ReturnRandomPosRoom();
            if (map.IsTileWalkable(pos))
            {
                furniture.Position = pos;
                map.Add(furniture);
            }
        }

        private static void PruneTrees(Map completeMap, WorldTile worldTile)
        {
            List<TileBase> trees = completeMap.ReturnAllTrees();

            int chanceToRemoveTree = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt
                ((worldTile.CivInfluence.Population)) / 100;
            for (int i = 0; i < trees.Count; i++)
            {
                int rng = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(0, 101);
                if (rng <= chanceToRemoveTree)
                {
                    Point pos = trees[i].Position;
                    var floor = TileEncyclopedia.GenericTreeTrunk(pos);
                    completeMap.SetTerrain(floor);
                }
            }
        }

        /// <summary>
        /// If the map has any modifer, like strong magic aura, cities, roads and particulaly civs
        /// \nAnything that changes the composition of the World Tile map.
        /// Trees also spawn here
        /// </summary>
        /// <param name="completeMap"></param>
        /// <param name="worldTile"></param>
        /// <param name="magicI">Serves to add unique seeds to the map</param>
        private void ApplyModifierToTheMap(Map completeMap, WorldTile worldTile, ulong magicI)
        {
            completeMap.SetSeed((uint)seed, (uint)worldTile.Position.X,
                (uint)worldTile.Position.Y, (uint)magicI);
            // investigate later if it's making it possible to properly reproduce a map
            randNum = new((ulong)completeMap.Seed);

            switch (worldTile.BiomeType)
            {
                case BiomeType.Sea:
                    return; // No modifer to apply here

                case BiomeType.Desert:
                    PlaceVegetations(completeMap,
                        new TileFloor("Cactus", Point.None,
                        "grass", 198, Color.Green, Color.Black));
                    break;

                case BiomeType.Savanna:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.TropicalRainforest:
                    PlaceGenericTrees(completeMap);
                    break;

                case BiomeType.Grassland:
                    PlaceVegetations(completeMap,
                        new TileFloor("Shrub", Point.None,
                            "grass", '"', Color.Green, Color.Black));
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
                    PutRngFloorTileThere(completeMap, TileEncyclopedia.GenericGrass(Point.None));
                    break;

                default:
                    break;
            }
            if (worldTile.CivInfluence is not null)
            {
                CityGenerator city = new();
                city.GenerateSmallVillageFromMapBSP(completeMap,
                    randNum.NextInt(6, 23), randNum.NextInt(4, 7), randNum.NextInt(8, 12), "Test Town");
            }
            if (worldTile.Rivers.Count > 0)
            {
            }
            if (worldTile.Road is not null)
            {
            }
            if (worldTile.MagicalAuraStrength > 7)
            {
            }
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

        private static Map DetermineBiomeLookForTile(WorldTile worldTile)
        {
            if (worldTile is null)
                return null;
            Map map = worldTile.BiomeType switch
            {
                BiomeType.Sea => GenericSeaMap(worldTile),
                BiomeType.Desert => GenericDesertMap(worldTile),
                BiomeType.Savanna => GenericSavannaMap(worldTile),
                BiomeType.TropicalRainforest => GenericTropicalRainforest(worldTile),
                BiomeType.Grassland => GenericGrassland(worldTile),
                BiomeType.Woodland => GenericWoodLands(worldTile),
                BiomeType.SeasonalForest => GenericSeasonalForests(worldTile),
                BiomeType.TemperateRainforest => GenericTemperateRainForest(worldTile),
                BiomeType.BorealForest => GenericBorealForest(worldTile),
                BiomeType.Tundra => GenericTundra(worldTile),
                BiomeType.Ice => GenericIceMap(worldTile),
                BiomeType.Mountain => GenericMountainMap(worldTile),
                _ => throw new Exception("Cound't find the biome to generate a map!"),
            };
            return map;
        }

        #region BiomeMaps

        private static Map GenericMountainMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");

            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = new TileFloor(pos);
                PrepareForAnyFloor(tile, map);
            }
            return map;
        }

        private static Map GenericIceMap(WorldTile worldTile)
        {
            Map map = new Map($"{worldTile.BiomeType}");
            for (int i = 0; i < map.Tiles.Length; i++)
            {
                Point pos = Point.FromIndex(i, map.Width);
                TileFloor tile = (TileFloor)DataManager.QueryTileInData("snow_floor");
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
                TileFloor tile = new TileFloor("Sand", pos, "sand", worldTile.Glyph,
                    worldTile.Foreground, Color.Transparent);
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

        #endregion BiomeMaps

        #endregion PlanetMapGenStuff
    }
}