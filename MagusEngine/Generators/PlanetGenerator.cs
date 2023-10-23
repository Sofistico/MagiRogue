using Arquimedes.Enumerators;
using GoRogue.DiceNotation;
using MagusEngine.Core.Civ;
using MagusEngine.Core.MapStuff;
using MagusEngine.Core.WorldStuff;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Enums;
using MagusEngine.Utils.Noise.AccidentalNoiseLibrary.Implicit;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

// take a look at the historygen-sad, that's quite interesting
// TODO: REDO!
namespace MagusEngine.Generators
{
    /// <summary>
    /// Generates a brand new planet
    /// </summary>
    public sealed class PlanetGenerator
    {
        #region Fields

        #region MapGenField

        private int _width;
        private int _height;
        private List<Civilization>? _civilizations;
        private int maxCivsWorld;

        private readonly float deepWater = 0.3f;
        private readonly float shallowWater = 0.5f;
        private readonly float sand = 0.6f;
        private readonly float grass = 0.7f;
        private readonly float forest = 0.8f;
        private readonly float rock = 0.9f;
        private readonly float snow = 0.95f;

        private readonly float coldestValue = 0.1f;
        private readonly float colderValue = 0.18f;
        private readonly float coldValue = 0.4f;
        private readonly float warmValue = 0.6f;
        private readonly float warmerValue = 0.8f;

        private readonly int terrainOctaves = 6;
        private readonly float terrainFrequency = 4.59f;
        private readonly float heatFrequency = 5.90f;
        private readonly int heatOctaves = 4;

        private readonly double gradientCenter = 0.5; // Center of the gradient
        private readonly double steepness = 10.0; // Adjust the steepness as needed

        private readonly int moistureOctaves = 7;
        private readonly float moistureFrequency = 4.22f;

        private readonly float dryerValue = 0.27f;
        private readonly float dryValue = 0.4f;
        private readonly float wetValue = 0.6f;
        private readonly float wetterValue = 0.8f;
        private readonly float wettestValue = 0.9f;

        // Rivers
        private readonly int riverMaxCount = 40;
        private readonly int maxRiverAttempts = 1000;
        private readonly int maxRecursiveRiverAttemepts = 750;
        private int currentRecursiveRiverAttempt = 0;
        private readonly float minRiverHeight = 0.8f;
        private readonly int minRiverTurns = 18;
        private readonly int _minRiverLength = 20;
        private readonly int maxRiverIntersections = 2;

        private readonly List<WorldTileGroup> waters = new();
        private readonly List<WorldTileGroup> lands = new();

        private int seed;

        // noise generator
        private ImplicitFractal heightMap;
        private ImplicitCombiner heatMap;
        private ImplicitFractal moistureMap;

        // Planet data
        private PlanetMap planetData;

        // final object
        private WorldTile[,] tiles;

        private float[,] HeightData;

        private float[,] HeatData;

        private float[,] MoistureData;

        private readonly BiomeType[,] biomeTable = new BiomeType[6, 6] {
            //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
            };

        #endregion MapGenField

        #endregion Fields

        /// <summary>
        /// Creates a brand new planet!
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public PlanetMap CreatePlanet(int width, int height, int yearsToGenerate = 100, int nmbCivilizations = 30)
        {
            _width = width;
            _height = height;
            maxCivsWorld = nmbCivilizations;
            _civilizations = new();
            HeightData = new float[width, height];
            HeatData = new float[width, height];
            MoistureData = new float[width, height];

            // Initialize the generator
            Initialize();

            // Build the height map
            GetData(ref planetData);

            // Build our final objects based on our data
            LoadTiles();

            UpdateNeighbors();

            GenerateRivers();
            DigRiverGroups();
            AdjustMoistureMap();

            UpdateBitmasks();
            FloodFill();

            GenerateBiomeMap();
            UpdateBiomeBitmask();

            SeedCivilizations();
            BasicHistory(yearsToGenerate);

            // Here will take care of the visualization
            CreateConsole(tiles);

            return planetData;
        }

