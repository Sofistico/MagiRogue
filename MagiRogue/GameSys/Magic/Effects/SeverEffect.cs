using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;

namespace MagiRogue.GameSys.Magic.Effects
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
            Actor poorGuy = GameLoop.GetCurrentMap().GetEntityAt<Actor>(target);
            int luck = GoRogue.DiceNotation.Dice.Roll($"{spellCasted.SpellLevel}d{spellCasted.Power}");

            if (poorGuy is not null && poorGuy.GetAnatomy().Limbs.Count > 0
                && MagicManager.PenetrateResistance(spellCasted, caster, poorGuy, luck))
            {
                var rng = GoRogue.Random.GlobalRandom.DefaultRNG;
                int i = rng.NextInt(poorGuy.GetAnatomy().Limbs.Count);

                Limb limbToLose = poorGuy.GetAnatomy().Limbs[i];

                //poorGuy.GetAnatomy().Dismember(limbToLose, poorGuy);
                Wound injury = new Wound(limbToLose.MaxBodyPartHp, DamageType.Sharp);
                poorGuy.GetAnatomy().Injury(injury, limbToLose, poorGuy);

                if (poorGuy is not null)
                {
                    DamageEffect damage = new
                        DamageEffect(BaseDamage, AreaOfEffect, SpellDamageType, canMiss: true, radius: Radius,
                        isResistable: true);
                    damage.ApplyEffect(target, caster, spellCasted);
                }
            }
        }
    }
}