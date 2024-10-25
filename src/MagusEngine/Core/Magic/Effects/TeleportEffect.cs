using Arquimedes.Enumerators;
using MagusEngine.Actions;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Services;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class TeleportEffect : SpellEffectBase
    {
        public string? EffectMessage { get; set; }

        [JsonConstructor]
        public TeleportEffect(SpellAreaEffect areaOfEffect = SpellAreaEffect.Target,
            string spellDamageTypeId = null, int radius = 0)
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageTypeId = spellDamageTypeId;
            Radius = radius;
            TargetsTile = true;
            EffectType = EffectType.TELEPORT;
        }

        public override void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            var entity = caster?.CurrentMagiMap?.GetEntityAt<MagiEntity>(target);
            if (ActionManager.MoveActorTo(entity, target))
            {
                var entityName = entity?.Name ?? "Unknown entity";
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{entityName} disappeared!"));
            }
        }
    }
}
