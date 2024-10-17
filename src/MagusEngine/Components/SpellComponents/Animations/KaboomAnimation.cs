using MagusEngine.Components.EntityComponents.Animations;

namespace MagusEngine.Components.SpellComponents.Animations
{
    public class KaboomAnimation : IAnimationHit
    {
        public int Radius { get; set; }
        public char[] AsciiAnimationPerFrame { get; set; } = ['*', '-'];

        public KaboomAnimation() { }

        public void AnimateHit()
        {
        }
    }
}
