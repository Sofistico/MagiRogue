using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using SadConsole.Effects;
using SadRogue.Primitives;
using System;

namespace MagusEngine.ECS.Components.ActorComponents
{
    public class ProjectileComp : ParentAwareComponentBase<MagiEntity>
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

        public const string Tag = "projectile";
        public long TicksToMoveOneStep { get; set; }
        public Point Origin { get; set; }
        public Point FinalPoint { get; set; }
        public Direction Direciton { get; set; }
        public bool IgnoresObstacles { get; set; }
        public double Acceleration { get; set; }

        /// <summary>
        /// Must follow the Direction.Types enum
        /// </summary>
        public char[] Glyphs { get; set; }

        public ProjectileComp(long ticksToMoveOneStep,
            Point origin,
            Point finalPoint,
            Direction direction,
            bool isPhysical,
            char[]? glyphs,
            double acceleration)
        {
            Direciton = direction;
            TicksToMoveOneStep = ticksToMoveOneStep;
            Origin = origin;
            FinalPoint = finalPoint;
            IgnoresObstacles = isPhysical;
            Glyphs = glyphs ?? _defaultGlyphs;
            Acceleration = acceleration;
        }

        public long Travel()
        {
            if (_currentStep == 0)
            {
                var blinkGlyph = new BlinkGlyph()
                {
                    BlinkCount = -1,
                    RunEffectOnApply = true,
                    BlinkSpeed = TimeSpan.FromSeconds(0.5),
                    GlyphIndex = TranslateDirToGlyph(),
                    RemoveOnFinished = true,
                    RestoreCellOnRemoved = true
                };
                Parent.SadCell.AppearanceSingle.Effect = blinkGlyph;
                blinkGlyph.Restart();
            }
            if (_path?.Length == _currentStep || Parent?.MoveTo(_path.GetStep(_currentStep++), IgnoresObstacles) == false)
            {
                // handle hit logic!
                var entity = Parent.CurrentMagiMap.GetEntityAt<MagiEntity>(Parent.Position);
                if (entity != null)
                {
                    CombatSystem.();
                }

                Parent?.RemoveComponent(Tag);
            }

            return TicksToMoveOneStep;
        }

        public void UpdatePath(MagiMap map)
        {
            _path = map.AStar.ShortestPath(Origin, FinalPoint);
        }

        private char TranslateDirToGlyph()
        {
            return TranslateDirToGlyph(Direciton, Glyphs);
        }

        private char TranslateDirToGlyph(Direction dir)
        {
            return TranslateDirToGlyph(dir, Glyphs);
        }

        private Point PointInCurrentPath()
        {
            if (_path != null)
                return _path.GetStep(_currentStep++);
            return Point.None;
        }

        private static char TranslateDirToGlyph(Direction dir, char[] glyphs)
        {
            if (glyphs.Length == 1)
                return glyphs[0];
            return glyphs[(int)dir.Type];
        }
    }
}
