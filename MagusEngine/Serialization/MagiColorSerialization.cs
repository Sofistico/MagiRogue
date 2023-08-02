using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MagusEngine.Serialization
{
    public readonly struct MagiColorSerialization
    {
        [JsonIgnore]
        public readonly Color Color { get; }
        public readonly string ColorName { get; }

        public MagiColorSerialization(string colorName)
        {
            if (colorName is not null && ColorExtensions2.ColorMappings.ContainsKey(colorName.ToLower()))
            {
                Color = ColorExtensions2.FromName(colorName);
            }
            else
            {
                Color = Color.PaleVioletRed;
            }

            ColorName = colorName;
        }

        public MagiColorSerialization(uint packedValue)
        {
            Color = new Color(packedValue);
            ColorName = "Packed Color";
        }

        public MagiColorSerialization(Color color)
        {
            Color = color;
            ColorName = "Empty Color";
        }
    }
}