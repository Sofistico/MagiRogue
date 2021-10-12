using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.System.Tiles;
using MagiRogue.Utils;
using SadConsole;
using Console = SadConsole.Console;

namespace MagiRogue.System.Physics
{
    public class PlanetGenerator
    {
        private int width = 256;
        private int height = 256;
        private float deepWater = 0.2f;
        private float shallowWater = 0.4f;
        private float sand = 0.5f;
        private float grass = 0.7f;
        private float forest = 0.8f;
        private float rock = 0.9f;
        private float snow = 1;

        private int terrainOctaves = 6;
        private float terrainFrequency = 1.25f;

        // noise generator
        private FastNoiseLite heightMap;

        // height map data
        private PlanetMap planetData;

        // final object
        private WorldTile[,] tiles;

        private Console mapRenderer;

        public PlanetMap CreatePlanet()
        {
            // Initialize the generator
            Initialize();

            // Build the height map
            GetData(heightMap, ref planetData);

            // Build our final objects based on our data
            LoadTiles();

            // Here will take care of the visualization
            CreateConsole(mapRenderer, tiles);

            return planetData;
        }

        private void CreateConsole(Console mapRenderer, WorldTile[,] tiles)
        {
            PlanetGlyphGenerator.SetTile(width, height, ref tiles);
            WorldTile[] coloredTiles = new WorldTile[width * height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    coloredTiles[x + y * width] = tiles[x, y];
                }
            }

            mapRenderer = new Console(width, height, coloredTiles);

            GameLoop.UIManager.Children.Clear();
            GameLoop.UIManager.Children.Add(mapRenderer);
            mapRenderer.Position = new SadRogue.Primitives.Point(0, 0);
        }

        // Build a Tile array from our data
        private void LoadTiles()
        {
            tiles = new WorldTile[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WorldTile t = new WorldTile();
                    t.Position = new SadRogue.Primitives.Point(x, y);

                    float value = planetData.Data[x, y];

                    // normalize value between 0 and 1
                    value = MathMagi.ReturnPositive((float)Math.Round(value, 1));

                    t.HeightValue = value;

                    //HeightMap Analyze
                    if (value < deepWater)
                    {
                        t.HeightType = HeightType.DeepWater;
                    }
                    else if (value < shallowWater)
                    {
                        t.HeightType = HeightType.ShallowWater;
                    }
                    else if (value < sand)
                    {
                        t.HeightType = HeightType.Sand;
                    }
                    else if (value < grass)
                    {
                        t.HeightType = HeightType.Grass;
                    }
                    else if (value < forest)
                    {
                        t.HeightType = HeightType.Forest;
                    }
                    else if (value < rock)
                    {
                        t.HeightType = HeightType.Rock;
                    }
                    else
                    {
                        t.HeightType = HeightType.Snow;
                    }

                    tiles[x, y] = t;
                }
            }
        }

        // Extract data from a noise module
        private void GetData(FastNoiseLite heightMap, ref PlanetMap planetData)
        {
            planetData = new PlanetMap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float x1 = x / (float)width;
                    float y1 = y / (float)height;

                    float value = heightMap.GetNoise(x1, y1);

                    if (value > planetData.Max) planetData.Max = value;
                    if (value < planetData.Min) planetData.Min = value;

                    planetData.Data[x, y] = value;
                }
            }
        }

        private void Initialize()
        {
            heightMap = new FastNoiseLite(DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
            heightMap.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

            heightMap.SetFractalOctaves(terrainOctaves);
            heightMap.SetFrequency(terrainFrequency);
        }
    }
}