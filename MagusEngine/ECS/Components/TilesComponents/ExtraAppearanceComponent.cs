using SadConsole;

namespace MagusEngine.ECS.Components.TilesComponents
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
