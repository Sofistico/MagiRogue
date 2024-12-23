using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using SadRogue.Primitives;

namespace MagusEngine.Components.EntityComponents.Projectiles
{
    public class ProjectileItemComp : ProjectileBaseComp<Item>
    {
        private readonly double _force;

        public ProjectileItemComp(long ticksToMoveOneStep,
            Point origin,
            Point finalPoint,
            Direction? direction,
            bool isPhysical,
            char[]? glyphs,
            double force,
            MagiMap map) : base(ticksToMoveOneStep, origin, finalPoint, direction, isPhysical, glyphs, map)
        {
            _force = force;
        }

        protected override void OnHit()
        {
            var dmgType = new DamageType(Parent?.ItemDamageType ?? default);
            CombatSystem.HitProjectile(Parent, _path!.GetStep(_currentStep > 0 ? _currentStep - 1 : 0), dmgType, Parent!.Material!, _force, IgnoresObstacles);
        }
    }
}
