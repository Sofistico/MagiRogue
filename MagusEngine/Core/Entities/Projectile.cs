using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using SadRogue.Primitives;

namespace MagusEngine.Core.Entities
{
    public class Projectile : MagiEntity
    {
        private static readonly char[] _defaultGlyphs = [
            '.',
            '|',
            '/',
            '-',
            '\\',
            '|',
            '\\',
            '-',
            '/'
        ];
        public int TurnsToMove { get; set; }
        public Point FinalPoint { get; set; }
        public Direction Direciton { get; set; }
        public bool IsPhysical { get; set; }

        /// <summary>
        /// Must follow the Direction.Types enum
        /// </summary>
        public char[] Glyphs { get; set; }

        public Projectile(int turnsToMove,
            Point origin,
            Point finalPoint,
            Direction direction,
            bool isPhysical,
            char[]? glyphs,
            Color foreground,
            Color background) : base(foreground, background, TranslateDirToGlyph(direction, glyphs ?? _defaultGlyphs), origin, (int)MapLayer.SPECIAL)
        {
            Direciton = direction;
            TurnsToMove = turnsToMove;
            FinalPoint = finalPoint;
            IsPhysical = isPhysical;
            Glyphs = glyphs ?? _defaultGlyphs;
        }

        public char TranslateDirToGlyph()
        {
            return TranslateDirToGlyph(Direciton, Glyphs);
        }

        public char TranslateDirToGlyph(Direction dir)
        {
            return TranslateDirToGlyph(dir, Glyphs);
        }

        private static char TranslateDirToGlyph(Direction dir, char[] glyphs)
        {
            return glyphs[(int)dir.Type];
        }
    }
}
