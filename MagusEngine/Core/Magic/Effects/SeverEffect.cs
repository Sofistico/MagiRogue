using Arquimedes.Enumerators;
using MagusEngine.Core.Entities;
using MagusEngine.Systems;
using Newtonsoft.Json;

namespace MagusEngine.Core.Magic.Effects
{
    /// <summary>
    /// Spell like a blade, designed to chop people limbs off
    /// </summary>
    public class SeverEffect : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public string SpellDamageTypeId { get; set; }
        public int Radius { get; set; }
        public double ConeCircleSpan { get; set; }

        public bool TargetsTile { get; set; } = false;
        public EffectType EffectType { get; set; } = EffectType.SEVER;
        public int BaseDamage { get; set; }
        public bool CanMiss { get; set; }
        public string? EffectMessage { get; set; }

        [JsonConstructor]
        public SeverEffect(SpellAreaEffect areaOfEffect, string spellDamageTypeId, int radius, int dmg)
        {
            AreaOfEffect = areaOfEffect;
            SpellDamageTypeId = spellDamageTypeId;
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
            Actor? poorGuy = Find.CurrentMap?.GetEntityAt<Actor>(target);
            int luck = GoRogue.DiceNotation.Dice.Roll($"{spellCasted.SpellLevel}d{spellCasted.Power}");

            if (poorGuy?.GetAnatomy().Limbs.Count > 0
                && MagicManager.PenetrateResistance(spellCasted, caster, poorGuy, luck))
            {
                var rng = GoRogue.Random.GlobalRandom.DefaultRNG;
                int i = rng.NextInt(poorGuy.GetAnatomy().Limbs.Count);

                Limb limbToLose = poorGuy.GetAnatomy().Limbs[i];

                Wound injury = new(GetDamageType(), limbToLose.Tissues);
                poorGuy.GetAnatomy().Injury(injury, limbToLose, poorGuy);

                if (poorGuy is not null)
                {
                    DamageEffect damage = new(BaseDamage,
                        AreaOfEffect,
                        SpellDamageTypeId,
                        canMiss: true,
                        radius: Radius,
                        isResistable: true);
                    damage.ApplyEffect(target, caster, spellCasted);
                }
            }
        }

        public DamageType? GetDamageType()
        {
            return DataManager.QueryDamageInData(SpellDamageTypeId);
        }
    }
}
