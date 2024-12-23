using System;
using Arquimedes.Enumerators;
using MagusEngine.Core.Animations;
using MagusEngine.Services.Factory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Serialization
{
    public class AnimationBaseJsonConverter : JsonConverter<AnimationBase>
    {
        public override AnimationBase? ReadJson(
            JsonReader reader,
            Type objectType,
            AnimationBase? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            JObject jsonNimation = JObject.Load(reader);
            var animationType = jsonNimation.Value<AnimationType>("AnimationType");
            var animation = Locator.GetService<AnimationFactory>().GetValueFromKey(animationType.ToString(), jsonNimation);
            return animation;
        }

        public override void WriteJson(
            JsonWriter writer,
            AnimationBase? value,
            JsonSerializer serializer
        )
        {
            throw new NotImplementedException();
        }
    }
}
