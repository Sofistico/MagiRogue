using MagiRogue.System.Tiles;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using TinkerWorX.AccidentalNoiseLibrary;
using Console = SadConsole.Console;

namespace MagiRogue.System.WorldGen
{
    /// <summary>
    /// Generates a brand new planet
    /// </summary>
    public class PlanetGenerator
    {
        private int _width;
        private int _height;
        private Civilization[] _civilizations;
        private int maxCivsWorld;

        private readonly float deepWater = 0.2f;
        private readonly float shallowWater = 0.4f;
        private readonly float sand = 0.5f;
        private readonly float grass = 0.7f;
        private readonly float magicLand = 0.75f;
        private readonly float snow = 0.6f;
        private readonly float forest = 0.8f;
        private readonly float rock = 0.9f;

        private readonly float coldestValue = 0.05f;
        private readonly float colderValue = 0.18f;
        private readonly float coldValue = 0.4f;
        private readonly float warmValue = 0.6f;
        private readonly float warmerValue = 0.8f;

        private readonly int terrainOctaves = 8;
        private readonly float terrainFrequency = 1.5f;
        private readonly float heatFrequency = 3.0f;
        private readonly int heatOctaves = 4;

        private readonly int moistureOctaves = 4;
        private readonly float moistureFrequency = 3.0f;
        private readonly float dryerValue = 0.27f;
        private readonly float dryValue = 0.4f;
        private readonly float wetValue = 0.6f;
        private readonly float wetterValue = 0.8f;
        private readonly float wettestValue = 0.9f;

        // Rivers
        private readonly int riverCount = 40;
        private readonly int maxRiverAttempts = 1000;
        private readonly float minRiverHeight = 0.6f;
        private readonly int minRiverTurns = 18;
        private readonly int _minRiverLength = 20;
        private readonly int maxRiverIntersections = 2;
        private List<River> rivers;
        private List<RiverGroup> riverGroups;

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
        private int roadTries = 0;
        private int roadMaxTries = 500;
        private readonly Console mapRenderer;

