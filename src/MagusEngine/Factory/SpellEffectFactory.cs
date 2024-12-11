using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Arquimedes.Enumerators;
using MagusEngine.Core.Magic.Effects;
using MagusEngine.Core.Magic.Interfaces;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Factory
{
    public class SpellEffectFactory : GenericFactory<ISpellEffect>
    {
        // public readonly Dictionary<string, Func<JToken, ISpellEffect>> Factories = new()
        // {
        // { "DAMAGE", token => token.ToObject<DamageEffect>()! },
        // { "HASTE", token => token.ToObject<HasteEffect>()! },
        // { "MAGESIGHT", token => token.ToObject<MageSightEffect>()! },
        // { "SEVER", token => token.ToObject<SeverEffect>()! },
        // { "TELEPORT", token => token.ToObject<TeleportEffect>()! },
        // { "KNOCKBACK", token => token.ToObject<KnockbackEffect>()! },
        // { "LIGHT", token => token.ToObject<LightEffect>()! },
        // { "MEMISSION", token => token.ToObject<MEssionEffect>()! },
        // };

        public SpellEffectFactory()
        {
            RegisterFactory("DAMAGE", static token => token.ToObject<DamageEffect>()!);
        }

        public void RegisterFactory(string effect, Func<JToken, ISpellEffect> factory)
        {
            if (Factories.ContainsKey(effect))
                return;
            Factories.Add(effect, factory);
        }

        public ISpellEffect GetSpellEffect(string effect, JToken token)
        {
            return Factories.TryGetValue(effect, out var factory)
                ? factory(token)
                : throw new KeyNotFoundException(
                    "Tried to find an effect that doensn't exists on the dictionary"
                );
        }

        public ISpellEffect GetSpellEffect(EffectType effect, JToken token) =>
            GetSpellEffect(effect.ToString(), token);

        public bool TryGetSpellEffect(
            string key,
            JToken token,
            [NotNullWhen(true)] out ISpellEffect? effect
        )
        {
            if (Factories.TryGetValue(key, out var factory))
            {
                effect = factory(token);
                return true;
            }
            effect = null;
            return false;
        }

        public bool TryGetSpellEffect(
            EffectType key,
            JToken token,
            [NotNullWhen(true)] out ISpellEffect? effect
        ) => TryGetSpellEffect(key.ToString(), token, out effect);
    }
}
