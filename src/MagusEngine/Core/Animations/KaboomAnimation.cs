using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Animations.Interfaces;
using MagusEngine.Services;

namespace MagusEngine.Core.Animations
{
    public class KaboomAnimation : AnimationBase, IAnimationHit
    {
        public int Radius { get; set; }
        public int LingeringTicks { get; set; }

        public KaboomAnimation(int radius, int lingeringTicks)
        {
            Radius = radius;
            LingeringTicks = lingeringTicks;
        }

        public void AnimateHit(Point position)
        {
            var request = new ShowGlyphOnConsole(Glyphs[0], WindowTag.Map, position);
            Locator.GetService<MessageBusService>().SendMessage(request);
        }
    }
}
