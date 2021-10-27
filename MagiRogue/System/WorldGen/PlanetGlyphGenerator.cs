using MagiRogue.Data;
using MagiRogue.System.Tiles;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.WorldGen
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
        private static readonly Color SnowColor = Color.White;
        private static readonly Color MagicColor = Color.Purple;

        // Heat Map Colors
        private static Color Coldest = new Color(0, 1, 1, 1);
        private static Color Colder = new Color(170 / 255f, 1, 1, 1);
        private static Color Cold = new Color(0, 229 / 255f, 133 / 255f, 1);
        private static Color Warm = new Color(1, 1, 100 / 255f, 1);
        private static Color Warmer = new Color(1, 100 / 255f, 0, 1);
        private static Color Warmest = new Color(241 / 255f, 12 / 255f, 0, 1);

        public static void SetTile(int width, int height, ref WorldTile[,] tiles)
        {
            ColoredGlyph tempTile;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (tiles[x, y].HeightType)
                    {
                        case HeightType.DeepWater:
                            tempTile = new(DeepColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            break;

                        case HeightType.ShallowWater:
                            tempTile = new(ShallowColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);

                            break;

                        case HeightType.Sand:
                            tempTile = new(SandColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);

                            break;

                        case HeightType.Grass:
                            tempTile = new(GrassColor, Color.Black, ',');
                            tiles[x, y].CopyAppearanceFrom(tempTile);

                            break;

                        case HeightType.Forest:
                            tempTile = new(ForestColor, Color.Black, 'T');
                            tiles[x, y].CopyAppearanceFrom(tempTile);

                            break;

                        case HeightType.Rock:
                            tempTile = new(RockColor, Color.Black, '^');
                            tiles[x, y].CopyAppearanceFrom(tempTile);

                            break;

                        case HeightType.Snow:
                            tempTile = new(SnowColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);

                            break;

                        case HeightType.MagicLand:
                            tempTile = new(MagicColor, Color.Black, 'i');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            break;
                    }

                    if (tiles[x, y].Bitmask != 15)
                    {
                        tiles[x, y].Foreground = Color.Lerp(tiles[x, y].Foreground, Color.Black, 0.2f);
                    }
                    tiles[x, y].Position = new Point(x, y);
                }
            }
        }

        public static void GetHeatMap(int width, int height, WorldTile[,] tiles)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    switch (tiles[x, y].HeatType)
                    {
                        case HeatType.Coldest:
                            tiles[x, y].Foreground = Coldest;

                            break;

                        case HeatType.Colder:
                            tiles[x, y].Foreground = Colder;

                            break;

                        case HeatType.Cold:
                            tiles[x, y].Foreground = Cold;
                            break;

                        case HeatType.Warm:
                            tiles[x, y].Foreground = Warm;
                            break;

                        case HeatType.Warmer:
                            tiles[x, y].Foreground = Warmer;
                            break;

                        case HeatType.Warmest:
                            tiles[x, y].Foreground = Warmest;
                            break;
                    }

                    // Darkens color if is a edge tile
                    if (tiles[x, y].Bitmask != 15)
                    {
                        tiles[x, y].Foreground = Color.Lerp(tiles[x, y].Foreground, Color.Black, 0.4f);
                    }
                }
            }
        }
    }
}