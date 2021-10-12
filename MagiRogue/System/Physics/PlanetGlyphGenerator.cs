using MagiRogue.Data;
using MagiRogue.System.Tiles;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Physics
{
    public static class PlanetGlyphGenerator
    {
        // Height Map Colors
        private static readonly Color DeepColor = new(0, 0, 0.5f, 1);
        private static readonly Color ShallowColor = new(25 / 255f, 25 / 255f, 150 / 255f, 1);
        private static readonly Color SandColor = new(240 / 255f, 240 / 255f, 64 / 255f, 1);
        private static readonly Color GrassColor = new(50 / 255f, 220 / 255f, 20 / 255f, 1);
        private static readonly Color ForestColor = new(16 / 255f, 160 / 255f, 0, 1);
        private static readonly Color RockColor = new(0.5f, 0.5f, 0.5f, 1);
        private static readonly Color SnowColor = new(1, 1, 1, 1);

        public static void SetTile(int width, int height, ref WorldTile[,] tiles)
        {
            WorldTile worldTile;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (tiles[x, y].HeightType)
                    {
                        case HeightType.DeepWater:
                            worldTile = new(DeepColor, Color.Black, '~',
                                new Point(x, y));
                            tiles[x, y] = worldTile;
                            break;

                        case HeightType.ShallowWater:
                            worldTile = new(ShallowColor, Color.Black, '~',
                                new Point(x, y));
                            tiles[x, y] = worldTile;

                            break;

                        case HeightType.Sand:
                            worldTile = new(SandColor, Color.Black, '~',
                                new Point(x, y));
                            tiles[x, y] = worldTile;

                            break;

                        case HeightType.Grass:
                            worldTile = new(GrassColor, Color.Black, ',',
                            new Point(x, y));
                            tiles[x, y] = worldTile;

                            break;

                        case HeightType.Forest:
                            worldTile = new(ForestColor, Color.Black, 'T',
                                new Point(x, y));
                            tiles[x, y] = worldTile;

                            break;

                        case HeightType.Rock:
                            worldTile = new(ForestColor, Color.Black, '^',
                                                            new Point(x, y));
                            tiles[x, y] = worldTile;

                            break;

                        case HeightType.Snow:
                            worldTile = new(SnowColor, Color.Black, '~',
                                new Point(x, y));
                            tiles[x, y] = worldTile;

                            break;
                    }
                }
            }
        }
    }
}