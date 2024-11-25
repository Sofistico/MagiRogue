using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Arquimedes.Enumerators;
using MagusEngine.Core.Animations;
using MagusEngine.Core.Magic.Effects;
using MagusEngine.Core.Magic.Interfaces;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Factory
{
    public static class SpellEffectFactory
    {
        public static readonly Dictionary<string, Func<JToken, ISpellEffect>> EffectFactory = new()
        {
            { "DAMAGE", static token => token.ToObject<DamageEffect>()! },
            { "HASTE", static token => token.ToObject<HasteEffect>()! },
            { "MAGESIGHT", static token => token.ToObject<MageSightEffect>()! },
            { "SEVER", static token => token.ToObject<SeverEffect>()! },
            { "TELEPORT", static token => token.ToObject<TeleportEffect>()! },
            { "KNOCKBACK", static token => token.ToObject<KnockbackEffect>()! },
            { "LIGHT", static token => token.ToObject<LightEffect>()! },
            { "MEMISSION", static token => token.ToObject<MEssionEffect>()! },
        };

        public static ISpellEffect GetSpellEffect(string effect, JToken token)
        {
            return EffectFactory.TryGetValue(effect, out var factory)
                ? factory(token)
                : throw new KeyNotFoundException(
                    "Tried to find an effect that doensn't exists on the dictionary"
                );
        }

        public static ISpellEffect GetSpellEffect(EffectType effect, JToken token) =>
            GetSpellEffect(effect.ToString(), token);

        public static bool TryGetSpellEffect(
            string key,
            JToken token,
            [NotNullWhen(true)] out ISpellEffect? effect
        )
        {
            if (EffectFactory.TryGetValue(key, out var factory))
            {
                effect = factory(token);
                return true;
            }
            effect = null;
            return false;
        }

        public static bool TryGetSpellEffect(
            EffectType key,
            JToken token,
            [NotNullWhen(true)] out ISpellEffect? effect
        ) => TryGetSpellEffect(key.ToString(), token, out effect);
    }
}
