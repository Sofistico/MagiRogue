using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagusEngine.Core.MapStuff;
using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.MagiObjComponents
{
    public abstract class ProjectileBaseComp<T> : ParentAwareComponentBase<T> where T : class
    {
        protected static readonly char[] _defaultGlyphs = [
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
        protected Path? _path;
        protected int _currentStep;

        public const string Tag = "projectile";
        public long TicksToMoveOneStep { get; set; }
        public Point Origin { get; set; }
        public Point FinalPoint { get; set; }
        public Direction Direciton { get; set; }
        public bool IgnoresObstacles { get; set; }
        public double Force { get; set; }

        /// <summary>
        /// Must follow the Direction.Types enum
        /// </summary>
        public char[] Glyphs { get; set; }

        public abstract long Travel();

        public abstract void UpdatePath(MagiMap map);

        protected char TranslateDirToGlyph()
        {
            return TranslateDirToGlyph(Direciton, Glyphs);
        }

        protected char TranslateDirToGlyph(Direction dir)
        {
            return TranslateDirToGlyph(dir, Glyphs);
        }

        protected Point PointInCurrentPath()
        {
            if (_path != null)
                return _path.GetStep(_currentStep++);
            return Point.None;
        }

        protected static char TranslateDirToGlyph(Direction dir, char[] glyphs)
        {
            if (glyphs.Length == 1)
                return glyphs[0];
            return glyphs[(int)dir.Type];
        }
    }
}
