using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.EntityComponents.Projectiles
{
    public class ProjectileSpellComp : ProjectileBaseComp<SpellEntity>
    {
        public const string Tag = "magic_projectile";

        public ProjectileSpellComp(long ticksToMoveOneStep,
            Point origin,
            Point finalPoint,
            Direction? direction,
            bool isPhysical,
            char[]? glyphs,
            MagiMap map) : base(ticksToMoveOneStep, origin, finalPoint, direction, isPhysical, glyphs, map)
        {
        }

        protected override void OnHit()
        {
            CombatSystem.HitSpellProjectile(Parent, Parent.Spell, _path.GetStep(_currentStep > 0 ? _currentStep - 1 : 0), IgnoresObstacles);
        }

        protected override char TranslateDirToGlyph()
        {
            return TravelDirection.TranslateDirToGlyph(Glyphs);
        }
    }
}
