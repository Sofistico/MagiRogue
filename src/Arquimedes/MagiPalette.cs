using SadConsole.UI;
using SadRogue.Primitives;

namespace Arquimedes
{
    public static class MagiPalette
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
        public static readonly Color DarkGrassColor = GrassColor.GetDark();

        public static void AddToColorDictionary()
        {
            // bizarre, why is this here?
            if (ColorExtensions2.ColorMappings.ContainsKey(nameof(DeepWaterColor).ToLower()))
                return;
            // height map color
            ColorExtensions2.ColorMappings.Add(nameof(DeepWaterColor).ToLower(), DeepWaterColor);
            ColorExtensions2.ColorMappings.Add(nameof(ShallowWaterColor).ToLower(), ShallowWaterColor);
            ColorExtensions2.ColorMappings.Add(nameof(RiverColor).ToLower(), RiverColor);
            ColorExtensions2.ColorMappings.Add(nameof(SandColor).ToLower(), SandColor);
            ColorExtensions2.ColorMappings.Add(nameof(GrassColor).ToLower(), GrassColor);
            ColorExtensions2.ColorMappings.Add(nameof(ForestColor).ToLower(), ForestColor);
            ColorExtensions2.ColorMappings.Add(nameof(RockColor).ToLower(), RockColor);
            ColorExtensions2.ColorMappings.Add(nameof(HighMountainColor).ToLower(), HighMountainColor);
            ColorExtensions2.ColorMappings.Add(nameof(SnowColor).ToLower(), SnowColor);
            ColorExtensions2.ColorMappings.Add(nameof(MagicColor).ToLower(), MagicColor);

            // temperature map color
            ColorExtensions2.ColorMappings.Add(nameof(Coldest).ToLower(), Coldest);
            ColorExtensions2.ColorMappings.Add(nameof(Colder).ToLower(), Colder);
            ColorExtensions2.ColorMappings.Add(nameof(Cold).ToLower(), Cold);
            ColorExtensions2.ColorMappings.Add(nameof(Warm).ToLower(), Warm);
            ColorExtensions2.ColorMappings.Add(nameof(Warmer).ToLower(), Warmer);
            ColorExtensions2.ColorMappings.Add(nameof(Warmest).ToLower(), Warmest);
            // hydro map color
            ColorExtensions2.ColorMappings.Add(nameof(Dryest).ToLower(), Dryest);
            ColorExtensions2.ColorMappings.Add(nameof(Dryer).ToLower(), Dryer);
            ColorExtensions2.ColorMappings.Add(nameof(Dry).ToLower(), Dry);
            ColorExtensions2.ColorMappings.Add(nameof(Wet).ToLower(), Wet);
            ColorExtensions2.ColorMappings.Add(nameof(Wetter).ToLower(), Wetter);
            ColorExtensions2.ColorMappings.Add(nameof(Wettest).ToLower(), Wettest);

            // river map colors
            ColorExtensions2.ColorMappings.Add(nameof(IceWater).ToLower(), IceWater);
            ColorExtensions2.ColorMappings.Add(nameof(ColdWater).ToLower(), ColdWater);
            ColorExtensions2.ColorMappings.Add(nameof(RiverWater).ToLower(), RiverWater);

            // biomes map colors
            ColorExtensions2.ColorMappings.Add(nameof(Ice).ToLower(), Ice);
            ColorExtensions2.ColorMappings.Add(nameof(Desert).ToLower(), Desert);
            ColorExtensions2.ColorMappings.Add(nameof(Savanna).ToLower(), Savanna);
            ColorExtensions2.ColorMappings.Add(nameof(TropicalRainforest).ToLower(), TropicalRainforest);
            ColorExtensions2.ColorMappings.Add(nameof(Tundra).ToLower(), Tundra);
            ColorExtensions2.ColorMappings.Add(nameof(TemperateRainforest).ToLower(), TemperateRainforest);
            ColorExtensions2.ColorMappings.Add(nameof(Grassland).ToLower(), Grassland);
            ColorExtensions2.ColorMappings.Add(nameof(SeasonalForest).ToLower(), SeasonalForest);
            ColorExtensions2.ColorMappings.Add(nameof(BorealForest).ToLower(), BorealForest);
            ColorExtensions2.ColorMappings.Add(nameof(Woodland).ToLower(), Woodland);

            // normal tile map colors
            ColorExtensions2.ColorMappings.Add(nameof(Dirt).ToLower(), Dirt);
            ColorExtensions2.ColorMappings.Add(nameof(DirtRoad).ToLower(), DirtRoad);
            ColorExtensions2.ColorMappings.Add(nameof(Wood).ToLower(), Wood);
            ColorExtensions2.ColorMappings.Add(nameof(DarkWood).ToLower(), DarkWood);
            ColorExtensions2.ColorMappings.Add(nameof(UnclearGlass).ToLower(), UnclearGlass);

            ColorExtensions2.ColorMappings.Add(nameof(DarkGrassColor).ToLower(), DarkGrassColor);
        }

        /// <summary>
        /// Convert a hex string to a Color object
        /// </summary>
        /// <param name="hex">Should follow the format #rrggbb, for now doesn't support alpha</param>
        /// <returns>Returns an Color object</returns>
        public static Color FromHexString(string hex)
        {
            if (hex.StartsWith('#'))
                hex = hex[1..];

            if (hex.Length != 6)
                throw new ArgumentException("Hex color must be 6 characters long (RRGGBB)");

            byte r = Convert.ToByte(hex[0..2], 16);
            byte g = Convert.ToByte(hex[2..4], 16);
            byte b = Convert.ToByte(hex[4..6], 16);
            byte a = 255; // fully opaque

            // Pack to RGBA (Red is most significant byte)
            // This is the the layout it should have in memory, since uint is a 32 byte:
            // Bits:   [RRRRRRRR][GGGGGGGG][BBBBBBBB][AAAAAAAA]
            // Index:     24-31     16-23     8-15       0-7
            uint packedValue = (uint)((r << 24) | (g << 16) | (b << 8) | a);

            return new Color(packedValue);
        }
    }
}
