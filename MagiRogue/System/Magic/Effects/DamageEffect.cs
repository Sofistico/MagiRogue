using GoRogue;
using MagiRogue.Entities;
using MagiRogue;
using MagiRogue.System.Time;
using SadRogue.Primitives;
using MagiRogue.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Magic.Effects
{
    public class DamageEffect : ISpellEffect
    {
        public SpellTypeEnum SpellEffect { get; set; }
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }
        public int Damage { get; set; }

        public DamageEffect(int dmg, SpellAreaEffect areaOfEffect, DamageType spellDamageType,
            SpellTypeEnum spellEffect = SpellTypeEnum.Damage)
        {
            Damage = dmg;
            SpellEffect = spellEffect;
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
        }

        public void ApplyEffect(Point target, Stat casterStats)
        {
            switch (AreaOfEffect)
            {
                case SpellAreaEffect.Target:
                    Entity poorGuy = GameLoop.World.CurrentMap.GetEntityAt<Entity>(target);
                    if (poorGuy == null)
                    {
                        return;
                    }

                    if (poorGuy == GameLoop.World.CurrentMap.ControlledEntitiy && poorGuy is Player)
                    {
                        poorGuy = GameLoop.World.CurrentMap.GetClosestEntity(poorGuy.Position, 1);
                    }

                    CombatUtils.DealDamage(Damage, poorGuy, SpellDamageType);

                    if (poorGuy is Item)
                    {
                        // Custom logic here
                    }

                    if (poorGuy is Actor)
                    {
                        // Custom logic here
                    }

                    break;

                case SpellAreaEffect.Ball:
                    break;

                case SpellAreaEffect.Shape:
                    break;

                case SpellAreaEffect.Beam:
                    break;

                case SpellAreaEffect.Level:
                    break;

                case SpellAreaEffect.World:
                    throw new NotImplementedException();

                default:
                    break;
            }
        }
    }
}