using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Magic;
using MagusEngine.Systems;
using SadConsole.Effects;
using SadRogue.Primitives;
using System;

namespace MagusEngine.ECS.Components.EntityComponents.Projectiles
{
    public class ProjectileSpellComp : ProjectileBaseComp<SpellEntity>
    {
        public ProjectileSpellComp(long ticksToMoveOneStep, Point origin, Point finalPoint, Direction direction, bool isPhysical, char[]? glyphs, double force) : base(ticksToMoveOneStep, origin, finalPoint, direction, isPhysical, glyphs, force)
        {
        }

        protected override void OnHit()
        {
            CombatSystem.HitProjectile(Parent, _path.GetStep(_currentStep > 0 ? _currentStep - 1 : 0), Parent.Spell, Force, IgnoresObstacles);
        }
    }
}
