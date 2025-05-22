using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.MapStuff;
using MagusEngine.Services;
using MagusEngine.Services.Factory;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils;

namespace MagusEngine.Core.Magic.Effects
{
    public class DigEffect : SpellEffectBase
    {
        public DigEffect()
        {
            EffectType = SpellEffectType.DIG.ToString();
        }

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            if (caster is null || caster?.CurrentMagiMap is null)
                return;

            var tile = caster.CurrentMagiMap?.GetTileAt<Tile>(target)!;
            if (tile.IsWalkable)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("There is nothing to dig there", firstOrThirdPerson: PointOfView.Third));
                return;
            }
            var threashold = PhysicsSystem.GetMiningDificulty(tile.Material);
            if ((spellCasted.Power * (125 + Mrn.Normal1D100Dice)) <= threashold)
                return;

            tile.ChangeTileType(TileType.Floor);
        }
    }
}
