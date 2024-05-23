using Arquimedes.Enumerators;
using MagusEngine.Actions;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class TeleportEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public string SpellDamageTypeId { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; } = true;

        public EffectType EffectType { get; set; } = EffectType.TELEPORT;
        public int BaseDamage { get; set; } = 0;
        public bool CanMiss { get; set; }
        public bool IsResistable { get; set; }
        public string? EffectMessage { get; set; }
        public int Volume { get; set; }
        public bool IgnoresWall { get; set; }

        [JsonConstructor]
        public TeleportEffect(SpellAreaEffect areaOfEffect = SpellAreaEffect.Target,
            string spellDamageTypeId = null, int radius = 0)
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageTypeId = spellDamageTypeId;
            Radius = radius;
        }

        public void ApplyEffect(Point target, Actor caster, Spell spellCasted)
        {
            if (ActionManager.MoveActorTo(caster, target))
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{caster.Name} disappeared!"));
            }
        }

        public DamageType? GetDamageType()
        {
            return null;
        }
    }
}