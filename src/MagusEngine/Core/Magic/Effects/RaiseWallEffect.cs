using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Services;

namespace MagusEngine.Core.Magic.Effects
{
    public class RaiseWallEffect : SpellEffectBase
    {
        public RaiseWallEffect()
        {
            EffectType = SpellEffectType.RAISEWALL.ToString();
        }

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            if (!TileHelpers.ChangeTileEffect(target, caster, spellCasted.Power, TileType.Wall))
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("Can't make the change to this tile"));
        }
    }
}