        private readonly BiomeType[,] biomeTable = new BiomeType[6, 6] {
            //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
            { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
            };

        /// <summary>
        /// Creates a brand new planet!
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public PlanetMap CreatePlanet(int width, int height, int nmbCivilizations = 30)
        {
            _width = width;
            _height = height;
            maxCivsWorld = nmbCivilizations;
            _civilizations = new Civilization[maxCivsWorld];

            // Initialize the generator
            Initialize();

            // Build the height map
            GetData(ref planetData);

            // Build our final objects based on our data
            LoadTiles();

            UpdateNeighbors();

            GenerateRivers();
            BuildRiverGroups();
            DigRiverGroups();
            AdjustMoistureMap();

            UpdateBitmasks();
            FloodFill();

            GenerateBiomeMap();
            UpdateBiomeBitmask();

            SeedCivilizations();
            BasicHistory();

            // Here will take care of the visualization
            CreateConsole(tiles);

            return planetData;
        }

        private void BasicHistory()
        {
            for (int i = 0; i < _civilizations.Length; i++)
            {
                var civ = _civilizations[i];

                if (i < _civilizations.Length - 1)
                {
                    var nextCiv = _civilizations[i + 1];

                    if (civ.Tendency == nextCiv.Tendency)
                    {
                        BuildRoadsToFriends(civ, nextCiv);
                    }
                }
            }
        }

        private void BuildRoadsToFriends(Civilization civ, Civilization friend)
        {
            var tile = civ.Territory.OwnedLand.WorldTiles[0];
            var closestCityTile = friend.Territory.OwnedLand.WorldTiles[0];

            FindPathToCity(tile, closestCityTile);
        }
        private bool loading = false;

        private void FindPathToCity(WorldTile tile, WorldTile closestCityTile)
        {
            if (!tile.Collidable)
                return;

            if (roadTries >= roadMaxTries)
                return;

            if (tile.Road != null)
                return;

            // Found the city
            if (tile == closestCityTile)
                return;

            // Shouldn't appear, but who knows
            if (tile.HeightType == HeightType.River)
                return;
            Point cityPoint = closestCityTile.Position;
            Point roadPoint = tile.Position;
            var direction = Direction.GetCardinalDirection(roadPoint, cityPoint);

            if (direction == Direction.Up)
            {
                WorldTile worldTile = tile.Top;
                worldTile.Road = new();
                worldTile.Road.AddTileToList(worldTile);
                roadTries++;

                FindPathToCity(worldTile, closestCityTile);
            }
            if (direction == Direction.Down)
            {
                WorldTile worldTile = tile.Bottom;
                worldTile.Road = new();
                worldTile.Road.AddTileToList(worldTile);
                roadTries++;

                FindPathToCity(worldTile, closestCityTile);
            }
            if (direction != Direction.Left)
            {
                WorldTile worldTile = tile.Left;
                worldTile.Road = new();
                worldTile.Road.AddTileToList(worldTile);
                roadTries++;

                FindPathToCity(worldTile, closestCityTile);
            }
            if (direction != Direction.Right)
            {
                WorldTile worldTile = tile.Top;
                worldTile.Road = new();
                worldTile.Road.AddTileToList(worldTile);
                roadTries++;

                FindPathToCity(worldTile, closestCityTile);
            }
        }

        private void SeedCivilizations()
        {
            int tries = 0;
            int maxTries = 300;
            int currentCivCount = 0;

            while (currentCivCount < maxCivsWorld && tries < maxTries)
            {
                tries++;

                int x = GoRogue.Random.GlobalRandom.DefaultRNG.Next(0, _width);
                int y = GoRogue.Random.GlobalRandom.DefaultRNG.Next(0, _height);
                WorldTile tile = tiles[x, y];

                if (tile.HeightType == HeightType.DeepWater
                || tile.HeightType == HeightType.ShallowWater
                || tile.HeightType == HeightType.River)
                    continue;
                if (tile.CivInfluence != null)
                    continue;

                int rng = GoRogue.Random.GlobalRandom.DefaultRNG.Next(100, 5000);

                int pop = (int)(rng * ((int)tile.HeightType * tile.MoistureValue + 1));

                Civilization civ = new($"Random Name Here {currentCivCount}",
                    new Entities.Race("Human"), pop, RandomCivTendency());

                civ.MundaneResources = tile.MineralValue;
                tile.CivInfluence = civ;
                civ.Territory.AddLand(tile);

                if (currentCivCount < maxCivsWorld)
                {
                    _civilizations[currentCivCount] = civ;
                }

                currentCivCount++;
            }
        }

        private static CivilizationTendency RandomCivTendency()
        {
            int rng = GoRogue.Random.GlobalRandom.DefaultRNG.Next(0, 2 + 1);
            return rng switch
            {
                0 => CivilizationTendency.Normal,
                1 => CivilizationTendency.Aggresive,
                2 => CivilizationTendency.Studious,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        private void CreateConsole(WorldTile[,] tiles)
        {
            PlanetGlyphGenerator.SetTile(_width, _height, ref tiles);
            // Test only!
            //PlanetGlyphGenerator.GetHeatMap(width, height, tiles);
            //PlanetGlyphGenerator.GetMoistureMap(width, height, tiles);
            PlanetGlyphGenerator.GetBiomeMapTexture(_width, _height, tiles);
            PlanetGlyphGenerator.SetCityTiles(_width, _height, tiles);
            /*WorldTile[] coloredTiles = new WorldTile[_width * _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    coloredTiles[x + y * _width] = tiles[x, y];
                }
            }*/
            planetData.SetWorldTiles(tiles);
            /*try
            {
                mapRenderer = new Console(width, height, coloredTiles);
                GameLoop.UIManager.Children.Clear();
                GameLoop.UIManager.Children.Add(mapRenderer);
                mapRenderer.Position = new SadRogue.Primitives.Point(0, 0);
                mapRenderer.Font = SadConsole.Game.Instance.LoadFont("cp437_12x12.font");
            }
            catch (Exception)
            {
                Debug.Print("Console generation went wrong");
            }*/
        }

        // Build a Tile array from our data
        private void LoadTiles()
        {
            tiles = new WorldTile[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    WorldTile t = new();
                    t.Position = new SadRogue.Primitives.Point(x, y);

                    float heightValue = planetData.HeightData[x, y];
                    t.MineralValue = MathMagi.ReturnPositive(MathF.Round
                        ((planetData.HeightData[x, y] + planetData.HeatData[x, y]) * 200));

                    // normalize value between 0 and 1
                    heightValue = MathMagi.ReturnPositive(heightValue);

                    t.HeightValue = heightValue;

                    //HeightMap Analyze
                    if (heightValue < deepWater)
                    {
                        t.HeightType = HeightType.DeepWater;
                        t.Collidable = false;
                        t.MineralValue *= 1.5f;
                    }
                    else if (heightValue < shallowWater)
                    {
                        t.HeightType = HeightType.ShallowWater;
                        t.Collidable = false;
                        t.MineralValue *= 0.5f;
                    }
                    else if (heightValue < sand)
                    {
                        t.HeightType = HeightType.Sand;
                        t.Collidable = true;
                        t.MineralValue *= 0.7f;
                    }
                    else if (heightValue < grass)
                    {
                        t.HeightType = HeightType.Grass;
                        t.Collidable = true;
                        t.MineralValue *= 0.7f;
                    }
                    else if (heightValue < magicLand)
                    {
                        t.HeightType = HeightType.MagicLand;
                        t.Collidable = true;
                        t.MineralValue *= 1.3f;
                    }
                    else if (heightValue < forest)
                    {
                        t.HeightType = HeightType.Forest;
                        t.Collidable = true;
                        t.MineralValue *= 1.2f;
                    }
                    else if (heightValue < snow)
                    {
                        t.HeightType = HeightType.Snow;
                        t.Collidable = true;
                    }
                    else
                    {
                        t.HeightType = HeightType.Rock;
                        t.Collidable = true;
                        t.MineralValue *= 2.0f;
                    }

                    //Moisture Map Analyze
                    float moistureValue = MathMagi.ReturnPositive(planetData.MoistureData[x, y]);
                    t.MoistureValue = MathF.Round(moistureValue, 1);

                    //adjust moisture based on height
                    if (t.HeightType == HeightType.DeepWater)
                    {
                        planetData.MoistureData[t.Position.X, t.Position.Y] += 8f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.ShallowWater)
                    {
                        planetData.MoistureData[t.Position.X, t.Position.Y] += 3f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.Shore)
                    {
                        planetData.MoistureData[t.Position.X, t.Position.Y] += 1f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.Sand)
                    {
                        planetData.MoistureData[t.Position.X, t.Position.Y] += 0.25f * t.HeightValue;
                    }

                    //set moisture type
                    if (moistureValue < dryerValue) t.MoistureType = MoistureType.Dryest;
                    else if (moistureValue < dryValue) t.MoistureType = MoistureType.Dryer;
                    else if (moistureValue < wetValue) t.MoistureType = MoistureType.Dry;
                    else if (moistureValue < wetterValue) t.MoistureType = MoistureType.Wet;
                    else if (moistureValue < wettestValue) t.MoistureType = MoistureType.Wetter;
                    else t.MoistureType = MoistureType.Wettest;

                    if (t.HeightType == HeightType.Forest)
                    {
                        planetData.HeatData[t.Position.X, t.Position.Y] -= 0.1f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.Rock)
                    {
                        planetData.HeatData[t.Position.X, t.Position.Y] -= 0.25f * t.HeightValue;
                    }
                    else if (t.HeightType == HeightType.Snow)
                    {
                        planetData.HeatData[t.Position.X, t.Position.Y] -= 0.4f * t.HeightValue;
                    }
                    else
                    {
                        planetData.HeatData[t.Position.X, t.Position.Y] += 0.01f * t.HeightValue;
                    }

                    // Set heat value
                    float heatModValue = MathMagi.ReturnPositive(planetData.HeatData[t.Position.X, t.Position.Y]);
                    t.HeatValue = heatModValue;

                    // set heat type
                    if (heatModValue < coldestValue) t.HeatType = HeatType.Coldest;
                    else if (heatModValue < colderValue) t.HeatType = HeatType.Colder;
                    else if (heatModValue < coldValue) t.HeatType = HeatType.Cold;
                    else if (heatModValue < warmValue) t.HeatType = HeatType.Warm;
                    else if (heatModValue < warmerValue) t.HeatType = HeatType.Warmer;
                    else t.HeatType = HeatType.Warmest;

                    tiles[x, y] = t;
                }
            }
        }

        // Extract data from a noise module
        private void GetData(ref PlanetMap planetData)
        {
            planetData = new PlanetMap(_width, _height);

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    float x1 = x / (float)_width;
                    float y1 = y / (float)_height;

                    float heightValue = (float)heightMap.Get(x1, y1);
                    float heatValue = (float)heatMap.Get(x1, y1);
                    float moistureValue = (float)moistureMap.Get(x1, y);

                    if (heightValue > planetData.Max) planetData.Max = heightValue;
                    if (heightValue < planetData.Min) planetData.Min = heightValue;
                    if (heatValue > planetData.Max) planetData.Max = heatValue;
                    if (heatValue < planetData.Min) planetData.Min = heatValue;
                    if (moistureValue > planetData.Max) planetData.Max = moistureValue;
                    if (moistureValue < planetData.Min) planetData.Min = moistureValue;

                    planetData.HeightData[x, y] = heightValue;
                    planetData.HeatData[x, y] = heatValue;
                    planetData.MoistureData[x, y] = moistureValue;
                }
            }
        }

