using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Factory
{
    public class GenericFactory<T>
    {
        public Dictionary<string, Func<JToken, T>> Factories { get; } = [];
    }
}
