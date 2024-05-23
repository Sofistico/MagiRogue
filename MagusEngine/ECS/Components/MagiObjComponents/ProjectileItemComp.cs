using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using SadConsole.Effects;
using SadRogue.Primitives;
using System;

namespace MagusEngine.ECS.Components.MagiObjComponents
{
    public class ProjectileItemComp : ProjectileBaseComp<Item>
    {
        public ProjectileItemComp(long ticksToMoveOneStep,
            Point origin,
            Point finalPoint,
            Direction direction,
            bool isPhysical,
            char[]? glyphs,
            double force) : base(ticksToMoveOneStep, origin, finalPoint, direction, isPhysical, glyphs, force)
        {
        }

        // test this way, if it doesn't won't work, make this as an Time event.
        public override long Travel()
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
                    RestoreCellOnRemoved = true,
                };
                Parent!.SadCell.AppearanceSingle!.Effect = blinkGlyph;
                blinkGlyph.Restart();
            }
            if (_path?.Length == _currentStep || Parent?.MoveTo(_path!.GetStep(_currentStep++), IgnoresObstacles) == false)
            {
                // handle hit logic!
                var dmgType = new DamageType(Parent.ItemDamageType);
                CombatSystem.HitProjectile(Parent, _path.GetStep(_currentStep > 0 ? _currentStep - 1 : 0), dmgType, Parent.Material, Force, IgnoresObstacles);
                if (Parent?.SadCell?.AppearanceSingle?.Effect != null)
                    Parent.SadCell.AppearanceSingle.Effect = null;
                Parent?.RemoveComponent(this);
                return 0;
            }

            return TicksToMoveOneStep;
        }

        public override void UpdatePath(MagiMap map)
        {
            _path = map.AStar.ShortestPath(Origin, FinalPoint);
            if (_path == null)
                throw new ApplicationException($"Path is null, can't update path. origin: {Origin}, end: {FinalPoint}");
        }
    }
}
