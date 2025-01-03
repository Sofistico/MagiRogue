using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Animations.Interfaces;
using MagusEngine.Services;

namespace MagusEngine.Core.Animations
{
    public class KaboomAnimation : AnimationBase, IAnimationScheduled
    {
        //    *
        //   ***
        //  #***#
        // +#***#+
        //-+#***#+-
        // +#***#+
        //  #***#
        //   ***
        //    *
        //  *
        // ***
        //*****
        // ***
        //  *
        public int Radius { get; set; }
        public int LingeringTicks { get; set; }
        public int GlyphChangeDistance { get; set; }

        public KaboomAnimation()
        {
        }

        public void ScheduledTickAnimation(Point originPos) { }
    }
}
