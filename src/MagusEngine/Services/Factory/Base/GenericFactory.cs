using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Services.Factory.Base
{
    public abstract class GenericFactory<T>
    {
        protected Dictionary<string, Func<JToken, T>> Factories { get; } = [];

        public virtual bool Register(string key, Func<JToken, T> factory)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));
            }
            ArgumentNullException.ThrowIfNull(factory);

            return Factories.TryAdd(key, factory);
        }

        public virtual T GetValueFromKey(string key, JToken token)
        {
            return Factories.TryGetValue(key, out var factory)
                ? factory(token)
                : throw new KeyNotFoundException(
                    "Tried to find a key that doensn't exists on the dictionary"
                );
        }

        public virtual bool TryGetValueFromKey(
            string key,
            JToken token,
            [NotNullWhen(true)] out T? value
        )
        {
            if (Factories.TryGetValue(key, out var factory))
            {
                value = factory(token)!;
                return true;
            }
            value = default;
            return false;
        }
    }
}
