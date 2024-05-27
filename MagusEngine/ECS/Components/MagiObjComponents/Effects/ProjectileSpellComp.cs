﻿using MagusEngine.Core;
using MagusEngine.Core.Magic;
using MagusEngine.Systems;
using SadConsole.Effects;
using SadRogue.Primitives;
using System;

namespace MagusEngine.ECS.Components.MagiObjComponents.Effects
{
    public class ProjectileSpellComp : ProjectileBaseComp<Spell>
    {
        public ProjectileSpellComp(long ticksToMoveOneStep, Point origin, Point finalPoint, Direction direction, bool isPhysical, char[]? glyphs, double force) : base(ticksToMoveOneStep, origin, finalPoint, direction, isPhysical, glyphs, force)
        {
        }

        public override long Travel()
        {
            if (_currentStep == 0)
            {
            }
            if (_path?.Length == _currentStep || Parent?.MoveTo(_path!.GetStep(_currentStep++), IgnoresObstacles) == false)
            {
                //// handle hit logic!
                //var dmgType = new DamageType(Parent.Da);
                //CombatSystem.HitProjectile(Parent, _path.GetStep(_currentStep > 0 ? _currentStep - 1 : 0), dmgType, Parent.Material, Force, IgnoresObstacles);
                //if (Parent?.SadCell?.AppearanceSingle?.Effect != null)
                //    Parent.SadCell.AppearanceSingle.Effect = null;
                //Parent?.RemoveComponent(this);
                //return 0;
                return 0;
            }

            return TicksToMoveOneStep;
        }
    }
}
}
