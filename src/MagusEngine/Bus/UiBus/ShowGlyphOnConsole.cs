namespace MagusEngine.Bus.UiBus
{
    public class ShowGlyphOnConsole
    {
        public char Glyph { get; set; }
        public Point Position { get; set; }
        public int WindowTagId { get; set; }

        public ShowGlyphOnConsole(char glyph, int windowTagId, Point position)
        {
            Glyph = glyph;
            WindowTagId = windowTagId;
            Position = position;
        }
    }
}
