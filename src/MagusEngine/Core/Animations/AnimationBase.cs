using Arquimedes.Interfaces;

namespace MagusEngine.Core.Animations
{
    /// <summary>
    /// This class implements the properties that a animation should use
    /// </summary>
    public abstract class AnimationBase : IJsonKey
    {
        public string Id { get; set; }
        public required string AnimationId { get; set; }
        public int LingeringTicks { get; set; }

        /// <summary>
        /// This should be the glphs the animation will use.
        /// </summary>
        public required char[] Glyphs { get; set; }

        /// <summary>
        /// The colors in plain string and or on hex format
        /// </summary>
        public required string[] Colors { get; set; }

        public AnimationBase() { }
    }
}
