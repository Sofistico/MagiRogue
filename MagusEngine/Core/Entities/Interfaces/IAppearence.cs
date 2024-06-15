using SadConsole;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities.Interfaces
{
    public interface IAppearence
    {
        public string[] Fores { get; set; }

        public string Fore { get; set; }

        public string[] Backs { get; set; }

        public string Back { get; set; }

        public char[] Glyphs { get; set; }

        public Color Foreground { get; }
        public Color Background { get; }

        public ColoredGlyph GetSadGlyph();
    }
}
