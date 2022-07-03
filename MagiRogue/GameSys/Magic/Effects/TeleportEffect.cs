using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Magic.Effects
{
    public class TeleportEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }
        public int Radius { get; set; }
        public bool TargetsTile { get; set; } = true;

        public EffectTypes EffectType { get; set; } = EffectTypes.TELEPORT;
        public int BaseDamage { get; set; } = 0;

        public TeleportEffect(SpellAreaEffect areaOfEffect = SpellAreaEffect.Target,
            DamageType spellDamageType = DamageType.None, int radius = 0)
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            Radius = radius;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            if (Commands.CommandManager.MoveActorTo(caster, target))
            {
                GameLoop.UIManager.MessageLog.Add($"{caster.Name} disappeared!");
            }
        }
    }
}