using SadRogue.Primitives;

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

        public static readonly Color Dirt = new Color(165, 103, 42);
        public static readonly Color DirtRoad = Color.Brown;
        public static readonly Color Wood = new Color(186, 140, 99);
        public static readonly Color DarkWood = new Color(87, 65, 46);
        public static readonly Color UnclearGlass = new Color(183, 183, 189);

        public static void AddToColorDictionary()
        {
            if (ColorExtensions2.ColorMappings.ContainsKey("DeepWaterColor".ToLower()))
                return;
            // height map color
            ColorExtensions2.ColorMappings.Add("DeepWaterColor".ToLower(), DeepWaterColor);
            ColorExtensions2.ColorMappings.Add("ShallowWaterColor".ToLower(), ShallowWaterColor);
            ColorExtensions2.ColorMappings.Add("RiverColor".ToLower(), RiverColor);
            ColorExtensions2.ColorMappings.Add("SandColor".ToLower(), SandColor);
            ColorExtensions2.ColorMappings.Add("GrassColor".ToLower(), GrassColor);
            ColorExtensions2.ColorMappings.Add("ForestColor".ToLower(), ForestColor);
            ColorExtensions2.ColorMappings.Add("RockColor".ToLower(), RockColor);
            ColorExtensions2.ColorMappings.Add("HighMountainColor".ToLower(), HighMountainColor);
            ColorExtensions2.ColorMappings.Add("SnowColor".ToLower(), SnowColor);
            ColorExtensions2.ColorMappings.Add("MagicColor".ToLower(), MagicColor);

            // temperature map color
            ColorExtensions2.ColorMappings.Add("Coldest".ToLower(), Coldest);
            ColorExtensions2.ColorMappings.Add("Colder".ToLower(), Colder);
            ColorExtensions2.ColorMappings.Add("Cold".ToLower(), Cold);
            ColorExtensions2.ColorMappings.Add("Warm".ToLower(), Warm);
            ColorExtensions2.ColorMappings.Add("Warmer".ToLower(), Warmer);
            ColorExtensions2.ColorMappings.Add("Warmest".ToLower(), Warmest);
            // hydro map color
            ColorExtensions2.ColorMappings.Add("Dryest".ToLower(), Dryest);
            ColorExtensions2.ColorMappings.Add("Dryer".ToLower(), Dryer);
            ColorExtensions2.ColorMappings.Add("Dry".ToLower(), Dry);
            ColorExtensions2.ColorMappings.Add("Wet".ToLower(), Wet);
            ColorExtensions2.ColorMappings.Add("Wetter".ToLower(), Wetter);
            ColorExtensions2.ColorMappings.Add("Wettest".ToLower(), Wettest);

            // river map colors
            ColorExtensions2.ColorMappings.Add("IceWater".ToLower(), IceWater);
            ColorExtensions2.ColorMappings.Add("ColdWater".ToLower(), ColdWater);
            ColorExtensions2.ColorMappings.Add("RiverWater".ToLower(), RiverWater);

            // biomes map colors
            ColorExtensions2.ColorMappings.Add("Ice".ToLower(), Ice);
            ColorExtensions2.ColorMappings.Add("Desert".ToLower(), Desert);
            ColorExtensions2.ColorMappings.Add("Savanna".ToLower(), Savanna);
            ColorExtensions2.ColorMappings.Add("TropicalRainforest".ToLower(), TropicalRainforest);
            ColorExtensions2.ColorMappings.Add("Tundra".ToLower(), Tundra);
            ColorExtensions2.ColorMappings.Add("TemperateRainforest".ToLower(), TemperateRainforest);
            ColorExtensions2.ColorMappings.Add("Grassland".ToLower(), Grassland);
            ColorExtensions2.ColorMappings.Add("SeasonalForest".ToLower(), SeasonalForest);
            ColorExtensions2.ColorMappings.Add("BorealForest".ToLower(), BorealForest);
            ColorExtensions2.ColorMappings.Add("Woodland".ToLower(), Woodland);

            // normal tile map colors
            ColorExtensions2.ColorMappings.Add("Dirt".ToLower(), Dirt);
            ColorExtensions2.ColorMappings.Add("DirtRoad".ToLower(), DirtRoad);
            ColorExtensions2.ColorMappings.Add("Wood".ToLower(), Wood);
            ColorExtensions2.ColorMappings.Add("DarkWood".ToLower(), DarkWood);
            ColorExtensions2.ColorMappings.Add("UnclearGlass".ToLower(), UnclearGlass);
        }
    }
}