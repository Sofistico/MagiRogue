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
        private static readonly Color RiverColor = new Color(30 / 255f, 120 / 255f, 200 / 255f, 1);
        private static readonly Color SandColor = new(240 / 255f, 240 / 255f, 64 / 255f, 1);
        private static readonly Color GrassColor = new(50 / 255f, 220 / 255f, 20 / 255f, 1);
        private static readonly Color ForestColor = new(16 / 255f, 160 / 255f, 0, 1);
        private static readonly Color RockColor = new(0.5f, 0.5f, 0.5f, 1);
        private static readonly Color SnowColor = Color.White;
        private static readonly Color MagicColor = Color.Purple;

        // Heat Map Colors
        private static readonly Color Coldest = new Color(0, 1, 1, 1);
        private static readonly Color Colder = new Color(170 / 255f, 1, 1, 1);
        private static readonly Color Cold = new Color(0, 229 / 255f, 133 / 255f, 1);
        private static readonly Color Warm = new Color(1, 1, 100 / 255f, 1);
        private static readonly Color Warmer = new Color(1, 100 / 255f, 0, 1);
        private static readonly Color Warmest = new Color(241 / 255f, 12 / 255f, 0, 1);

        //Moisture map
        private static readonly Color Dryest = new Color(255 / 255f, 139 / 255f, 17 / 255f, 1);
        private static readonly Color Dryer = new Color(245 / 255f, 245 / 255f, 23 / 255f, 1);
        private static readonly Color Dry = new Color(80 / 255f, 255 / 255f, 0 / 255f, 1);
        private static readonly Color Wet = new Color(85 / 255f, 255 / 255f, 255 / 255f, 1);
        private static readonly Color Wetter = new Color(20 / 255f, 70 / 255f, 255 / 255f, 1);
        private static readonly Color Wettest = new Color(0 / 255f, 0 / 255f, 100 / 255f, 1);

        private static readonly Color IceWater = new Color(210 / 255f, 255 / 255f, 252 / 255f, 1);
        private static readonly Color ColdWater = new Color(119 / 255f, 156 / 255f, 213 / 255f, 1);
        private static readonly Color RiverWater = new Color(65 / 255f, 110 / 255f, 179 / 255f, 1);

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

                        case HeightType.River:
                            tempTile = new(RiverColor, Color.Black, '~');
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

        public static void GetMoistureMap(int width, int height, WorldTile[,] tiles)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WorldTile tile = tiles[x, y];

                    switch (tile.MoistureType)
                    {
                        case MoistureType.Wettest:
                            tile.Foreground = Wettest;
                            break;

                        case MoistureType.Wetter:
                            tile.Foreground = Wetter;
                            break;

                        case MoistureType.Wet:
                            tile.Foreground = Wet;
                            break;

                        case MoistureType.Dry:
                            tile.Foreground = Dry;
                            break;

                        case MoistureType.Dryer:
                            tile.Foreground = Dryer;
                            break;

                        case MoistureType.Dryest:
                            tile.Foreground = Dryest;
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}