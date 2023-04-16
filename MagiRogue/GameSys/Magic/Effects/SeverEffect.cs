using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using Newtonsoft.Json;

namespace MagiRogue.GameSys.Magic.Effects
{
    /// <summary>
    /// Spell like a blade, designed to chop people limbs off
    /// </summary>
    public class SeverEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageTypes SpellDamageType { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }

        public bool TargetsTile { get; set; } = false;
        public EffectType EffectType { get; set; } = EffectType.SEVER;
        public int BaseDamage { get; set; }
        public bool CanMiss { get; set; }

        [JsonConstructor]
        public SeverEffect(SpellAreaEffect areaOfEffect, DamageTypes spellDamageType, int radius, int dmg)
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

            if (poorGuy?.GetAnatomy().Limbs.Count > 0
                && MagicManager.PenetrateResistance(spellCasted, caster, poorGuy, luck))
            {
                var rng = GoRogue.Random.GlobalRandom.DefaultRNG;
                int i = rng.NextInt(poorGuy.GetAnatomy().Limbs.Count);

                Limb limbToLose = poorGuy.GetAnatomy().Limbs[i];

                //poorGuy.GetAnatomy().Dismember(limbToLose, poorGuy);
                Wound injury = new Wound(limbToLose.MaxBodyPartHp, DamageTypes.Sharp);
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