        #region Civ

        private void BasicHistory(int yearToGameBegin)
        {
            AccumulatedHistory history = new AccumulatedHistory()
            {
                Year = yearToGameBegin,
            };

            history.RunHistory(_civilizations, yearToGameBegin, planetData, tiles);
            planetData.WorldHistory = history;
        }

        private void SeedCivilizations()
        {
            int tries = 0;
            int maxTries = 3000;
            int currentCivCount = 0;

            while (currentCivCount < maxCivsWorld && tries < maxTries)
            {
                tries++;

                int x = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(0, _width);
                int y = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(0, _height);
                WorldTile tile = tiles[x, y];
                Tile Parent = tile.Parent!;

                if (tile.HeightType == HeightType.DeepWater
                    || tile.HeightType == HeightType.ShallowWater
                    || tile.HeightType == HeightType.River)
                    continue;

                if (Parent.GetComponent<SiteTile>(out var site))
                    continue;

                var possibleCivs = DataManager.QueryCultureTemplateFromBiome(tile.BiomeType.ToString());
                Civilization civ;
                if (possibleCivs.Count > 0)
                    civ = possibleCivs.GetRandomItemFromList().ConvertToCivilization();
                else
                    continue;

                int rng = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(25, 50);

                int popNmr = (int)(rng * ((int)tile.HeightType * tile.MoistureValue + 1));
                Site set = new Site(Parent.Position, civ.RandomSiteFromLanguageName(), new Population(popNmr, civ.PrimaryRace.Id))
                {
                    MundaneResources = (int)tile.GetResources()
                };
                var room = new Room(RoomTag.Farm);
                set.Buildings.Add(new Building(room));
                set.DefineSiteSize();

                Parent.AddComponent<SiteTile>(new(set));
                civ.AddSiteToCiv(set);
                if (civ.Territory.Count == 0)
                    throw new Exception();

                if (currentCivCount < maxCivsWorld)
                {
                    _civilizations?.Add(civ);
                }

                planetData.Civilizations.Add(civ);

                currentCivCount++;
            }
        }

        #endregion Civ

        #region Tiles

        private void CreateConsole(WorldTile[,] tiles)
        {
            PlanetGlyphGenerator.SetTile(_width, _height, ref tiles);
            // For Test only!
            //PlanetGlyphGenerator.GetHeatMap(width, height, tiles);
            //PlanetGlyphGenerator.GetMoistureMap(width, height, tiles);
            PlanetGlyphGenerator.GetBiomeMapTexture(_width, _height, tiles);
            PlanetGlyphGenerator.SetSiteTiles(_width, _height, tiles);
#if DEBUG
            PlanetGlyphGenerator.SetSpecialTiles(_width, _height, tiles);
#endif
            planetData.SetWorldTiles(tiles);
        }

