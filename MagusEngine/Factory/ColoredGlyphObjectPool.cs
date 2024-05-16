using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagusEngine.Factory
{
    public static class ColoredGlyphObjectPool
    {
        private static readonly Dictionary<int, ColoredGlyph> _glyphs = [];

        public static ColoredGlyph GetColoredGlyph(Color foreground, Color background, char glyph)
        {
            var key = GetKeyFromGlyph(foreground.PackedValue, background.PackedValue, glyph);
            if (_glyphs.TryGetValue(key, out var glyphFromDict))
            {
                return glyphFromDict;
            }

            var newGlyph = new ColoredGlyph(foreground, background, glyph);
            AddGlyphToDict(newGlyph, key);
            return newGlyph;
        }

        public static ColoredGlyph GetColoredGlyph(ColoredGlyph glyph)
        {
            return GetColoredGlyph(glyph.Foreground, glyph.Background, glyph.GlyphCharacter);
        }

        private static bool AddGlyphToDict(ColoredGlyph glyph, int? key = null)
        {
            key ??= GetKeyFromGlyph(glyph);
            return _glyphs.TryAdd(key.Value, glyph);
        }

        private static int GetKeyFromGlyph(ColoredGlyph glyph)
        {
            uint fore = glyph.Foreground.PackedValue;
            uint back = glyph.Background.PackedValue;
            uint glyphchar = (uint)glyph.Glyph;
            return GetKeyFromGlyph(fore, back, glyphchar);
        }

        private static int GetKeyFromGlyph(uint fore, uint back, uint glyph)
        {
            return HashCode.Combine(fore, back, glyph);
        }
    }
}
