using GoRogue.DiceNotation;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.Entities.Core;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MagiRogue.GameSys.Magic
{
    /// <summary>
    /// The class that is the manager of the magic system to an entity
    /// </summary>
    public class MagicManager
    {
        // Create a magic inspired by Mother of learning
        public List<SpellBase> KnowSpells { get; set; }
        public List<EffectType> KnowEffects { get; set; }
        public List<SpellAreaEffect> KnowArea { get; set; }
        public List<DamageTypes> KnowDamageTypes { get; set; }

        /// <summary>
        /// The amount of mana finess required to pull of a spell, something can only be casted if you can
        /// have enough control to properly control the mana, see <see cref="SpellBase.Proficiency"/>.
        /// <para> Should be at minimum a 3 to cast the simplest battle spell reliable.</para>
        /// </summary>
        public int ShapingSkill { get; set; }

        /// <summary>
        /// The innate resistance to magic from a being, how hard it is to harm another with pure magic
        /// </summary>
        public int InnateMagicResistance { get; set; } = 1;

        /// <summary>
        /// The resistance to magic from other stuff add to a being
        /// </summary>
        public int MagicResistance { get; set; } = 0;

        /// <summary>
        /// How likely it is to penetrate the resistance of another being,
        /// the formula should be to win against
        /// the resistance
        /// ((0.3 * Proficiency) + (ShapingSkill * 0.5) + MagicPenetration)
        /// + bonusLuck >= (InnateResistance + MagicResistance) * 2
        /// </summary>
        public int MagicPenetration { get; set; } = 1;

        /// <summary>
        /// Any enchantment that affects something
        /// </summary>
        public List<ISpellEffect> Enchantments { get; set; } = new List<ISpellEffect>();

        public MagicManager()
        {
            KnowSpells = new List<SpellBase>();
            KnowEffects = new();
            KnowArea = new();
            KnowDamageTypes = new();
        }

        public static int CalculateSpellDamage(Actor entityStats, SpellBase spellCasted)
        {
            int baseDamage = (int)(spellCasted.Power + spellCasted.SpellLevel
                + entityStats.Mind.Inteligence + (entityStats.Soul.WillPower * 0.5));

            int rngDmg = Dice.Roll($"{spellCasted.SpellLevel}d{baseDamage}");

            return (int)(rngDmg * spellCasted.Proficiency);
        }

        public static bool PenetrateResistance(SpellBase spellCasted, MagiEntity caster, MagiEntity defender,
            int bonusLuck) =>
            (int)((0.3 * spellCasted.Proficiency) + (caster.Magic.ShapingSkill * 0.5)
            + caster.Magic.MagicPenetration) + bonusLuck >= (defender.Magic.InnateMagicResistance
            + defender.Magic.MagicResistance) * 2;

        public SpellBase QuerySpell(string spellId)
        {
            return KnowSpells.Find(i => i.SpellId.Equals(spellId));
        }

        public bool AddToSpellList(SpellBase spell)
        {
            if (KnowSpells.Contains(spell, new SpellComparator()))
            {
                return false;
            }
            KnowSpells.Add(spell);
            int count = spell.Effects.Count;
            for (int i = 0; i < count; i++)
            {
                var sp = spell.Effects[i];
                if (sp is null)
                {
                    GameLoop.WriteToLog($"Spell effect is null! Something went wrong \nSpell Name: {spell.SpellName} \nSpell Id: {spell.SpellId} \nSpell effects: {spell.Effects}");
                    continue;
                }
                if (!KnowEffects.Contains(sp.EffectType))
                    KnowEffects.Add(sp.EffectType);
                if (!KnowArea.Contains(sp.AreaOfEffect))
                    KnowArea.Add(sp.AreaOfEffect);
                if (!KnowDamageTypes.Contains(sp.SpellDamageType))
                    KnowDamageTypes.Add(sp.SpellDamageType);
            }
            return true;
        }
    }

    public class SpellComparator : IEqualityComparer<SpellBase>
    {
        public bool Equals(SpellBase? x, SpellBase? y)
        {
            return x.SpellId.Equals(y.SpellId);
        }

        public int GetHashCode([DisallowNull] SpellBase obj)
        {
            return obj.GetHashCode();
        }
    }
}