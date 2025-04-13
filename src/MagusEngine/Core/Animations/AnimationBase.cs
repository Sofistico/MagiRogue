using Arquimedes.Enumerators;
using MagusEngine.Core.Animations.Interfaces;

namespace MagusEngine.Core.Animations
{
    public abstract class AnimationBase : IAnimation
    {
        public required string Id { get; set; }
        public required AnimationType AnimationType { get; set; }
        public required char[] Glyphs { get; set; }
        public required string[] Colors { get; set; }

        public virtual void ScheduleTickAnimation(Point originPos, bool ignoreWall)
        {
            throw new System.NotImplementedException();
        }
    }
}
