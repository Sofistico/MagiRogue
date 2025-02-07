using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Animations.Interfaces;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Utils;
using SadRogue.Primitives;

namespace MagusEngine.Core.Animations
{
    public class KaboomAnimation : AnimationBase, IAnimationScheduled
    {
        public int Radius { get; set; }
        public int LingeringTicks { get; set; }
        public int GlyphChangeDistance { get; set; }

        public KaboomAnimation()
        {
        }

        public void ScheduleTickAnimation(Point originPos)
        {
            var circle = GeometryUtils.CircleFromOriginPoint(originPos, Radius);
            var totalGlyphs = Glyphs.Length;
            foreach (var item in Glyphs)
            {
                var particleGroup = new ParticlesGroup();
            }
        }
    }
}