        // Build a Tile array from our data
        private void LoadTiles()
        {
            tiles = new WorldTile[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    WorldTile worldTile = new();
                    Tile Parent = new()
                    {
                        Position = new Point(x, y)
                    };

                    float heightValue = HeightData[x, y];
                    worldTile.MineralValue = MathMagi.ReturnPositive(MathMagi.Round
                        ((HeightData[x, y] + HeatData[x, y]) * 200));

                    // normalize value between 0 and 1
                    heightValue = MathMagi.ReturnPositive(heightValue);

                    worldTile.HeightValue = heightValue;

                    //HeightMap Analyze
                    if (heightValue < deepWater)
                    {
                        worldTile.HeightType = HeightType.DeepWater;
                        worldTile.Collidable = false;
                        worldTile.MineralValue *= 1.5f;
                    }
                    else if (heightValue < shallowWater)
                    {
                        worldTile.HeightType = HeightType.ShallowWater;
                        worldTile.Collidable = false;
                        worldTile.MineralValue *= 0.5f;
                    }
                    else if (heightValue < sand)
                    {
                        worldTile.HeightType = HeightType.Sand;
                        worldTile.Collidable = true;
                        worldTile.MineralValue *= 0.7f;
                    }
                    else if (heightValue < grass)
                    {
                        worldTile.HeightType = HeightType.Grass;
                        worldTile.Collidable = true;
                        worldTile.MineralValue *= 0.7f;
                    }
                    else if (heightValue < forest)
                    {
                        worldTile.HeightType = HeightType.Forest;
                        worldTile.Collidable = true;
                        worldTile.MineralValue *= 1.2f;
                    }
                    else if (heightValue < rock)
                    {
                        worldTile.HeightType = HeightType.Mountain;
                        worldTile.Collidable = true;
                        worldTile.MineralValue *= 2.0f;
                    }
                    else if (heightValue < snow)
                    {
                        worldTile.HeightType = HeightType.Snow;
                        worldTile.Collidable = true;
                    }
                    else
                    {
                        worldTile.HeightType = HeightType.HighMountain;
                        worldTile.Collidable = true;
                        worldTile.MineralValue *= 3.0f;
                    }

                    //Moisture Map Analyze
                    float moistureValue = MathMagi.ReturnPositive(MoistureData[x, y]);
                    worldTile.MoistureValue = MathMagi.Round(moistureValue);

                    //adjust moisture based on height
                    if (worldTile.HeightType == HeightType.DeepWater)
                    {
                        MoistureData[Parent.Position.X, Parent.Position.Y] += 8f * worldTile.HeightValue;
                    }
                    else if (worldTile.HeightType == HeightType.ShallowWater)
                    {
                        MoistureData[Parent.Position.X, Parent.Position.Y] += 3f * worldTile.HeightValue;
                    }
                    else if (worldTile.HeightType == HeightType.Shore)
                    {
                        MoistureData[Parent.Position.X, Parent.Position.Y] += 1f * worldTile.HeightValue;
                    }
                    else if (worldTile.HeightType == HeightType.Sand)
                    {
                        MoistureData[Parent.Position.X, Parent.Position.Y] += 0.25f * worldTile.HeightValue;
                    }
                    else if (worldTile.HeightType == HeightType.Snow)
                    {
                        MoistureData[Parent.Position.X, Parent.Position.Y] += 2f * worldTile.HeightValue;
                    }

                    //set moisture type
                    if (moistureValue < dryerValue) worldTile.MoistureType = MoistureType.Dryest;
                    else if (moistureValue < dryValue) worldTile.MoistureType = MoistureType.Dryer;
                    else if (moistureValue < wetValue) worldTile.MoistureType = MoistureType.Dry;
                    else if (moistureValue < wetterValue) worldTile.MoistureType = MoistureType.Wet;
                    else if (moistureValue < wettestValue) worldTile.MoistureType = MoistureType.Wetter;
                    else worldTile.MoistureType = MoistureType.Wettest;

                    if (worldTile.HeightType == HeightType.Forest)
                    {
                        HeatData[Parent.Position.X, Parent.Position.Y] -= 0.1f * worldTile.HeightValue;
                    }
                    else if (worldTile.HeightType == HeightType.Mountain)
                    {
                        HeatData[Parent.Position.X, Parent.Position.Y] -= 0.25f * worldTile.HeightValue;
                    }
                    else if (worldTile.HeightType == HeightType.HighMountain)
                    {
                        HeatData[Parent.Position.X, Parent.Position.Y] -= 0.5f * worldTile.HeightValue;
                    }
                    else if (worldTile.HeightType == HeightType.Snow)
                    {
                        HeatData[Parent.Position.X, Parent.Position.Y] -= 0.7f * worldTile.HeightValue;
                    }
                    else
                    {
                        HeatData[Parent.Position.X, Parent.Position.Y] += 0.01f * worldTile.HeightValue;
                    }

                    // Set heat value
                    float heatModValue = MathMagi.ReturnPositive(HeatData[Parent.Position.X, Parent.Position.Y]);
                    worldTile.HeatValue = heatModValue;

                    // set heat type
                    if (heatModValue < coldestValue) worldTile.HeatType = HeatType.Coldest;
                    else if (heatModValue < colderValue) worldTile.HeatType = HeatType.Colder;
                    else if (heatModValue < coldValue) worldTile.HeatType = HeatType.Cold;
                    else if (heatModValue < warmValue) worldTile.HeatType = HeatType.Warm;
                    else if (heatModValue < warmerValue) worldTile.HeatType = HeatType.Warmer;
                    else worldTile.HeatType = HeatType.Warmest;

                    worldTile.MagicalAuraStrength = Dice.Roll("2d50 / 10");

                    if (worldTile.MagicalAuraStrength >= 10)
                        worldTile.SpecialLandType = SpecialLandType.MagicLand;

                    Parent.AddComponent(worldTile);
                    tiles[x, y] = worldTile;
                }
            }
        }

