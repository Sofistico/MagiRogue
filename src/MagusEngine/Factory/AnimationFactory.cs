using System;
using System.Collections.Generic;
using MagusEngine.Core.Animations;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Factory
{
    public static class AnimationFactory : test<string, JToken, AnimationBase>
    {
    }

    public class test<TKey, TFunc, TReturn> where TKey : notnull
    {
        private readonly Dictionary<TKey, Func<TFunc, TReturn>> Turn = [];
    }
}
