using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.EntityComponents.Projectiles
{
    public class ProjectileSpellComp : ProjectileBaseComp<SpellEntity>
    {
        public ProjectileSpellComp(long ticksToMoveOneStep,
            Point origin,
            Point finalPoint,
            Direction? direction,
            bool isPhysical,
            char[]? glyphs,
            double force,
            MagiMap map) : base(ticksToMoveOneStep, origin, finalPoint, direction, isPhysical, glyphs, force, map)
        {
        }

        protected override void OnHit()
        {
            CombatSystem.HitProjectile(Parent, _path.GetStep(_currentStep > 0 ? _currentStep - 1 : 0), Parent.Spell, Force, IgnoresObstacles);
        }
    }
}
