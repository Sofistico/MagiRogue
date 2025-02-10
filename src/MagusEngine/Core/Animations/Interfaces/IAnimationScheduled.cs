using Arquimedes.Interfaces;
using MagusEngine.Serialization;
using Newtonsoft.Json;

namespace MagusEngine.Core.Animations.Interfaces
{
    [JsonConverter(typeof(AnimationBaseJsonConverter))]
    public interface IAnimationScheduled : IJsonKey
    {
        void ScheduleTickAnimation(Point originPos);
    }
}
