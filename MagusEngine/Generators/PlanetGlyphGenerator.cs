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
                            tiles[x, y].Parent.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].Parent.IsWalkable = false;
                            break;

                        case HeightType.ShallowWater:
                            tempTile = new(MagiPalette.ShallowWaterColor, Color.Black, '~');
                            tiles[x, y].Parent.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].Parent.IsWalkable = false;

                            break;

                        case HeightType.Sand:
                            tempTile = new(MagiPalette.SandColor, Color.Black, '~');
                            tiles[x, y].Parent.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].Parent.IsWalkable = true;

                            break;

                        case HeightType.Grass:
                            tempTile = new(MagiPalette.GrassColor, Color.Black, 39);
                            tiles[x, y].Parent.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].Parent.IsWalkable = true;

                            break;

                        case HeightType.Forest:
                            tempTile = new(MagiPalette.ForestColor, Color.Black, 'T');
                            tiles[x, y].Parent.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].Parent.IsWalkable = true;

                            break;

                        case HeightType.Mountain:
                            tempTile = new(MagiPalette.RockColor, Color.Black, 127);
                            tiles[x, y].Parent.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].Parent.MoveTimeCost = 500;
                            tiles[x, y].Parent.IsWalkable = true;

                            break;

                        case HeightType.Snow:
                            tempTile = new(MagiPalette.SnowColor, Color.Black, '~');
                            tiles[x, y].Parent.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].Parent.IsWalkable = true;

                            break;

                        case HeightType.River:
                            tempTile = new(MagiPalette.RiverColor, Color.Black, '~');
                            tiles[x, y].Parent.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].Parent.IsWalkable = true;

                            break;

                        case HeightType.HighMountain:
                            tempTile = new(MagiPalette.HighMountainColor, Color.Black, 30);
                            tiles[x, y].Parent.Appearence.CopyAppearanceFrom(tempTile);
                            tiles[x, y].Parent.IsWalkable = true;
                            break;
                    }

                    if (tiles[x, y].Bitmask != 15)
                    {
                        tiles[x, y].Parent.Appearence.Foreground = Color.Lerp(tiles[x, y].Parent.Appearence.Foreground, Color.Black, 0.2f);
                    }
                    tiles[x, y].Parent.Position = new Point(x, y);
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
                            tiles[x, y].Parent.Appearence.Foreground = MagiPalette.Coldest;

                            break;

                        case HeatType.Colder:
                            tiles[x, y].Parent.Appearence.Foreground = MagiPalette.Colder;

                            break;

                        case HeatType.Cold:
                            tiles[x, y].Parent.Appearence.Foreground = MagiPalette.Cold;
                            break;

                        case HeatType.Warm:
                            tiles[x, y].Parent.Appearence.Foreground = MagiPalette.Warm;
                            break;

                        case HeatType.Warmer:
                            tiles[x, y].Parent.Appearence.Foreground = MagiPalette.Warmer;
                            break;

                        case HeatType.Warmest:
                            tiles[x, y].Parent.Appearence.Foreground = MagiPalette.Warmest;
                            break;
                    }

                    // Darkens color if is a edge tile
                    if (tiles[x, y].Bitmask != 15)
                    {
                        tiles[x, y].Parent.Appearence.Foreground = Color.Lerp(tiles[x, y].Parent.Appearence.Foreground, Color.Black, 0.4f);
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
                            tile.Parent.Appearence.Foreground = MagiPalette.Wettest;
                            break;

                        case MoistureType.Wetter:
                            tile.Parent.Appearence.Foreground = MagiPalette.Wetter;
                            break;

                        case MoistureType.Wet:
                            tile.Parent.Appearence.Foreground = MagiPalette.Wet;
                            break;

                        case MoistureType.Dry:
                            tile.Parent.Appearence.Foreground = MagiPalette.Dry;
                            break;

                        case MoistureType.Dryer:
                            tile.Parent.Appearence.Foreground = MagiPalette.Dryer;
                            break;

                        case MoistureType.Dryest:
                            tile.Parent.Appearence.Foreground = MagiPalette.Dryest;
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
                                tile.Parent.Appearence.Foreground = MagiPalette.Ice;
                                tile.Collidable = true;
                                tile.Parent.IsWalkable = false;
                                tile.Parent.Appearence.Glyph = 176;
                                break;

                            case BiomeType.BorealForest:
                                tile.Parent.Appearence.Foreground = MagiPalette.BorealForest;
                                tile.Parent.MoveTimeCost = 275;
                                tile.Collidable = true;
                                tile.Parent.IsWalkable = false;
                                tile.Parent.Appearence.Glyph = 20;

                                break;

                            case BiomeType.Desert:
                                tile.Parent.Appearence.Foreground = MagiPalette.Desert;
                                tile.Parent.MoveTimeCost = 350;
                                tile.Collidable = true;
                                tile.Parent.IsWalkable = false;
                                tile.Parent.Appearence.Glyph = 247;

                                break;

                            case BiomeType.Grassland:
                                tile.Parent.Appearence.Foreground = MagiPalette.Grassland;
                                tile.Collidable = true;
                                tile.Parent.IsWalkable = false;
                                tile.Parent.Appearence.Glyph = 'n';

                                break;

                            case BiomeType.SeasonalForest:
                                tile.Parent.Appearence.Foreground = MagiPalette.SeasonalForest;
                                tile.Parent.MoveTimeCost = 275;
                                tile.Collidable = true;
                                tile.Parent.IsWalkable = false;
                                tile.Parent.Appearence.Glyph = 6;
                                break;

                            case BiomeType.Tundra:
                                tile.Parent.Appearence.Foreground = MagiPalette.Tundra;
                                tile.Parent.MoveTimeCost = 300;
                                tile.Collidable = true;
                                tile.Parent.IsWalkable = false;
                                tile.Parent.Appearence.Glyph = 226;

                                break;

                            case BiomeType.Savanna:
                                tile.Parent.Appearence.Foreground = MagiPalette.Savanna;
                                tile.Parent.MoveTimeCost = 300;
                                tile.Collidable = true;
                                tile.Parent.IsWalkable = false;
                                tile.Parent.Appearence.Glyph = 5;

                                break;

                            case BiomeType.TemperateRainforest:
                                tile.Parent.Appearence.Foreground = MagiPalette.TemperateRainforest;
                                tile.Parent.MoveTimeCost = 310;
                                tile.Parent.IsWalkable = false;
                                tile.Collidable = true;
                                tile.Parent.Appearence.Glyph = 6;

                                break;

                            case BiomeType.TropicalRainforest:
                                tile.Parent.Appearence.Foreground = MagiPalette.TropicalRainforest;
                                tile.Parent.MoveTimeCost = 315;
                                tile.Parent.IsWalkable = false;
                                tile.Collidable = true;
                                tile.Parent.Appearence.Glyph = 6;

                                break;

                            case BiomeType.Woodland:
                                tile.Parent.Appearence.Foreground = MagiPalette.Woodland;
                                tile.Parent.MoveTimeCost = 250;
                                tile.Parent.IsWalkable = false;
                                tile.Collidable = true;
                                tile.Parent.Appearence.Glyph = 5;

                                break;
                        }
                    }
                    // Water tiles
                    if (tiles[x, y].HeightType == HeightType.DeepWater)
                    {
                        tile.Parent.Appearence.Foreground = MagiPalette.DeepWaterColor;
                        tile.Parent.IsWalkable = true;
                        tile.BiomeType = BiomeType.Sea;
                    }
                    else if (tiles[x, y].HeightType == HeightType.ShallowWater)
                    {
                        tile.Parent.Appearence.Foreground = MagiPalette.ShallowWaterColor;
                        tile.Parent.IsWalkable = true;
                        tile.BiomeType = BiomeType.Sea;
                    }

                    // draw rivers
                    if (tiles[x, y].HeightType == HeightType.River)
                    {
                        float heatValue = tiles[x, y].HeatValue;

                        if (tiles[x, y].HeatType == HeatType.Coldest)
                            tile.Parent.Appearence.Foreground = Color.Lerp(MagiPalette.IceWater, MagiPalette.ColdWater, heatValue / MagiPalette.Coldest.B);
                        else if (tiles[x, y].HeatType == HeatType.Colder)
                            tile.Parent.Appearence.Foreground = Color.Lerp(MagiPalette.ColdWater, MagiPalette.RiverWater, (heatValue - MagiPalette.Coldest.B) / (MagiPalette.Colder.B - MagiPalette.Coldest.G));
                        else if (tiles[x, y].HeatType == HeatType.Cold)
                            tile.Parent.Appearence.Foreground = Color.Lerp(MagiPalette.RiverWater, MagiPalette.ShallowWaterColor, (heatValue - MagiPalette.Colder.G) / (MagiPalette.Cold.R - MagiPalette.Colder.B));
                        else
                            tile.Parent.Appearence.Foreground = MagiPalette.ShallowWaterColor;
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
                            tile.Parent.Appearence.Foreground = Color.Lerp(tile.Parent.Appearence.Foreground, Color.Black, 0.35f);
                    }
                    tile.Parent.Name = tile.BiomeType.ToString();

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

                    if (!tile.Parent.GetComponent<SiteTile>(out var site))
                        continue;

                    if (site.SiteInfluence.SiteType is SiteType.City)
                    {
                        tile.Parent.Appearence.Glyph = '#';
                        tile.Parent.Appearence.Foreground = Color.White;
                    }

                    if (site.SiteInfluence.SiteType is SiteType.Camp)
                    {
                        tile.Parent.Appearence.Glyph = GlyphHelper.GetGlyph('☼');
                        tile.Parent.Appearence.Foreground = Color.AnsiYellow;
                    }
                    if (site.SiteInfluence.SiteType is SiteType.Tower)
                    {
                        tile.Parent.Appearence.Glyph = GlyphHelper.GetGlyph('T');
                        tile.Parent.Appearence.Foreground = Color.MediumPurple;
                    }
                    if (site.SiteInfluence.SiteType is SiteType.Dungeon)
                    {
                        tile.Parent.Appearence.Glyph = GlyphHelper.GetGlyph('D');
                        tile.Parent.Appearence.Foreground = Color.MediumPurple;
                    }

                    // think of a better way later
                    /*if (tile.Road != null &&
                        tile.HeightType != HeightType.DeepWater &&
                        tile.HeightType != HeightType.ShallowWater)
                    {
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.Down
                            || tile.Road.RoadDirectionInPos[tile.Position] == Direction.Top)
                        {
                            tile.Parent.Appearence.Glyph = 179;
                            tile.Parent.Appearence.Foreground = MagiPalette.DirtRoad;
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.Right
                            || tile.Road.RoadDirectionInPos[tile.Position] == Direction.Left)
                        {
                            tile.Parent.Appearence.Glyph = 196;
                            tile.Parent.Appearence.Foreground = MagiPalette.DirtRoad;
                        }
                        // Need to make it better!
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.TopLeft)
                        {
                            tile.Parent.Appearence.Foreground = MagiPalette.DirtRoad;
                            tile.Parent.Appearence.Glyph = '\\';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.DownRight)
                        {
                            tile.Parent.Appearence.Foreground = MagiPalette.DirtRoad;
                            tile.Parent.Appearence.Glyph = '\\';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.TopRight)
                        {
                            tile.Parent.Appearence.Foreground = MagiPalette.DirtRoad;
                            tile.Parent.Appearence.Glyph = '/';
                        }
                        if (tile.Road.RoadDirectionInPos[tile.Position] == Direction.DownLeft)
                        {
                            tile.Parent.Appearence.Foreground = MagiPalette.DirtRoad;
                            tile.Parent.Appearence.Glyph = '/';
                        }

                        tile.Parent.MoveTimeCost = 100;
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
                                tile.Parent.Appearence.Foreground = MagiPalette.MagicColor;
#if DEBUG
                                tile.Parent.Appearence.Background = MagiPalette.MagicColor;
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