using Arquimedes;
using MagusEngine.Exceptions;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MagusEngine.Serialization
{
    public readonly struct MagiColorSerialization
    {
        public readonly Color Color { get; }
        public readonly string? ColorName { get; }

        [JsonConstructor]
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

        public static implicit operator Color(MagiColorSerialization color) => color.Color;

        public static implicit operator MagiColorSerialization(string colorName)
        {
            if (ColorExtensions2.ColorMappings.Count == 0)
                MagiPalette.AddToColorDictionary();
            if (!ColorExtensions2.ColorMappings.ContainsKey(colorName.ToLower()))
                throw new ColorNotFoundException(colorName);
            return new(colorName);
        }

        public static implicit operator MagiColorSerialization(uint packedValue) => new(packedValue);
    }
}
