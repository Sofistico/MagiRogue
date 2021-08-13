using MagiRogue.Data;
using MagiRogue.Entities;
using MagiRogue.Utils;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Magic.Effects
{
    /// <summary>
    /// Spell like a blade, designed to chop people limbs off
    /// </summary>
    public class SeverEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }
        public int Radius { get; set; }

        public bool TargetsTile { get; set; } = false;
        public EffectTypes EffectType { get; set; } = EffectTypes.SEVER;
        public int BaseDamage { get; set; }

        public SeverEffect(SpellAreaEffect areaOfEffect, DamageType spellDamageType, int radius, int dmg)
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageType = spellDamageType;
            Radius = radius;
            BaseDamage = dmg;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            CutLimb(target, caster, spellCasted);
        }

        private void CutLimb(Point target, Actor caster, SpellBase spellCasted)
        {
            // Actor because only actor have an anatomy
            Actor poorGuy = GameLoop.World.CurrentMap.GetEntityAt<Actor>(target);
            int luck = GoRogue.DiceNotation.Dice.Roll($"{spellCasted.SpellLevel}d{spellCasted.Power}");

            if (poorGuy is not null && poorGuy.Anatomy.Limbs.Count > 0
                && Magic.PenetrateResistance(spellCasted, caster, poorGuy, luck))
            {
                var rng = GoRogue.Random.GlobalRandom.DefaultRNG;
                int i = rng.Next(poorGuy.Anatomy.Limbs.Count);

                TypeOfLimb limbToLose = poorGuy.Anatomy.Limbs[i].TypeLimb;

                poorGuy.Anatomy.Dismember(limbToLose, poorGuy);

                if (poorGuy is not null)
                {
                    DamageEffect damage = new
                        DamageEffect(BaseDamage, AreaOfEffect, SpellDamageType, canMiss: true, radius: Radius);
                    damage.ApplyEffect(target, caster, spellCasted);
                }
            }
        }
    }
}