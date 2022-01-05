using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue
{
    public static class Palette
    {
        // Height Map Colors
        public static readonly Color DeepWaterColor = new(0, 0, 0.5f, 1);
        public static readonly Color ShallowWaterColor = new(25 / 255f, 25 / 255f, 150 / 255f, 1);
        public static readonly Color RiverColor = new Color(30 / 255f, 120 / 255f, 200 / 255f, 1);
        public static readonly Color SandColor = new(240 / 255f, 240 / 255f, 64 / 255f, 1);
        public static readonly Color GrassColor = new(50 / 255f, 220 / 255f, 20 / 255f, 1);
        public static readonly Color ForestColor = new(16 / 255f, 160 / 255f, 0, 1);
        public static readonly Color RockColor = new(0.5f, 0.5f, 0.5f, 1);
        public static readonly Color HighMountainColor = Color.AnsiWhiteBright;
        public static readonly Color SnowColor = Color.White;
        public static readonly Color MagicColor = Color.Purple;

        // Heat Map Colors
        public static readonly Color Coldest = new Color(0, 1, 1, 1);
        public static readonly Color Colder = new Color(170 / 255f, 1, 1, 1);
        public static readonly Color Cold = new Color(0, 229 / 255f, 133 / 255f, 1);
        public static readonly Color Warm = new Color(1, 1, 100 / 255f, 1);
        public static readonly Color Warmer = new Color(1, 100 / 255f, 0, 1);
        public static readonly Color Warmest = new Color(241 / 255f, 12 / 255f, 0, 1);

        //Moisture map
        public static readonly Color Dryest = new Color(255 / 255f, 139 / 255f, 17 / 255f, 1);
        public static readonly Color Dryer = new Color(245 / 255f, 245 / 255f, 23 / 255f, 1);
        public static readonly Color Dry = new Color(80 / 255f, 255 / 255f, 0 / 255f, 1);
        public static readonly Color Wet = new Color(85 / 255f, 255 / 255f, 255 / 255f, 1);
        public static readonly Color Wetter = new Color(20 / 255f, 70 / 255f, 255 / 255f, 1);
        public static readonly Color Wettest = new Color(0 / 255f, 0 / 255f, 100 / 255f, 1);

        public static readonly Color IceWater = new Color(210 / 255f, 255 / 255f, 252 / 255f, 1);
        public static readonly Color ColdWater = new Color(119 / 255f, 156 / 255f, 213 / 255f, 1);
        public static readonly Color RiverWater = new Color(65 / 255f, 110 / 255f, 179 / 255f, 1);

        //biome map
        public static readonly Color Ice = Color.White;
        public static readonly Color Desert = new Color(238 / 255f, 218 / 255f, 130 / 255f, 1);
        public static readonly Color Savanna = new Color(177 / 255f, 209 / 255f, 110 / 255f, 1);
        public static readonly Color TropicalRainforest = new Color(66 / 255f, 123 / 255f, 25 / 255f, 1);
        public static readonly Color Tundra = new Color(96 / 255f, 131 / 255f, 112 / 255f, 1);
        public static readonly Color TemperateRainforest = new Color(29 / 255f, 73 / 255f, 40 / 255f, 1);
        public static readonly Color Grassland = new Color(164 / 255f, 225 / 255f, 99 / 255f, 1);
        public static readonly Color SeasonalForest = new Color(73 / 255f, 100 / 255f, 35 / 255f, 1);
        public static readonly Color BorealForest = new Color(95 / 255f, 115 / 255f, 62 / 255f, 1);
        public static readonly Color Woodland = new Color(139 / 255f, 175 / 255f, 90 / 255f, 1);

        public static readonly Color DirtRoad = new Color(165, 103, 42);
        public static readonly Color Wood = new Color(186, 140, 99);
    }
}