using Arquimedes.Enumerators;

namespace MagusEngine.Bus.UiBus
{
    public class ShowGlyphOnConsole
    {
        public char Glyph { get; set; }
        public Point Position { get; set; }
        public WindowTag WindowTag { get; set; }

        public int LastUsedGlyph { get; set; }

        public ShowGlyphOnConsole(char glyph, WindowTag windowTag, Point position)
        {
            Glyph = glyph;
            WindowTag = windowTag;
            Position = position;
        }
    }
}
