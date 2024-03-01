using GoRogue.Pathing;
using MagusEngine.Core.MapStuff;
using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.ActorComponents
{
    public class ProjectileComp
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
        private Path? _path;
        private int _currentStep;

        public int TurnsToMove { get; set; }
        public Point Origin { get; set; }
        public Point FinalPoint { get; set; }
        public Direction Direciton { get; set; }
        public bool IsPhysical { get; set; }

        /// <summary>
        /// Must follow the Direction.Types enum
        /// </summary>
        public char[] Glyphs { get; set; }

        public ProjectileComp(int turnsToMove,
            Point origin,
            Point finalPoint,
            Direction direction,
            bool isPhysical,
            char[]? glyphs)
        {
            Direciton = direction;
            TurnsToMove = turnsToMove;
            Origin = origin;
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

        public void UpdatePath(MagiMap map)
        {
            _path = map.AStar.ShortestPath(Origin, FinalPoint);
        }

        public Point PointInCurrentPath()
        {
            if (_path != null)
                return _path.GetStep(_currentStep++);
            return Point.None;
        }

        private static char TranslateDirToGlyph(Direction dir, char[] glyphs)
        {
            return glyphs[(int)dir.Type];
        }
    }
}