        private void Initialize()
        {
            seed = GoRogue.Random.GlobalRandom.DefaultRNG.Next(0, int.MaxValue);
            heightMap = new(FractalType.Multi,
                                       BasisType.Simplex,
                                       InterpolationType.Quintic,
                                       terrainOctaves,
                                       terrainFrequency,
                                       seed);

            var gradient = new ImplicitGradient(1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1);
            var heatFractal = new ImplicitFractal(
                FractalType.Multi,
                BasisType.Simplex,
                InterpolationType.Quintic,
                heatOctaves,
                heatFrequency,
                seed
                );

            heatMap = new(CombinerType.Multiply);
            heatMap.AddSource(gradient);
            heatMap.AddSource(heatFractal);

            moistureMap = new(FractalType.Multi, BasisType.Simplex, InterpolationType.Quintic,
                moistureOctaves, moistureFrequency, seed);

            riverGroups = new();
            rivers = new();
        }

        private WorldTile GetTop(WorldTile center)
        {
            return tiles[center.Position.X, MathMagi.Mod(center.Position.Y - 1, _height)];
        }

        private WorldTile GetBottom(WorldTile t)
        {
            return tiles[t.Position.X, MathMagi.Mod(t.Position.Y + 1, _height)];
        }

        private WorldTile GetLeft(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Position.X - 1, _width), t.Position.Y];
        }

        private WorldTile GetRight(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Position.X + 1, _width), t.Position.Y];
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
            int count = riverCount;

            // generate some rivers
            while (count > 0 && attempts < maxRiverAttempts)
            {
                // get random tiles
                int x = GoRogue.Random.GlobalRandom.DefaultRNG.Next(0, _width);
                int y = GoRogue.Random.GlobalRandom.DefaultRNG.Next(0, _height);
                WorldTile tile = tiles[x, y];

                // validate the tile
                if (!tile.Collidable) continue;
                if (tile.Rivers.Count > 0) continue;

                if (tile.HeightValue > minRiverHeight)
                {
                    // tile is good to start river from
                    River river = new(riverCount);

                    // Figure out the direction this river will try to flow
                    river.CurrentDirection = tile.GetLowestNeighbor();

                    // Recursively find a path to water
                    FindPathToWater(tile, river.CurrentDirection, ref river);

                    // Validate the generated river
                    if (river.TurnCount < minRiverTurns
                        || river.Tiles.Count < _minRiverLength
                        || river.Intersections > maxRiverIntersections)
                    {
                        //Validation failed - remove this river
                        for (int i = 0; i < river.Tiles.Count; i++)
                        {
                            WorldTile t = river.Tiles[i];
                            t.Rivers.Remove(river);
                        }
                    }
                    else if (river.Tiles.Count >= _minRiverLength)
                    {
                        //Validation passed - Add river to list
                        rivers.Add(river);
                        tile.Rivers.Add(river);
                        count--;
                    }
                }
                attempts++;
            }
        }

        private void FindPathToWater(WorldTile tile, WorldDirection currentDirection, ref River river)
        {
            if (tile.Rivers.Contains(river))
                return;

            if (tile.Rivers.Count > 0)
                river.Intersections++;

            river.AddTile(tile);

            WorldTile left = GetLeft(tile);
            WorldTile right = GetRight(tile);
            WorldTile top = GetTop(tile);
            WorldTile bottom = GetBottom(tile);

            float leftValue = int.MaxValue;
            float rigthValue = int.MaxValue;
            float topValue = int.MaxValue;
            float bottomValue = int.MaxValue;

            // query height values of neighbors
            if (left.GetRiverNeighborCount(river) < 2 && !river.Tiles.Contains(left))
                leftValue = left.HeightValue;
            if (right.GetRiverNeighborCount(river) <= 2 && !river.Tiles.Contains(right))
                rigthValue = right.HeightValue;
            if (top.GetRiverNeighborCount(river) <= 2 && !river.Tiles.Contains(top))
                topValue = top.HeightValue;
            if (bottom.GetRiverNeighborCount(river) <= 2 && !river.Tiles.Contains(bottom))
                bottomValue = bottom.HeightValue;

            // if neighbor is existing river that is not this one, flow into it
            if (bottom.Rivers.Count == 0 && !bottom.Collidable)
                bottomValue = 0;
            if (top.Rivers.Count == 0 && !top.Collidable)
                topValue = 0;
            if (right.Rivers.Count == 0 && !right.Collidable)
                rigthValue = 0;
            if (left.Rivers.Count == 0 && !left.Collidable)
                leftValue = 0;

            // override flow direction if a tile is significantly lower
            if (currentDirection == WorldDirection.Left)
                if (MathF.Abs(rigthValue - leftValue) < 0.1f)
                    rigthValue = int.MaxValue;
            if (currentDirection == WorldDirection.Right)
                if (MathF.Abs(rigthValue - leftValue) < 0.1f)
                    leftValue = int.MaxValue;
            if (currentDirection == WorldDirection.Top)
                if (MathF.Abs(topValue - bottomValue) < 0.1f)
                    topValue = int.MaxValue;
            if (currentDirection == WorldDirection.Bottom)
                if (MathF.Abs(topValue - bottomValue) < 0.1f)
                    bottomValue = int.MaxValue;

            // find mininum
            // has god forsaken us?
            float min = MathF.Min(MathF.Min(MathF.Min(leftValue, rigthValue), topValue), bottomValue);

            // if no minimum found - exit
            if (min == int.MaxValue)
                return;

            //Move to next neighbor
            if (min == leftValue)
            {
                if (left.Collidable)
                {
                    if (river.CurrentDirection != WorldDirection.Left)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = WorldDirection.Left;
                    }
                    FindPathToWater(left, currentDirection, ref river);
                }
            }
            if (min == rigthValue)
            {
                if (right.Collidable)
                {
                    if (river.CurrentDirection != WorldDirection.Right)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = WorldDirection.Right;
                    }
                    FindPathToWater(right, currentDirection, ref river);
                }
            }
            if (min == bottomValue)
            {
                if (bottom.Collidable)
                {
                    if (river.CurrentDirection != WorldDirection.Bottom)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = WorldDirection.Bottom;
                    }
                    FindPathToWater(bottom, currentDirection, ref river);
                }
            }
            if (min == topValue)
            {
                if (top.Collidable)
                {
                    if (river.CurrentDirection != WorldDirection.Top)
                    {
                        river.TurnCount++;
                        river.CurrentDirection = WorldDirection.Top;
                    }
                    FindPathToWater(top, currentDirection, ref river);
                }
            }
        }

        private void BuildRiverGroups()
        {
            //loop each tile, checking if it belongs to multiple rivers
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    WorldTile t = tiles[x, y];

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
        }

        private void DigRiverGroups()
        {
            for (int i = 0; i < riverGroups.Count; i++)
            {
                RiverGroup group = riverGroups[i];
                River? longest = null;

                //Find longest river in this group
                for (int j = 0; j < group.Rivers.Count; j++)
                {
                    River river = group.Rivers[j];
                    if (longest == null)
                        longest = river;
                    else if (longest.Tiles.Count < river.Tiles.Count)
                        longest = river;
                }

                if (longest != null)
                {
                    //Dig out longest path first
                    DigRiver(longest);

                    for (int j = 0; j < group.Rivers.Count; j++)
                    {
                        River river = group.Rivers[j];
                        if (river != longest)
                        {
                            DigRiver(river, longest);
                        }
                    }
                }
            }
        }

        private static void DigRiver(River river)
        {
            int counter = 0;

            // How wide are we digging this river?
            int size = GoRogue.Random.GlobalRandom.DefaultRNG.Next(1, 5);
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

            // randomize lenght of each size
            int count1 = GoRogue.Random.GlobalRandom.DefaultRNG.Next(fivemin, five);
            if (size < 4)
            {
                count1 = 0;
            }
            int count2 = count1 + GoRogue.Random.GlobalRandom.DefaultRNG.Next(fourmin, four);
            if (size < 3)
            {
                count2 = 0;
                count1 = 0;
            }
            int count3 = count2 + GoRogue.Random.GlobalRandom.DefaultRNG.Next(threemin, three);
            if (size < 2)
            {
                count3 = 0;
                count2 = 0;
                count1 = 0;
            }
            int count4 = count3 + GoRogue.Random.GlobalRandom.DefaultRNG.Next(twomin, two);

            // Make sure we are not digging past the river path
            if (count4 > river.Length)
            {
                int extra = count4 - river.Length;
                while (extra > 0)
                {
                    if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                    else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                    else if (count3 > 0) { count3--; count4--; extra--; }
                    else if (count4 > 0) { count4--; extra--; }
                }
            }

            // Dig it out
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
                        AddMoisture(t, (int)60);
                    }
                }
            }
        }

        private void AddMoisture(WorldTile t, int radius)
        {
            Point center = new(t.Position.X, t.Position.Y);
            int curr = radius;

            while (curr > 0)
            {
                int x1 = MathMagi.Mod(t.Position.X - curr, _width);
                int x2 = MathMagi.Mod(t.Position.X + curr, _width);
                int y = t.Position.Y;

                AddMoisture(tiles[x1, y],
                    (int)(0.025f / (center - new SadRogue.Primitives.Point(x1, y)).PointMagnitude()));

                for (int i = 0; i < curr; i++)
                {
                    AddMoisture(tiles[x1, MathMagi.Mod(y + i + 1, _height)],
                        (int)(0.025f / (center - new SadRogue.Primitives.Point(x1,
                        MathMagi.Mod(y + i + 1, _height))).PointMagnitude()));
                    AddMoisture(tiles[x1, MathMagi.Mod(y - (i + 1), _height)],
                        (int)(0.025f / (center - new SadRogue.Primitives.Point(x1,
                        MathMagi.Mod(y - (i + 1), _height))).PointMagnitude()));

                    AddMoisture(tiles[x2, MathMagi.Mod(y + i + 1, _height)],
                        (int)(0.025f / (center - new SadRogue.Primitives.Point(x2,
                        MathMagi.Mod(y + i + 1, _height))).PointMagnitude()));
                    AddMoisture(tiles[x2, MathMagi.Mod(y - (i + 1), _height)],
                        (int)(0.025f / (center - new SadRogue.Primitives.Point(x2,
                        MathMagi.Mod(y - (i + 1), _height))).PointMagnitude()));
                }
                curr--;
            }
        }

        // Dig river based on a parent river vein
        private static void DigRiver(River river, River parent)
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
            int size = GoRogue.Random.GlobalRandom.DefaultRNG.Next(intersectionSize, 5);
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
            int count1 = GoRogue.Random.GlobalRandom.DefaultRNG.Next(fivemin, five);
            if (size < 4)
            {
                count1 = 0;
            }
            int count2 = count1 + GoRogue.Random.GlobalRandom.DefaultRNG.Next(fourmin, four);
            if (size < 3)
            {
                count2 = 0;
                count1 = 0;
            }
            int count3 = count2 + GoRogue.Random.GlobalRandom.DefaultRNG.Next(threemin, three);
            if (size < 2)
            {
                count3 = 0;
                count2 = 0;
                count1 = 0;
            }
            int count4 = count3 + GoRogue.Random.GlobalRandom.DefaultRNG.Next(twomin, two);

            // Make sure we are not digging past the river path
            if (count4 > river.Length)
            {
                int extra = count4 - river.Length;
                while (extra > 0)
                {
                    if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                    else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                    else if (count3 > 0) { count3--; count4--; extra--; }
                    else if (count4 > 0) { count4--; extra--; }
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
        }

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
                    tiles[x, y].UpdateBiomeBitmask();
                }
            }
        }
    }
}