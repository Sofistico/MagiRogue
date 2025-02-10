using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Core.Animations.Interfaces;

namespace MagusEngine.Core.Animations
{
    /// <summary>
    /// This class implements the properties that a animation should use
    /// </summary>
    public class AnimationBase : IAnimationScheduled
    {
        public required string Id { get; set; }
        public AnimationType AnimationType { get; set; }

        /// <summary>
        /// This should be the glphs the animation will use.
        /// </summary>
        public required char[] Glyphs { get; set; }

        /// <summary>
        /// The colors in plain string and or on hex format
        /// </summary>
        public required string[] Colors { get; set; }

        public virtual void ScheduleTickAnimation(Point originPos)
        {
            throw new System.NotImplementedException();
        }
    }
}
