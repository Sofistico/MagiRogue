namespace MagusEngine.Bus.UiBus
{
    public class ShowGlyphOnConsole
    {
        public char Glyph { get; set; }
        public int WindowTagId { get; set; }

        public ShowGlyphOnConsole(char glyph, int windowTagId)
        {
            Glyph = glyph;
            WindowTagId = windowTagId;
        }
    }
}
