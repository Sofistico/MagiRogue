using System.Diagnostics.CodeAnalysis;
using Arquimedes.Enumerators;
using MagusEngine.Core.Magic.Effects;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Services.Factory.Base;
using Newtonsoft.Json.Linq;

namespace MagusEngine.Services.Factory
{
    public sealed class SpellEffectFactory : GenericFactory<ISpellEffect?>
    {
        public SpellEffectFactory()
        {
            Register("DAMAGE", static token => token.ToObject<DamageEffect>());
            Register("HASTE", static token => token.ToObject<HasteEffect>());
            Register("MAGESIGHT", static token => token.ToObject<MageSightEffect>());
            Register("SEVER", static token => token.ToObject<SeverEffect>());
            Register("TELEPORT", static token => token.ToObject<TeleportEffect>());
            Register("KNOCKBACK", static token => token.ToObject<KnockbackEffect>());
            Register("LIGHT", static token => token.ToObject<LightEffect>());
            Register("MEMISSION", static token => token.ToObject<MEssionEffect>());
            Register("RAISEWALL", static token => token.ToObject<RaiseWallEffect>());
            Register("DIG", static token => token.ToObject<DigEffect>());
        }

        public ISpellEffect? GetSpellEffect(SpellEffectType effect, JToken token) =>
            GetValueFromKey(effect.ToString(), token);

        public bool TryGetSpellEffect(
            SpellEffectType key,
            JToken token,
            [NotNullWhen(true)] out ISpellEffect? effect
        ) => TryGetValueFromKey(key.ToString(), token, out effect);
    }
}
