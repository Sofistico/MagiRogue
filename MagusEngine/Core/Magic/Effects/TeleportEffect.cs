using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    public class TeleportEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageTypes SpellDamageType { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }
        public bool TargetsTile { get; set; } = true;

        public EffectType EffectType { get; set; } = EffectType.TELEPORT;
        public int BaseDamage { get; set; } = 0;
        public bool CanMiss { get; set; }

        [JsonConstructor]
        public TeleportEffect(SpellAreaEffect areaOfEffect = SpellAreaEffect.Target,
            DamageTypes spellDamageType = DamageTypes.None, int radius = 0)
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            Radius = radius;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            if (Commands.ActionManager.MoveActorTo(caster, target))
            {
                GameLoop.AddMessageLog($"{caster.Name} disappeared!");
            }
        }
    }
}