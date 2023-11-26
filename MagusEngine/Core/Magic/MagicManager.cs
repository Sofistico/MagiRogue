﻿using Arquimedes.Enumerators;
using GoRogue.DiceNotation;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Services;
using MagusEngine.Utils.Extensions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MagusEngine.Core.Magic
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
        /// Any enchantment that affects something
        /// </summary>
        public List<ISpellEffect> Enchantments { get; set; }

        public MagicManager()
        {
            KnowSpells = [];
            KnowEffects = [];
            KnowArea = [];
            KnowDamageTypes = [];
            Enchantments = [];
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
            (int)((0.3 * spellCasted.Proficiency) + (caster.GetShapingAbility(spellCasted.ShapingAbility) * 0.5)
            + caster.GetPenetration()) + bonusLuck >= defender.GetMagicResistance() * 2;

        public SpellBase? QuerySpell(string spellId)
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
                    Locator.GetService<MagiLog>()
                        .Log($"Spell effect is null! Something went wrong \nSpell Name: {spell.SpellName} \nSpell Id: {spell.SpellId} \nSpell effects: {spell.Effects}");
                    continue;
                }
                if (!KnowEffects.Contains(sp.EffectType))
                    KnowEffects.Add(sp.EffectType);
                if (!KnowArea.Contains(sp.AreaOfEffect))
                    KnowArea.Add(sp.AreaOfEffect);
                if (!sp.SpellDamageTypeId.IsNullOrEmpty() && !KnowDamageTypes.Contains(sp.GetDamageType()!.Type))
                    KnowDamageTypes.Add(sp.GetDamageType()!.Type);
            }
            return true;
        }
    }

    public class SpellComparator : IEqualityComparer<SpellBase>
    {
        public bool Equals(SpellBase? x, SpellBase? y)
        {
            return x!.SpellId.Equals(y!.SpellId);
        }

        public int GetHashCode([DisallowNull] SpellBase obj)
        {
            return obj.GetHashCode();
        }
    }
}
