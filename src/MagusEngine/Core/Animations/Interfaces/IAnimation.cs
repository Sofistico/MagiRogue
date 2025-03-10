using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Serialization;
using Newtonsoft.Json;

namespace MagusEngine.Core.Animations.Interfaces
{
    /// <summary>
    /// This class implements the properties that a animation should use
    /// </summary>
    [JsonConverter(typeof(AnimationBaseJsonConverter))]
    public interface IAnimation : IJsonKey
    {
        public AnimationType AnimationType { get; set; }

        /// <summary>
        /// This should be the glphs the animation will use.
        /// </summary>
        public char[] Glyphs { get; set; }

        /// <summary>
        /// The colors in plain string and or on hex format
        /// </summary>
        public string[] Colors { get; set; }

        void ScheduleTickAnimation(Point originPos);
    }
}
