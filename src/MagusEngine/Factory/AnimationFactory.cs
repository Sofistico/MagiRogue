using System;
using System.Collections.Generic;
using MagusEngine.Core.Animations;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Factory
{
    public static class AnimationFactory
    {
        public static readonly Dictionary<string, Func<JToken, AnimationBase>> AnimationDictionary = new()
        {
            { "Kaboom", static token => token.ToObject<KaboomAnimation>()! },
        };
    }
}
