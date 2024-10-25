using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using SadRogue.Primitives;

namespace MagusEngine.Components.EntityComponents.Projectiles
{
    public static class ProjectileHelper
    {
        public const string Tag = "projectile";
        public static readonly char[] DefaultTravelingGlyphs = [
            '.',
            '|',
            '/',
            '-',
            '\\',
            '|',
            '\\',
            '-',
            '/'];

        /// <summary>
        /// This follows the convention of Direction.Types
        /// </summary>
        public static char TranslateDirTypeToGlyph(this Direction dir, char[] glyphs)
        {
            if (glyphs.Length == 1)
                return glyphs[0];
            return glyphs[(int)dir.Type];
        }

        /// <summary>
        /// This follows a custom convetion of mine, first 4 are the cardinal directions starting on left,
        /// then the next 4 are the diagonals that starts as well on left and goes clockwise
        /// </summary>
        public static char TranslateDirToGlyph(this Direction dir, char[] glyphs)
        {
            char glyph;
            if (glyphs.Length == 1)
            {
                glyph = glyphs[0];
            }
            else if (glyphs.Length <= 4)
            {
                glyph = dir.Type switch
                {
                    Direction.Types.Left => glyphs[0],
                    Direction.Types.Up => glyphs[1],
                    Direction.Types.Right => glyphs[2],
                    Direction.Types.Down => glyphs[3],
                    _ => glyphs.GetRandomItemFromList()
                };
            }
            else
            {
                glyph = dir.Type switch
                {
                    Direction.Types.Left => glyphs[0],
                    Direction.Types.Up => glyphs[1],
                    Direction.Types.Right => glyphs[2],
                    Direction.Types.Down => glyphs[3],
                    Direction.Types.UpLeft => glyphs[4],
                    Direction.Types.UpRight => glyphs[5],
                    Direction.Types.DownLeft => glyphs[6],
                    Direction.Types.DownRight => glyphs[7],
                    _ => glyphs.GetRandomItemFromList()
                };
            }
            return (char)glyph.GetGlyph();
        }
    }
}
