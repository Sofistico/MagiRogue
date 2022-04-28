using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SadRogue.Primitives;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MagiRogue.Data.Serialization
{
    public struct MagiColorSerialization
    {
        [JsonIgnore]
        public readonly Color Color { get; }
        public readonly string ColorName { get; }

        public MagiColorSerialization(string colorName)
        {
            if (ColorExtensions2.ColorMappings.ContainsKey(colorName.ToLower()))
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