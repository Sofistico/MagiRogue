using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.EntityComponents.Projectiles
{
    public class ProjectileItemComp : ProjectileBaseComp<Item>
    {
        public ProjectileItemComp(long ticksToMoveOneStep,
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
            var dmgType = new DamageType(Parent.ItemDamageType);
            CombatSystem.HitProjectile(Parent, _path.GetStep(_currentStep > 0 ? _currentStep - 1 : 0), dmgType, Parent.Material, Force, IgnoresObstacles);
        }
    }
}