        #endregion Tiles

        #region HelpMethods

        // Extract data from a noise module
        private void GetData(ref PlanetMap planetData)
        {
            planetData = new PlanetMap(_width, _height);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    //Noise range
                    // 1 to 2 makes the poles straight
                    // -1 to 1 makes extreme straigh lines
                    // -1 to 2 makes circular world
                    // -1 to 3 makes lots of archipelagos
                    const float x1 = 1, x2 = 2;
                    const float y1 = 1, y2 = 2;
                    const float dx = x2 - x1;
                    const float dy = y2 - y1;

                    //Sample noise at smaller intervals
                    float s = x / (float)_width;
                    float t = y / (float)_height;

                    // Calculate our 2D coordinates
                    float nx = x1 + (MathF.Cos(s * 2) * dx);
                    float ny = y1 + (MathF.Sin(t * 2) * dy);

                    float heightValue = (float)heightMap.Get(nx, ny);

                    var sigmoid = 1.0 / (1.0 + Math.Exp(-steepness * (y - gradientCenter)));
                    var heatNoise = heatMap.Get(x, y);
                    // Add the noise value to the gradient
                    var val = sigmoid + heatNoise - 0.5;

                    float heatValue = (float)val;
                    float moistureValue = (float)moistureMap.Get(nx, ny);

                    HeightData[x, y] = heightValue;
                    HeatData[x, y] = heatValue;
                    MoistureData[x, y] = moistureValue;
                }
            }
        }

        private void Initialize()
        {
            seed = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(0, int.MaxValue);
            heightMap = new(FractalType.Multi,
                                       BasisType.Simplex,
                                       InterpolationType.Quintic,
                                       terrainOctaves,
                                       terrainFrequency,
                                       seed);

            var heatGradient = new ImplicitGradient(1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1);
            var heatFractal = new ImplicitFractal(
                FractalType.Multi,
                BasisType.Simplex,
                InterpolationType.Quintic,
                heatOctaves,
                heatFrequency,
                seed
                );

            heatMap = new(CombinerType.Multiply);
            heatMap.AddSource(heatGradient);
            heatMap.AddSource(heatFractal);

            moistureMap = new(FractalType.Multi, BasisType.Simplex, InterpolationType.Quintic,
                moistureOctaves, moistureFrequency, seed);
        }

        // need to get the mod so that it doesn't pick up a tile outside of the map.
        private WorldTile GetTop(WorldTile center)
        {
            return tiles[center.Parent.Position.X, MathMagi.Mod(center.Parent.Position.Y - 1, _height)];
        }

        private WorldTile GetBottom(WorldTile t)
        {
            return tiles[t.Parent.Position.X, MathMagi.Mod(t.Parent.Position.Y + 1, _height)];
        }

        private WorldTile GetLeft(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Parent.Position.X - 1, _width), t.Parent.Position.Y];
        }

        private WorldTile GetRight(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Parent.Position.X + 1, _width), t.Parent.Position.Y];
        }

        private WorldTile GetTopRight(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Parent.Position.X + 1, _width), MathMagi.Mod(t.Parent.Position.Y - 1, _width)];
        }

        private WorldTile GetBottomRight(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Parent.Position.X + 1, _width), MathMagi.Mod(t.Parent.Position.Y + 1, _width)];
        }

        private WorldTile GetTopLeft(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Parent.Position.X + -1, _width), MathMagi.Mod(t.Parent.Position.Y + 1, _width)];
        }

        private WorldTile GetBottomLeft(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Parent.Position.X - 1, _width), MathMagi.Mod(t.Parent.Position.Y - 1, _width)];
        }

        private void UpdateNeighbors()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    WorldTile tile = tiles[x, y];

                    tile.Top = GetTop(tile);
                    tile.Bottom = GetBottom(tile);
                    tile.Left = GetLeft(tile);
                    tile.Right = GetRight(tile);
                    tile.TopRight = GetTopRight(tile);
                    tile.TopLeft = GetTopLeft(tile);
                    tile.BottomRight = GetBottomRight(tile);
                    tile.BottomLeft = GetBottomLeft(tile);
                }
            }
        }

        private void UpdateBitmasks()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    tiles[x, y].UpdateBitmask();
                }
            }
        }

        private void FloodFill()
        {
            Stack<WorldTile> stack = new();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    WorldTile t = tiles[x, y];

                    //Tile already flood filled, skip
                    if (t.FloodFilled)
                        continue;

                    // Land
                    if (t.Collidable)
                    {
                        WorldTileGroup landGroup = new();
                        landGroup.Type = TileGroupType.Land;

                        stack.Push(t);

                        while (stack.Count > 0)
                        {
                            FloodFill(stack.Pop(), ref landGroup, ref stack);
                        }

                        if (landGroup.WorldTiles.Count > 0)
                        {
                            lands.Add(landGroup);
                        }
                    }
                    else
                    {
                        WorldTileGroup waterGroup = new();

                        waterGroup.Type = TileGroupType.Water;
                        stack.Push(t);

                        while (stack.Count > 0)
                        {
                            FloodFill(stack.Pop(), ref waterGroup, ref stack);
                        }

                        if (waterGroup.WorldTiles.Count > 0)
                        {
                            waters.Add(waterGroup);
                        }
                    }
                }
            }
        }

        private void FloodFill(WorldTile worldTile,
            ref WorldTileGroup group,
            ref Stack<WorldTile> stack)
        {
            if (worldTile.FloodFilled)
                return;
            if (group.Type == TileGroupType.Land && !worldTile.Collidable)
                return;
            if (group.Type == TileGroupType.Water && worldTile.Collidable)
                return;

            // Add to TileGroup
            group.WorldTiles.Add(worldTile);
            worldTile.FloodFilled = true;

            WorldTile t = GetTop(worldTile);

            // floodfill into neighbors
            if (!t.FloodFilled && worldTile.Collidable == t.Collidable)
                stack.Push(t);
            t = GetBottom(worldTile);
            if (!t.FloodFilled && worldTile.Collidable == t.Collidable)
                stack.Push(t);
            t = GetLeft(worldTile);
            if (!t.FloodFilled && worldTile.Collidable == t.Collidable)
                stack.Push(t);
            t = GetRight(worldTile);
            if (!t.FloodFilled && worldTile.Collidable == t.Collidable)
                stack.Push(t);
        }

        private void GenerateRivers()
        {
            int attempts = 0;
            int count = riverMaxCount;

            // generate some rivers
            while (count > 0 && attempts < maxRiverAttempts)
            {
                // get random tiles
                int x = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(0, _width);
                int y = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(0, _height);
                WorldTile tile = tiles[x, y];

                // validate the tile
                if (!tile.Collidable) continue;
                if (planetData.Rivers.Any(i => i.Points.Contains(tile.Position))) continue;
                Direction currentDirection;
                if (tile.HeightValue > minRiverHeight)
                {
                    // tile is good to start river from
                    River river = new(SequentialIdGenerator.RiverId);

                    // Figure out the direction this river will try to flow
                    currentDirection = tile.GetLowestNeighbor();

                    // Recursively find a path to water
                    FindPathToWater(tile, currentDirection, ref river);

                    if (river.Points.Count >= _minRiverLength)
                    {
                        //Validation passed - Add river to list
                        planetData.Rivers.Add(river);
                        count--;
                        tile.HeightValue -= 0.1f; // channel into the mountain
                    }
                }
                attempts++;
            }
        }

        // Worried with this method, the recursion is throwing too many errors
        private void FindPathToWater(WorldTile tile, Direction currentDirection, ref River river)
        {
            currentRecursiveRiverAttempt++;
            if (currentRecursiveRiverAttempt >= maxRecursiveRiverAttemepts)
                return;

            if (planetData.Rivers.Contains(river))
                return;

            if (planetData.Rivers.Any(i => i.Points.Contains(tile.Position))) { }
            river.AddIntersection(tile.Position, _width);

            river.AddTile(tile);

            try
            {
                currentDirection = tile.GetLowestNeighbor();
                if (currentDirection == Direction.None)
                    return;
                // override flow direction if a tile is significantly lower
                //if (currentDirection == Direction.Left && MathF.Abs(rigthValue - leftValue) < 0.1f)
                //    rigthValue = int.MaxValue;
                //if (currentDirection == Direction.Right && MathF.Abs(rigthValue - leftValue) < 0.1f)
                //    leftValue = int.MaxValue;
                //if (currentDirection == Direction.Up && MathF.Abs(topValue - bottomValue) < 0.1f)
                //    topValue = int.MaxValue;
                //if (currentDirection == Direction.Down && MathF.Abs(topValue - bottomValue) < 0.1f)
                //    bottomValue = int.MaxValue;

                // find mininum has god forsaken us?
                //float min = MathF.Min(MathF.Min(MathF.Min(leftValue, rigthValue), topValue), bottomValue);

                //// if no minimum found - exit
                //if (min == int.MaxValue)
                //    return;

                //Move to next neighbor
                //if (min == leftValue)
                //{
                //    if (left.Collidable)
                //    {
                //        if (river.CurrentDirection != Direction.Left)
                //        {
                //            river.TurnCount++;
                //            river.CurrentDirection = Direction.Left;
                //        }
                //        FindPathToWater(left, currentDirection, ref river);
                //    }
                //}
                //if (min == rigthValue)
                //{
                //    if (right.Collidable)
                //    {
                //        if (river.CurrentDirection != Direction.Right)
                //        {
                //            river.TurnCount++;
                //            river.CurrentDirection = Direction.Right;
                //        }
                //        FindPathToWater(right, currentDirection, ref river);
                //    }
                //}
                //if (min == bottomValue)
                //{
                //    if (bottom.Collidable)
                //    {
                //        if (river.CurrentDirection != Direction.Down)
                //        {
                //            river.TurnCount++;
                //            river.CurrentDirection = Direction.Down;
                //        }
                //        FindPathToWater(bottom, currentDirection, ref river);
                //    }
                //}
                //if (min == topValue)
                //{
                //    if (top.Collidable)
                //    {
                //        if (river.CurrentDirection != Direction.Up)
                //        {
                //            river.TurnCount++;
                //            river.CurrentDirection = Direction.Up;
                //        }
                //        FindPathToWater(top, currentDirection, ref river);
                //    }
                //}
                FindPathToWater(tile.Directions[currentDirection], currentDirection, ref river);
            }
            catch
            {
                throw new Exception("The recursive method to find the river path failed!");
            }
        }

        /*private void BuildRiverGroups()
        {
            //loop each tile, checking if it belongs to multiple rivers
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    WorldTile t = tiles[x, y].GetComponent<WorldTile>();

                    if (t.Rivers.Count > 1)
                    {
                        // multiple rivers == intersection
                        RiverGroup? group = null;

                        // Does a rivergroup already exist for this group?
                        for (int n = 0; n < t.Rivers.Count; n++)
                        {
                            River tileriver = t.Rivers[n];
                            for (int i = 0; i < riverGroups.Count; i++)
                            {
                                for (int j = 0; j < riverGroups[i].Rivers.Count; j++)
                                {
                                    River river = riverGroups[i].Rivers[j];
                                    if (river.Id == tileriver.Id)
                                    {
                                        group = riverGroups[i];
                                    }
                                    if (group != null) break;
                                }
                                if (group != null) break;
                            }
                            if (group != null) break;
                        }

                        // existing group found -- add to it
                        if (group != null)
                        {
                            for (int n = 0; n < t.Rivers.Count; n++)
                            {
                                if (!group.Rivers.Contains(t.Rivers[n]))
                                    group.Rivers.Add(t.Rivers[n]);
                            }
                        }
                        else   //No existing group found - create a new one
                        {
                            group = new RiverGroup();
                            for (int n = 0; n < t.Rivers.Count; n++)
                            {
                                group.Rivers.Add(t.Rivers[n]);
                            }
                            riverGroups.Add(group);
                        }
                    }
                }
            }
        }*/

        private void DigRiverGroups()
        {
            for (int i = 0; i < planetData.Rivers.Count; i++)
            {
                River river = planetData.Rivers[i];

                if (river != null)
                {
                    //Dig out longest path first
                    DigRiver(river);
                }
            }
        }

        private void DigRiver(River river)
        {
            int counter = 0;

            // How wide are we digging this river?
            int size = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(1, 5);
            river.Length = river.Points.Count;

            // randomize size change
            int two = river.Length / 2;
            int three = two / 2;
            int four = three / 2;
            int five = four / 2;

            int twomin = two / 3;
            int threemin = three / 3;
            int fourmin = four / 3;
            int fivemin = five / 3;

            // randomize lenght of each size
            int count1 = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(fivemin, five);
            if (size < 4)
            {
                count1 = 0;
            }
            int count2 = count1 + GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(fourmin, four);
            if (size < 3)
            {
                count2 = 0;
                count1 = 0;
            }
            int count3 = count2 + GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(threemin, three);
            if (size < 2)
            {
                count3 = 0;
                count2 = 0;
                count1 = 0;
            }
            int count4 = count3 + GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(twomin, two);

            // Make sure we are not digging past the river path
            if (count4 > river.Length)
            {
                int extra = count4 - river.Length;
                while (extra > 0)
                {
                    if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; } else if (count2 > 0) { count2--; count3--; count4--; extra--; } else if (count3 > 0) { count3--; count4--; extra--; } else if (count4 > 0) { count4--; extra--; }
                }
            }

            // Dig it out
            for (int i = river.Points.Count - 1; i >= 0; i--)
            {
                var pos = river.Points[i];
                WorldTile t = tiles[pos.X, pos.Y];

                t.Collidable = false;
                t.HeightType = HeightType.River;
            }
        }

        private void AdjustMoistureMap()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    WorldTile t = tiles[x, y];
                    if (t.HeightType == HeightType.River)
                    {
                        AddMoisture(t, 60);
                    }
                }
            }
        }

        private void AddMoisture(WorldTile t, int radius)
        {
            Point center = new(t.Parent.Position.X, t.Parent.Position.Y);
            int curr = radius;

            while (curr > 0)
            {
                int x1 = MathMagi.Mod(t.Parent.Position.X - curr, _width);
                int x2 = MathMagi.Mod(t.Parent.Position.X + curr, _width);
                int y = t.Parent.Position.Y;

                AddMoisture(tiles[x1, y],
                    (int)(0.025f / (center - new Point(x1, y)).PointMagnitude()));

                for (int i = 0; i < curr; i++)
                {
                    AddMoisture(tiles[x1, MathMagi.Mod(y + i + 1, _height)],
                        (int)(0.025f / (center - new Point(x1,
                        MathMagi.Mod(y + i + 1, _height))).PointMagnitude()));
                    AddMoisture(tiles[x1, MathMagi.Mod(y - (i + 1), _height)],
                        (int)(0.025f / (center - new Point(x1,
                        MathMagi.Mod(y - (i + 1), _height))).PointMagnitude()));

                    AddMoisture(tiles[x2, MathMagi.Mod(y + i + 1, _height)],
                        (int)(0.025f / (center - new Point(x2,
                        MathMagi.Mod(y + i + 1, _height))).PointMagnitude()));
                    AddMoisture(tiles[x2, MathMagi.Mod(y - (i + 1), _height)],
                        (int)(0.025f / (center - new Point(x2,
                        MathMagi.Mod(y - (i + 1), _height))).PointMagnitude()));
                }
                curr--;
            }
        }

        // Dig river based on a parent river vein
        /*private static void DigRiver(River river, River parent)
        {
            int intersectionID = 0;
            int intersectionSize = 0;

            // determine point of intersection
            for (int i = 0; i < river.Tiles.Count; i++)
            {
                WorldTile t1 = river.Tiles[i];
                for (int j = 0; j < parent.Tiles.Count; j++)
                {
                    WorldTile t2 = parent.Tiles[j];
                    if (t1 == t2)
                    {
                        intersectionID = i;
                        intersectionSize = t2.RiverSize;
                    }
                }
            }

            int counter = 0;
            int intersectionCount = river.Tiles.Count - intersectionID;
            int size = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(intersectionSize, 5);
            river.Length = river.Tiles.Count;

            // randomize size change
            int two = river.Length / 2;
            int three = two / 2;
            int four = three / 2;
            int five = four / 2;

            int twomin = two / 3;
            int threemin = three / 3;
            int fourmin = four / 3;
            int fivemin = five / 3;

            // randomize length of each size
            int count1 = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(fivemin, five);
            if (size < 4)
            {
                count1 = 0;
            }
            int count2 = count1 + GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(fourmin, four);
            if (size < 3)
            {
                count2 = 0;
                count1 = 0;
            }
            int count3 = count2 + GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(threemin, three);
            if (size < 2)
            {
                count3 = 0;
                count2 = 0;
                count1 = 0;
            }
            int count4 = count3 + GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(twomin, two);

            // Make sure we are not digging past the river path
            if (count4 > river.Length)
            {
                int extra = count4 - river.Length;
                while (extra > 0)
                {
                    if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; } else if (count2 > 0) { count2--; count3--; count4--; extra--; } else if (count3 > 0) { count3--; count4--; extra--; } else if (count4 > 0) { count4--; extra--; }
                }
            }

            // adjust size of river at intersection point
            if (intersectionSize == 1)
            {
                count4 = intersectionCount;
                count1 = 0;
                count2 = 0;
                count3 = 0;
            }
            else if (intersectionSize == 2)
            {
                count3 = intersectionCount;
                count1 = 0;
                count2 = 0;
            }
            else if (intersectionSize == 3)
            {
                count2 = intersectionCount;
                count1 = 0;
            }
            else if (intersectionSize == 4)
            {
                count1 = intersectionCount;
            }
            else
            {
                count1 = 0;
                count2 = 0;
                count3 = 0;
                count4 = 0;
            }

            // dig out the river
            for (int i = river.Tiles.Count - 1; i >= 0; i--)
            {
                WorldTile t = river.Tiles[i];

                if (counter < count1)
                {
                    t.DigRiver(river, 4);
                }
                else if (counter < count2)
                {
                    t.DigRiver(river, 3);
                }
                else if (counter < count3)
                {
                    t.DigRiver(river, 2);
                }
                else if (counter < count4)
                {
                    t.DigRiver(river, 1);
                }
                else
                {
                    t.DigRiver(river, 0);
                }
                counter++;
            }
        }*/

        public BiomeType GetBiomeType(WorldTile tile)
        {
            return biomeTable[(int)tile.MoistureType, (int)tile.HeatType];
        }

        private void GenerateBiomeMap()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    if (!tiles[x, y].Collidable) continue;

                    WorldTile t = tiles[x, y];
                    t.BiomeType = GetBiomeType(t);
                }
            }
        }

        private void UpdateBiomeBitmask()
        {
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    tiles?[x, y].UpdateBiomeBitmask();
                }
            }
        }

        #endregion HelpMethods
    }
}
