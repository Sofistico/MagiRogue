using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagusEngine.Core.MapStuff;
using SadRogue.Primitives;
using System;

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

        protected ProjectileBaseComp(long ticksToMoveOneStep,
            Point origin,
            Point finalPoint,
            Direction direction,
            bool isPhysical,
            char[]? glyphs,
            double force)
        {
            Direciton = direction;
            TicksToMoveOneStep = ticksToMoveOneStep;
            Origin = origin;
            FinalPoint = finalPoint;
            IgnoresObstacles = !isPhysical;
            Glyphs = glyphs ?? _defaultGlyphs;
            Force = force;
        }

        public abstract long Travel();

        public void UpdatePath(MagiMap map)
        {
            _path = map.AStar.ShortestPath(Origin, FinalPoint);
            if (_path == null)
                throw new ApplicationException($"Path is null, can't update path. origin: {Origin}, end: {FinalPoint}");
        }

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
