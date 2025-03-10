using System;
using Arquimedes.Enumerators;
using MagusEngine.Core.Animations.Interfaces;
using MagusEngine.Exceptions;
using MagusEngine.Services.Factory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Serialization
{
    public class AnimationBaseJsonConverter : JsonConverter<IAnimation>
    {
        public override IAnimation? ReadJson(
            JsonReader reader,
            Type objectType,
            IAnimation? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            JObject jsonNimation = JObject.Load(reader);
            var animationType = jsonNimation["AnimationType"]?.ToString() ?? throw new NullValueException("Value of the AnimationType was null", null);
            var animation = Locator.GetService<AnimationFactory>().GetValueFromKey(animationType, jsonNimation);
            return animation;
        }

        public override void WriteJson(
            JsonWriter writer,
            IAnimation? value,
            JsonSerializer serializer
        )
        {
            throw new NotImplementedException();
        }
    }
}
