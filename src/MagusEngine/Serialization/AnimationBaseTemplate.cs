using System;
using Arquimedes.Enumerators;
using MagusEngine.Core.Animations;
using MagusEngine.Factory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Serialization
{
    public class AnimationBaseJsonConverter : JsonConverter<AnimationBase>
    {
        public override AnimationBase? ReadJson(JsonReader reader, Type objectType, AnimationBase? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject animation = JObject.Load(reader);
            var animationType = animation.Value<AnimationType>("AnimationType");
            var animation = AnimationFactory.
        }

        public override void WriteJson(JsonWriter writer, AnimationBase? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class AnimationBaseTemplate
    {
    }
}
