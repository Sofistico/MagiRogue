using Arquimedes;
using Arquimedes.Enumerators;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Utils;
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
                            tempTile = new(MagiPalette.DeepWaterColor, Color.Black, '~');
                            tiles[x, y].ParentTile.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].ParentTile.IsWalkable = false;
                            break;

                        case HeightType.ShallowWater:
                            tempTile = new(MagiPalette.ShallowWaterColor, Color.Black, '~');
                            tiles[x, y].ParentTile.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].ParentTile.IsWalkable = false;

                            break;

                        case HeightType.Sand:
                            tempTile = new(MagiPalette.SandColor, Color.Black, '~');
                            tiles[x, y].ParentTile.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].ParentTile.IsWalkable = true;

                            break;

                        case HeightType.Grass:
                            tempTile = new(MagiPalette.GrassColor, Color.Black, 39);
                            tiles[x, y].ParentTile.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].ParentTile.IsWalkable = true;

                            break;

                        case HeightType.Forest:
                            tempTile = new(MagiPalette.ForestColor, Color.Black, 'T');
                            tiles[x, y].ParentTile.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].ParentTile.IsWalkable = true;

                            break;

                        case HeightType.Mountain:
                            tempTile = new(MagiPalette.RockColor, Color.Black, 127);
                            tiles[x, y].ParentTile.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].ParentTile.MoveTimeCost = 500;
                            tiles[x, y].ParentTile.IsWalkable = true;

                            break;

                        case HeightType.Snow:
                            tempTile = new(MagiPalette.SnowColor, Color.Black, '~');
                            tiles[x, y].ParentTile.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].ParentTile.IsWalkable = true;

                            break;

                        case HeightType.River:
                            tempTile = new(MagiPalette.RiverColor, Color.Black, '~');
                            tiles[x, y].ParentTile.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].ParentTile.IsWalkable = true;

                            break;

                        case HeightType.HighMountain:
                            tempTile = new(MagiPalette.HighMountainColor, Color.Black, 30);
                            tiles[x, y].ParentTile.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].ParentTile.IsWalkable = true;
                            break;
                    }

                    if (tiles[x, y].Bitmask != 15)
                    {
                        tiles[x, y].ParentTile.Appearence.Foreground = Color.Lerp(tiles[x, y].ParentTile.Appearence.Foreground, Color.Black, 0.2f);
                    }
                    tiles[x, y].ParentTile.Position = new Point(x, y);
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
                            tiles[x, y].ParentTile.Appearence.Foreground = MagiPalette.Coldest;

                            break;

                        case HeatType.Colder:
                            tiles[x, y].ParentTile.Appearence.Foreground = MagiPalette.Colder;

                            break;

                        case HeatType.Cold:
                            tiles[x, y].ParentTile.Appearence.Foreground = MagiPalette.Cold;
                            break;

                        case HeatType.Warm:
                            tiles[x, y].ParentTile.Appearence.Foreground = MagiPalette.Warm;
                            break;

                        case HeatType.Warmer:
                            tiles[x, y].ParentTile.Appearence.Foreground = MagiPalette.Warmer;
                            break;

                        case HeatType.Warmest:
                            tiles[x, y].ParentTile.Appearence.Foreground = MagiPalette.Warmest;
                            break;
                    }

                    // Darkens color if is a edge tile
                    if (tiles[x, y].Bitmask != 15)
                    {
                        tiles[x, y].ParentTile.Appearence.Foreground = Color.Lerp(tiles[x, y].ParentTile.Appearence.Foreground, Color.Black, 0.4f);
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
                            tile.ParentTile.Appearence.Foreground = MagiPalette.Wettest;
                            break;

                        case MoistureType.Wetter:
                            tile.ParentTile.Appearence.Foreground = MagiPalette.Wetter;
                            break;

                        case MoistureType.Wet:
                            tile.ParentTile.Appearence.Foreground = MagiPalette.Wet;
                            break;

                        case MoistureType.Dry:
                            tile.ParentTile.Appearence.Foreground = MagiPalette.Dry;
                            break;

                        case MoistureType.Dryer:
                            tile.ParentTile.Appearence.Foreground = MagiPalette.Dryer;
                            break;

                        case MoistureType.Dryest:
                            tile.ParentTile.Appearence.Foreground = MagiPalette.Dryest;
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
                                tile.ParentTile.Appearence.Foreground = MagiPalette.Ice;
                                tile.Collidable = true;
                                tile.ParentTile.IsWalkable = false;
                                tile.ParentTile.Appearence.Glyph = 176;
                                break;

                            case BiomeType.BorealForest:
                                tile.ParentTile.Appearence.Foreground = MagiPalette.BorealForest;
                                tile.ParentTile.MoveTimeCost = 275;
                                tile.Collidable = true;
                                tile.ParentTile.IsWalkable = false;
                                tile.ParentTile.Appearence.Glyph = 20;

                                break;

                            case BiomeType.Desert:
                                tile.ParentTile.Appearence.Foreground = MagiPalette.Desert;
                                tile.ParentTile.MoveTimeCost = 350;
                                tile.Collidable = true;
                                tile.ParentTile.IsWalkable = false;
                                tile.ParentTile.Appearence.Glyph = 247;

                                break;

                            case BiomeType.Grassland:
                                tile.ParentTile.Appearence.Foreground = MagiPalette.Grassland;
                                tile.Collidable = true;
                                tile.ParentTile.IsWalkable = false;
                                tile.ParentTile.Appearence.Glyph = 'n';

                                break;

                            case BiomeType.SeasonalForest:
                                tile.ParentTile.Appearence.Foreground = MagiPalette.SeasonalForest;
                                tile.ParentTile.MoveTimeCost = 275;
                                tile.Collidable = true;
                                tile.ParentTile.IsWalkable = false;
                                tile.ParentTile.Appearence.Glyph = 6;
                                break;

                            case BiomeType.Tundra:
                                tile.ParentTile.Appearence.Foreground = MagiPalette.Tundra;
                                tile.ParentTile.MoveTimeCost = 300;
                                tile.Collidable = true;
                                tile.ParentTile.IsWalkable = false;
                                tile.ParentTile.Appearence.Glyph = 226;

                                break;

                            case BiomeType.Savanna:
                                tile.ParentTile.Appearence.Foreground = MagiPalette.Savanna;
                                tile.ParentTile.MoveTimeCost = 300;
                                tile.Collidable = true;
                                tile.ParentTile.IsWalkable = false;
                                tile.ParentTile.Appearence.Glyph = 5;

                                break;

                            case BiomeType.TemperateRainforest:
                                tile.ParentTile.Appearence.Foreground = MagiPalette.TemperateRainforest;
                                tile.ParentTile.MoveTimeCost = 310;
                                tile.ParentTile.IsWalkable = false;
                                tile.Collidable = true;
                                tile.ParentTile.Appearence.Glyph = 6;

                                break;

                            case BiomeType.TropicalRainforest:
                                tile.ParentTile.Appearence.Foreground = MagiPalette.TropicalRainforest;
                                tile.ParentTile.MoveTimeCost = 315;
                                tile.ParentTile.IsWalkable = false;
                                tile.Collidable = true;
                                tile.ParentTile.Appearence.Glyph = 6;

                                break;

                            case BiomeType.Woodland:
                                tile.ParentTile.Appearence.Foreground = MagiPalette.Woodland;
                                tile.ParentTile.MoveTimeCost = 250;
                                tile.ParentTile.IsWalkable = false;
                                tile.Collidable = true;
                                tile.ParentTile.Appearence.Glyph = 5;

                                break;
                        }
                    }
                    // Water tiles
                    if (tiles[x, y].HeightType == HeightType.DeepWater)
                    {
                        tile.ParentTile.Appearence.Foreground = MagiPalette.DeepWaterColor;
                        tile.ParentTile.IsWalkable = true;
                        tile.BiomeType = BiomeType.Sea;
                    }
                    else if (tiles[x, y].HeightType == HeightType.ShallowWater)
                    {
                        tile.ParentTile.Appearence.Foreground = MagiPalette.ShallowWaterColor;
                        tile.ParentTile.IsWalkable = true;
                        tile.BiomeType = BiomeType.Sea;
                    }

                    // draw rivers
                    if (tiles[x, y].HeightType == HeightType.River)
                    {
                        float heatValue = tiles[x, y].HeatValue;

                        if (tiles[x, y].HeatType == HeatType.Coldest)
                            tile.ParentTile.Appearence.Foreground = Color.Lerp(MagiPalette.IceWater, MagiPalette.ColdWater, heatValue / MagiPalette.Coldest.B);
                        else if (tiles[x, y].HeatType == HeatType.Colder)
                            tile.ParentTile.Appearence.Foreground = Color.Lerp(MagiPalette.ColdWater, MagiPalette.RiverWater, (heatValue - MagiPalette.Coldest.B) / (MagiPalette.Colder.B - MagiPalette.Coldest.G));
                        else if (tiles[x, y].HeatType == HeatType.Cold)
                            tile.ParentTile.Appearence.Foreground = Color.Lerp(MagiPalette.RiverWater, MagiPalette.ShallowWaterColor, (heatValue - MagiPalette.Colder.G) / (MagiPalette.Cold.R - MagiPalette.Colder.B));
                        else
                            tile.ParentTile.Appearence.Foreground = MagiPalette.ShallowWaterColor;
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
                            tile.ParentTile.Appearence.Foreground = Color.Lerp(tile.ParentTile.Appearence.Foreground, Color.Black, 0.35f);
                    }
                    tile.ParentTile.Name = tile.BiomeType.ToString();

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

                    if (!tile.ParentTile.GetComponent<SiteTile>(out var site))
                        continue;

                    if (site.SiteInfluence.SiteType is SiteType.City)
                    {
                        tile.ParentTile.Appearence.Glyph = '#';
                        tile.ParentTile.Appearence.Foreground = Color.White;
                    }

                    if (site.SiteInfluence.SiteType is SiteType.Camp)
                    {
                        tile.ParentTile.Appearence.Glyph = GlyphHelper.GetGlyph('☼');
                        tile.ParentTile.Appearence.Foreground = Color.AnsiYellow;
                    }
                    if (site.SiteInfluence.SiteType is SiteType.Tower)
                    {
                        tile.ParentTile.Appearence.Glyph = GlyphHelper.GetGlyph('T');
                        tile.ParentTile.Appearence.Foreground = Color.MediumPurple;
                    }
                    if (site.SiteInfluence.SiteType is SiteType.Dungeon)
                    {
                        tile.ParentTile.Appearence.Glyph = GlyphHelper.GetGlyph('D');
                        tile.ParentTile.Appearence.Foreground = Color.MediumPurple;
                    }

                    // think of a better way later
                    /*if (tile.Road != null &&
                        tile.HeightType != HeightType.DeepWater &&
                        tile.HeightType != HeightType.ShallowWater)
                    {
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.Down
                            || tile.Road.RoadDirectionInPos[tile.Position] == Direction.Top)
                        {
                            tile.ParentTile.Appearence.Glyph = 179;
                            tile.ParentTile.Appearence.Foreground = MagiPalette.DirtRoad;
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.Right
                            || tile.Road.RoadDirectionInPos[tile.Position] == Direction.Left)
                        {
                            tile.ParentTile.Appearence.Glyph = 196;
                            tile.ParentTile.Appearence.Foreground = MagiPalette.DirtRoad;
                        }
                        // Need to make it better!
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.TopLeft)
                        {
                            tile.ParentTile.Appearence.Foreground = MagiPalette.DirtRoad;
                            tile.ParentTile.Appearence.Glyph = '\\';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.DownRight)
                        {
                            tile.ParentTile.Appearence.Foreground = MagiPalette.DirtRoad;
                            tile.ParentTile.Appearence.Glyph = '\\';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.TopRight)
                        {
                            tile.ParentTile.Appearence.Foreground = MagiPalette.DirtRoad;
                            tile.ParentTile.Appearence.Glyph = '/';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.DownLeft)
                        {
                            tile.ParentTile.Appearence.Foreground = MagiPalette.DirtRoad;
                            tile.ParentTile.Appearence.Glyph = '/';
                        }

                        tile.ParentTile.MoveTimeCost = 100;
                    }*/
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
                                tile.ParentTile.Appearence.Foreground = MagiPalette.MagicColor;
#if DEBUG
                                tile.ParentTile.Appearence.Background = MagiPalette.MagicColor;
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