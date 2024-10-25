using SadConsole;

namespace MagusEngine.Components.TilesComponents
{
    public class ExtraAppearanceComponent
    {
        public ColoredGlyph SadGlyph { get; }

        public ExtraAppearanceComponent(ColoredGlyph sadGlyph)
        {
            SadGlyph = sadGlyph;
        }
    }
}
