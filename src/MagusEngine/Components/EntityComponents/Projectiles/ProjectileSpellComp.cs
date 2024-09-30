using MagusEngine.Components.EntityComponents.Animations;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Systems;
using SadRogue.Primitives;

namespace MagusEngine.Components.EntityComponents.Projectiles
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
            MagiMap map,
            bool animate = false) : base(ticksToMoveOneStep, origin, finalPoint, direction, isPhysical, glyphs, map, animate)
        {
        }

        protected override void OnHit()
        {
            CombatSystem.HitSpellProjectile(Parent!, Parent!.Spell, _path!.GetStep(_currentStep > 0 ? _currentStep - 1 : 0), IgnoresObstacles);
        }

        protected override char TranslateDirToGlyph()
        {
            return TravelDirection.TranslateDirToGlyph(Glyphs);
        }

        protected override void OnHitAnimation()
        {
            if(!_animate)
                return;

            if(Parent!.GetComponent<IAnimationHit>(out var animation, "spell-animation"))
            {
                animation.AnimateFrames();
            }
        }
    }
}
