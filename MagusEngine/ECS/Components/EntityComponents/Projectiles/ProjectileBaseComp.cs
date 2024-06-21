﻿using GoRogue.Components.ParentAware;
using GoRogue.Pathing;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using SadConsole.Effects;
using SadRogue.Primitives;
using System;

namespace MagusEngine.ECS.Components.EntityComponents.Projectiles
{
    public abstract class ProjectileBaseComp<T> : ParentAwareComponentBase<T> where T : MagiEntity
    {
        protected static readonly char[] _defaultTravelingGlyphs = [
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

        public long TicksToMoveOneStep { get; set; }
        public Point Origin { get; set; }
        public Point FinalPoint { get; set; }
        public Direction TravelDirection { get; set; }
        public bool IgnoresObstacles { get; set; }
        public double Force { get; set; }

        /// <summary>
        /// Must follow the Direction.Types enum
        /// </summary>
        public char[] Glyphs { get; set; }

        protected ProjectileBaseComp(long ticksToMoveOneStep,
            Point origin,
            Point finalPoint,
            Direction? direction,
            bool isPhysical,
            char[]? glyphs,
            double force,
            MagiMap? map = null)
        {
            TravelDirection = direction ?? Direction.GetDirection(origin, finalPoint);
            TicksToMoveOneStep = ticksToMoveOneStep;
            Origin = origin;
            FinalPoint = finalPoint;
            IgnoresObstacles = !isPhysical;
            Glyphs = glyphs ?? _defaultTravelingGlyphs;
            Force = force;
            if (map != null)
                UpdatePath(map);
        }

        public long Travel()
        {
            if (_currentStep == 0)
            {
                TravelAnimation();
            }
            if (TickMovement())
            {
                // handle hit logic!
                OnHit();
                if (Parent?.SadCell?.AppearanceSingle?.Effect != null)
                    Parent.SadCell.AppearanceSingle.Effect = null;
                Parent?.RemoveComponent(this);
                return 0;
            }

            return TicksToMoveOneStep;
        }

        protected virtual void TravelAnimation()
        {
            var blinkGlyph = new BlinkGlyph()
            {
                BlinkCount = -1,
                RunEffectOnApply = true,
                BlinkSpeed = TimeSpan.FromSeconds(0.5),
                GlyphIndex = TranslateDirToGlyph(),
                RemoveOnFinished = true,
                RestoreCellOnRemoved = true,
            };
            Parent!.SadCell.AppearanceSingle!.Effect = blinkGlyph;
            blinkGlyph.Restart();
        }

        protected virtual bool TickMovement() => _path?.Length == _currentStep || Parent?.MoveTo(_path!.GetStep(_currentStep++), IgnoresObstacles) == false;

        protected abstract void OnHit();

        public void UpdatePath(MagiMap map)
        {
            _path = map.AStar.ShortestPath(Origin, FinalPoint);
            if (_path == null)
                throw new ApplicationException($"Path is null, can't update path. origin: {Origin}, end: {FinalPoint}");
        }

        protected char TranslateDirToGlyph()
        {
            return TravelDirection.TranslateDirTypeToGlyph(Glyphs);
        }

        protected Point PointInCurrentPath()
        {
            if (_path != null)
                return _path.GetStep(_currentStep++);
            return Point.None;
        }
    }
}