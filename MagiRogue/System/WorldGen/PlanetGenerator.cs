using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.System.Tiles;
using MagiRogue.Utils;
using MagiRogue.Utils.Noise;
using SadConsole;
using Console = SadConsole.Console;

namespace MagiRogue.System.WorldGen
{
    public class PlanetGenerator
    {
        private readonly int width = GameLoop.GameWidth;
        private readonly int height = GameLoop.GameHeight * 2;
        private readonly float deepWater = 0.2f;
        private readonly float shallowWater = 0.4f;
        private readonly float sand = 0.5f;
        private readonly float grass = 0.7f;
        private readonly float forest = 0.8f;
        private readonly float rock = 0.9f;
        private readonly float snow = 1;
        //private readonly float magicLand = 1.2f;

        private float coldestValue = 0.05f;
        private float colderValue = 0.18f;
        private float coldValue = 0.4f;
        private float warmValue = 0.6f;
        private float warmerValue = 0.8f;

        private readonly int terrainOctaves = 8;
        private readonly float terrainFrequency = 1.5f;
        private readonly float terrainLacunarity = 2f;
        private readonly float terrainGain = 0.5f;

        private readonly List<WorldTileGroup> waters = new();
        private readonly List<WorldTileGroup> lands = new();
        private int seed;

        // noise generator
        private FastNoiseLite heightMap;
        private ImplicitGradient heatMap;

        // Planet data
        private PlanetMap planetData;

        // final object
        private WorldTile[,] tiles;

        private Console mapRenderer;

        public PlanetMap CreatePlanet()
        {
            // Initialize the generator
            Initialize();

            // Build the height map
            GetData(ref planetData);

            // Build our final objects based on our data
            LoadTiles();

            UpdateNeighbors();
            UpdateBitmasks();
            FloodFill();

            // Here will take care of the visualization
            CreateConsole(tiles);

            return planetData;
        }

        private void CreateConsole(WorldTile[,] tiles)
        {
            PlanetGlyphGenerator.SetTile(width, height, ref tiles);
            PlanetGlyphGenerator.GetHeatMap(width, height, tiles);
            WorldTile[] coloredTiles = new WorldTile[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    coloredTiles[x + y * width] = tiles[x, y];
                }
            }
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
            tiles = new WorldTile[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WorldTile t = new();
                    t.Position = new SadRogue.Primitives.Point(x, y);

                    float value = planetData.HeightData[x, y];

                    // normalize value between 0 and 1
                    value = MathMagi.ReturnPositive(value);

                    t.HeightValue = value;

                    //HeightMap Analyze
                    if (value < deepWater)
                    {
                        t.HeightType = HeightType.DeepWater;
                        t.Collidable = false;
                    }
                    else if (value < shallowWater)
                    {
                        t.HeightType = HeightType.ShallowWater;
                        t.Collidable = false;
                    }
                    else if (value < sand)
                    {
                        t.HeightType = HeightType.Sand;
                        t.Collidable = true;
                    }
                    else if (value < grass)
                    {
                        t.HeightType = HeightType.Grass;
                        t.Collidable = true;
                    }
                    else if (value < forest)
                    {
                        t.HeightType = HeightType.Forest;
                        t.Collidable = true;
                    }
                    else if (value < rock)
                    {
                        t.HeightType = HeightType.Rock;
                        t.Collidable = true;
                    }
                    else if (value < snow)
                    {
                        t.HeightType = HeightType.Snow;
                        t.Collidable = true;
                    }
                    else
                    {
                        t.HeightType = HeightType.MagicLand;
                        t.Collidable = true;
                    }

                    float heatValue = planetData.HeatData[x, y];

                    if (heatValue < coldestValue)
                        t.HeatType = HeatType.Coldest;
                    else if (heatValue < colderValue)
                        t.HeatType = HeatType.Colder;
                    else if (heatValue < coldValue)
                        t.HeatType = HeatType.Cold;
                    else if (heatValue < warmValue)
                        t.HeatType = HeatType.Warm;
                    else if (heatValue < warmerValue)
                        t.HeatType = HeatType.Warmer;
                    else
                        t.HeatType = HeatType.Warmest;

                    tiles[x, y] = t;
                }
            }
        }

        // Extract data from a noise module
        private void GetData(ref PlanetMap planetData)
        {
            planetData = new PlanetMap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float x1 = x / (float)width;
                    float y1 = y / (float)height;

                    float heightValue = heightMap.GetNoise(x1, y1);
                    float heatValue = (float)heatMap.Get(x1, y1);

                    if (heightValue > planetData.Max) planetData.Max = heightValue;
                    if (heightValue < planetData.Min) planetData.Min = heightValue;
                    if (heatValue > planetData.Max) planetData.Max = heatValue;
                    if (heatValue < planetData.Min) planetData.Min = heatValue;

                    planetData.HeightData[x, y] = heightValue;
                    planetData.HeatData[x, y] = heatValue;
                }
            }
        }

        private void Initialize()
        {
            seed = GoRogue.Random.GlobalRandom.DefaultRNG.Next(0, int.MaxValue);
            heightMap = new FastNoiseLite(seed);
            heightMap.SetNoiseType(FastNoiseLite.NoiseType.Value);

            heightMap.SetFractalOctaves(terrainOctaves);
            heightMap.SetFrequency(terrainFrequency);
            heightMap.SetFractalLacunarity(terrainLacunarity);
            heightMap.SetFractalGain(terrainGain);
            heightMap.SetFractalType(FastNoiseLite.FractalType.PingPong);

            heatMap = new(1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1);
        }

        private WorldTile GetTop(WorldTile center)
        {
            return tiles[center.Position.X, MathMagi.Mod(center.Position.Y - 1, height)];
        }

        private WorldTile GetBottom(WorldTile t)
        {
            return tiles[t.Position.X, MathMagi.Mod(t.Position.Y + 1, height)];
        }

        private WorldTile GetLeft(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Position.X - 1, width), t.Position.Y];
        }

        private WorldTile GetRight(WorldTile t)
        {
            return tiles[MathMagi.Mod(t.Position.X + 1, width), t.Position.Y];
        }

        private void UpdateNeighbors()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
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
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].UpdateBitmask();
                }
            }
        }

        private void FloodFill()
        {
            Stack<WorldTile> stack = new();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
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
    }
}