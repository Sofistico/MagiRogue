using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Components.EntityComponents.Animations;
using MagusEngine.Services;

namespace MagusEngine.Components.SpellComponents.Animations
{
    public class KaboomAnimation : IAnimationHit
    {
        public int Radius { get; set; }
        public char[] AsciiAnimationPerFrame { get; set; } = ['*', '+', '#', '-', '.'];

        public KaboomAnimation(int radius)
        {
            Radius = radius;
        }

        public void AnimateHit(Point position)
        {
            var request = new ShowGlyphOnConsole(AsciiAnimationPerFrame[0], WindowTag.Map, position);
            Locator.GetService<MessageBusService>().SendMessage(request);
        }
    }
}
