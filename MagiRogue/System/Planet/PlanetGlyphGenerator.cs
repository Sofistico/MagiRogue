using MagiRogue.Data;
using MagiRogue.System.Tiles;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Planet
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
        private static readonly Color HighMountainColor = Color.AnsiWhiteBright;
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

        //biome map
        private static readonly Color Ice = Color.White;
        private static readonly Color Desert = new Color(238 / 255f, 218 / 255f, 130 / 255f, 1);
        private static readonly Color Savanna = new Color(177 / 255f, 209 / 255f, 110 / 255f, 1);
        private static readonly Color TropicalRainforest = new Color(66 / 255f, 123 / 255f, 25 / 255f, 1);
        private static readonly Color Tundra = new Color(96 / 255f, 131 / 255f, 112 / 255f, 1);
        private static readonly Color TemperateRainforest = new Color(29 / 255f, 73 / 255f, 40 / 255f, 1);
        private static readonly Color Grassland = new Color(164 / 255f, 225 / 255f, 99 / 255f, 1);
        private static readonly Color SeasonalForest = new Color(73 / 255f, 100 / 255f, 35 / 255f, 1);
        private static readonly Color BorealForest = new Color(95 / 255f, 115 / 255f, 62 / 255f, 1);
        private static readonly Color Woodland = new Color(139 / 255f, 175 / 255f, 90 / 255f, 1);

        private static readonly Color DirtRoad = new Color(165, 103, 42);

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
                            tiles[x, y].IsBlockingMove = true;
                            break;

                        case HeightType.ShallowWater:
                            tempTile = new(ShallowColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = true;

                            break;

                        case HeightType.Sand:
                            tempTile = new(SandColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.Grass:
                            tempTile = new(GrassColor, Color.Black, 39);
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.Forest:
                            tempTile = new(ForestColor, Color.Black, 'T');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.Mountain:
                            tempTile = new(RockColor, Color.Black, 127);
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].MoveTimeCost = 500;
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.Snow:
                            tempTile = new(SnowColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.River:
                            tempTile = new(RiverColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.HighMountain:
                            tempTile = new(HighMountainColor, Color.Black, 30);
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;
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

        #region Debug Code

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

        #endregion Debug Code

        public static void GetBiomeMapTexture(int width, int height, WorldTile[,] tiles)
        {
            for (int x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    BiomeType value = tiles[x, y].BiomeType;
                    var tile = tiles[x, y];

                    if (tile.HeightType != HeightType.HighMountain && tile.HeightType != HeightType.Mountain)
                    {
                        switch (value)
                        {
                            case BiomeType.Ice:
                                tile.Foreground = Ice;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 176;
                                break;

                            case BiomeType.BorealForest:
                                tile.Foreground = BorealForest;
                                tile.MoveTimeCost = 275;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 20;

                                break;

                            case BiomeType.Desert:
                                tile.Foreground = Desert;
                                tile.MoveTimeCost = 350;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 247;

                                break;

                            case BiomeType.Grassland:
                                tile.Foreground = Grassland;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 'n';

                                break;

                            case BiomeType.SeasonalForest:
                                tile.Foreground = SeasonalForest;
                                tile.MoveTimeCost = 275;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 6;
                                break;

                            case BiomeType.Tundra:
                                tile.Foreground = Tundra;
                                tile.MoveTimeCost = 300;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 226;

                                break;

                            case BiomeType.Savanna:
                                tile.Foreground = Savanna;
                                tile.MoveTimeCost = 300;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 5;

                                break;

                            case BiomeType.TemperateRainforest:
                                tile.Foreground = TemperateRainforest;
                                tile.MoveTimeCost = 310;
                                tile.IsBlockingMove = false;
                                tile.Collidable = true;
                                tile.Glyph = 6;

                                break;

                            case BiomeType.TropicalRainforest:
                                tile.Foreground = TropicalRainforest;
                                tile.MoveTimeCost = 315;
                                tile.IsBlockingMove = false;
                                tile.Collidable = true;
                                tile.Glyph = 6;

                                break;

                            case BiomeType.Woodland:
                                tile.Foreground = Woodland;
                                tile.MoveTimeCost = 250;
                                tile.IsBlockingMove = false;
                                tile.Collidable = true;
                                tile.Glyph = 5;

                                break;
                        }
                    }
                    // Water tiles
                    if (tiles[x, y].HeightType == HeightType.DeepWater)
                    {
                        tile.Foreground = DeepColor;
                        tile.IsBlockingMove = true;
                        tile.BiomeType = BiomeType.Sea;
                    }
                    else if (tiles[x, y].HeightType == HeightType.ShallowWater)
                    {
                        tile.Foreground = ShallowColor;
                        tile.IsBlockingMove = true;
                        tile.BiomeType = BiomeType.Sea;
                    }

                    // draw rivers
                    if (tiles[x, y].HeightType == HeightType.River)
                    {
                        float heatValue = tiles[x, y].HeatValue;

                        if (tiles[x, y].HeatType == HeatType.Coldest)
                            tile.Foreground = Color.Lerp(IceWater, ColdWater, (heatValue) / (Coldest.B));
                        else if (tiles[x, y].HeatType == HeatType.Colder)
                            tile.Foreground = Color.Lerp(ColdWater, RiverWater, (heatValue - Coldest.B) / (Colder.B - Coldest.G));
                        else if (tiles[x, y].HeatType == HeatType.Cold)
                            tile.Foreground = Color.Lerp(RiverWater, ShallowColor, (heatValue - Colder.G) / (Cold.R - Colder.B));
                        else
                            tile.Foreground = ShallowColor;
                    }

                    // add a outline
                    if (tiles[x, y].HeightType >= HeightType.Shore
                        && tiles[x, y].HeightType != HeightType.River)
                    {
                        if (tiles[x, y].BiomeBitmask != 15)
                            tile.Foreground = Color.Lerp(tile.Foreground, Color.Black, 0.35f);
                    }
                    tile.Name = tile.BiomeType.ToString();

                    if (tile.HeightType == HeightType.ShallowWater && tile.BiomeType != BiomeType.Sea)
                        throw new Exception("Tried to add a biome to the ShallowWater other than Sea!");
                }
            }
        }

        public static void SetCityTiles(int width, int height, WorldTile[,] tiles)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WorldTile tile = tiles[x, y];

                    if (tile.CivInfluence is not null)
                    {
                        tile.Glyph = '#';
                        tile.Foreground = Color.White;
                    }
                    if (tile.Road != null &&
                        tile.HeightType != HeightType.DeepWater &&
                        tile.HeightType != HeightType.ShallowWater)
                    {
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.Bottom
                            || tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.Top)
                        {
                            tile.Glyph = 179;
                            tile.Foreground = DirtRoad;
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.Right
                            || tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.Left)
                        {
                            tile.Glyph = 196;
                            tile.Foreground = DirtRoad;
                        }
                        // Need to make it better!
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.TopLeft)
                        {
                            tile.Foreground = DirtRoad;
                            tile.Glyph = '\\';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.BottomRight)
                        {
                            tile.Foreground = DirtRoad;
                            tile.Glyph = '\\';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.TopRight)
                        {
                            tile.Foreground = DirtRoad;
                            tile.Glyph = '/';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.BottomLeft)
                        {
                            tile.Foreground = DirtRoad;
                            tile.Glyph = '/';
                        }

                        tile.MoveTimeCost = 100;
                    }
                }
            }
        }

        public static void SetSpecialTiles(int width, int height, WorldTile[,] tiles)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WorldTile tile = tiles[x, y];

                    if (tile.SpecialLandType != SpecialLandType.None)
                    {
                        switch (tile.SpecialLandType)
                        {
                            case SpecialLandType.None:
                                throw new Exception("An error occured! the game tried to add an special land type to a land that has nothing special.");

                            case SpecialLandType.MagicLand:
                                tile.Foreground = MagicColor;
#if DEBUG
                                tile.Background = MagicColor;
#endif

                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}