using Arquimedes.Enumerators;

namespace MagusEngine.Bus.UiBus
{
    public class ShowGlyphOnConsole
    {
        public char Glyph { get; set; }
        public Point Position { get; set; }
        public WindowTag WindowTag { get; set; }

        public int LastUsedGlyph { get; set; }

        /// <summary>
        /// Initializes a new instance of the ShowGlyphOnConsole class.
        /// </summary>
        /// <param name="glyph">The character to display</param>
        /// <param name="windowTag">The target window identifier</param>
        /// <param name="position">The position where the glyph will be displayed</param>
        /// <exception cref="ArgumentNullException">Thrown when position or windowTag is null</exception>
        public ShowGlyphOnConsole(char glyph, WindowTag windowTag, Point position)
        {
            if (windowTag == null)
                throw new ArgumentNullException(nameof(windowTag));
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Glyph = glyph;
            WindowTag = windowTag;
            Position = position;
            LastUsedGlyph = glyph;  // Initialize if this property is needed
        }
    }
}
