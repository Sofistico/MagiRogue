using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Tiles;
using MagiRogue.Utils;
using SadConsole;
using SadRogue.Primitives;
using System;

namespace MagusEngine.Generators
{
    public static class PlanetGlyphGenerator
    {
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
                            tempTile = new(Palette.DeepWaterColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = true;
                            break;

                        case HeightType.ShallowWater:
                            tempTile = new(Palette.ShallowWaterColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = true;

                            break;

                        case HeightType.Sand:
                            tempTile = new(Palette.SandColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.Grass:
                            tempTile = new(Palette.GrassColor, Color.Black, 39);
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.Forest:
                            tempTile = new(Palette.ForestColor, Color.Black, 'T');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.Mountain:
                            tempTile = new(Palette.RockColor, Color.Black, 127);
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].MoveTimeCost = 500;
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.Snow:
                            tempTile = new(Palette.SnowColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.River:
                            tempTile = new(Palette.RiverColor, Color.Black, '~');
                            tiles[x, y].CopyAppearanceFrom(tempTile);
                            tiles[x, y].IsBlockingMove = false;

                            break;

                        case HeightType.HighMountain:
                            tempTile = new(Palette.HighMountainColor, Color.Black, 30);
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
                            tiles[x, y].Foreground = Palette.Coldest;

                            break;

                        case HeatType.Colder:
                            tiles[x, y].Foreground = Palette.Colder;

                            break;

                        case HeatType.Cold:
                            tiles[x, y].Foreground = Palette.Cold;
                            break;

                        case HeatType.Warm:
                            tiles[x, y].Foreground = Palette.Warm;
                            break;

                        case HeatType.Warmer:
                            tiles[x, y].Foreground = Palette.Warmer;
                            break;

                        case HeatType.Warmest:
                            tiles[x, y].Foreground = Palette.Warmest;
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
                            tile.Foreground = Palette.Wettest;
                            break;

                        case MoistureType.Wetter:
                            tile.Foreground = Palette.Wetter;
                            break;

                        case MoistureType.Wet:
                            tile.Foreground = Palette.Wet;
                            break;

                        case MoistureType.Dry:
                            tile.Foreground = Palette.Dry;
                            break;

                        case MoistureType.Dryer:
                            tile.Foreground = Palette.Dryer;
                            break;

                        case MoistureType.Dryest:
                            tile.Foreground = Palette.Dryest;
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
                                tile.Foreground = Palette.Ice;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 176;
                                break;

                            case BiomeType.BorealForest:
                                tile.Foreground = Palette.BorealForest;
                                tile.MoveTimeCost = 275;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 20;

                                break;

                            case BiomeType.Desert:
                                tile.Foreground = Palette.Desert;
                                tile.MoveTimeCost = 350;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 247;

                                break;

                            case BiomeType.Grassland:
                                tile.Foreground = Palette.Grassland;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 'n';

                                break;

                            case BiomeType.SeasonalForest:
                                tile.Foreground = Palette.SeasonalForest;
                                tile.MoveTimeCost = 275;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 6;
                                break;

                            case BiomeType.Tundra:
                                tile.Foreground = Palette.Tundra;
                                tile.MoveTimeCost = 300;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 226;

                                break;

                            case BiomeType.Savanna:
                                tile.Foreground = Palette.Savanna;
                                tile.MoveTimeCost = 300;
                                tile.Collidable = true;
                                tile.IsBlockingMove = false;
                                tile.Glyph = 5;

                                break;

                            case BiomeType.TemperateRainforest:
                                tile.Foreground = Palette.TemperateRainforest;
                                tile.MoveTimeCost = 310;
                                tile.IsBlockingMove = false;
                                tile.Collidable = true;
                                tile.Glyph = 6;

                                break;

                            case BiomeType.TropicalRainforest:
                                tile.Foreground = Palette.TropicalRainforest;
                                tile.MoveTimeCost = 315;
                                tile.IsBlockingMove = false;
                                tile.Collidable = true;
                                tile.Glyph = 6;

                                break;

                            case BiomeType.Woodland:
                                tile.Foreground = Palette.Woodland;
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
                        tile.Foreground = Palette.DeepWaterColor;
                        tile.IsBlockingMove = true;
                        tile.BiomeType = BiomeType.Sea;
                    }
                    else if (tiles[x, y].HeightType == HeightType.ShallowWater)
                    {
                        tile.Foreground = Palette.ShallowWaterColor;
                        tile.IsBlockingMove = true;
                        tile.BiomeType = BiomeType.Sea;
                    }

                    // draw rivers
                    if (tiles[x, y].HeightType == HeightType.River)
                    {
                        float heatValue = tiles[x, y].HeatValue;

                        if (tiles[x, y].HeatType == HeatType.Coldest)
                            tile.Foreground = Color.Lerp(Palette.IceWater, Palette.ColdWater, heatValue / Palette.Coldest.B);
                        else if (tiles[x, y].HeatType == HeatType.Colder)
                            tile.Foreground = Color.Lerp(Palette.ColdWater, Palette.RiverWater, (heatValue - Palette.Coldest.B) / (Palette.Colder.B - Palette.Coldest.G));
                        else if (tiles[x, y].HeatType == HeatType.Cold)
                            tile.Foreground = Color.Lerp(Palette.RiverWater, Palette.ShallowWaterColor, (heatValue - Palette.Colder.G) / (Palette.Cold.R - Palette.Colder.B));
                        else
                            tile.Foreground = Palette.ShallowWaterColor;
                    }

                    if (tile.HeightType == HeightType.HighMountain || tile.HeightType == HeightType.Mountain)
                    {
                        tile.BiomeType = BiomeType.Mountain;
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

        public static void SetSiteTiles(int width, int height, WorldTile[,] tiles)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WorldTile tile = tiles[x, y];

                    if (tile.SiteInfluence is null)
                        continue;

                    if (tile.SiteInfluence.SiteType is SiteType.City)
                    {
                        tile.Glyph = '#';
                        tile.Foreground = Color.White;
                    }

                    if (tile.SiteInfluence.SiteType is SiteType.Camp)
                    {
                        tile.Glyph = GlyphHelper.GetGlyph('☼');
                        tile.Foreground = Color.AnsiYellow;
                    }
                    if (tile.SiteInfluence.SiteType is SiteType.Tower)
                    {
                        tile.Glyph = GlyphHelper.GetGlyph('T');
                        tile.Foreground = Color.MediumPurple;
                    }
                    if (tile.SiteInfluence.SiteType is SiteType.Dungeon)
                    {
                        tile.Glyph = GlyphHelper.GetGlyph('D');
                        tile.Foreground = Color.MediumPurple;
                    }

                    if (tile.Road != null &&
                        tile.HeightType != HeightType.DeepWater &&
                        tile.HeightType != HeightType.ShallowWater)
                    {
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.Bottom
                            || tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.Top)
                        {
                            tile.Glyph = 179;
                            tile.Foreground = Palette.DirtRoad;
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.Right
                            || tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.Left)
                        {
                            tile.Glyph = 196;
                            tile.Foreground = Palette.DirtRoad;
                        }
                        // Need to make it better!
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.TopLeft)
                        {
                            tile.Foreground = Palette.DirtRoad;
                            tile.Glyph = '\\';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.BottomRight)
                        {
                            tile.Foreground = Palette.DirtRoad;
                            tile.Glyph = '\\';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.TopRight)
                        {
                            tile.Foreground = Palette.DirtRoad;
                            tile.Glyph = '/';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == WorldDirection.BottomLeft)
                        {
                            tile.Foreground = Palette.DirtRoad;
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
                                tile.Foreground = Palette.MagicColor;
#if DEBUG
                                tile.Background = Palette.MagicColor;
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