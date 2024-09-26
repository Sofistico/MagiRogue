using Arquimedes.Enumerators;
using GoRogue.DiceNotation;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Services;
using MagusEngine.Utils.Extensions;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MagusEngine.Core.Magic
{
    /// <summary>
    /// The class that is the manager of the magic system to an entity
    /// </summary>
    public class Magic : IMagic
    {
        public const string Tag = "magic_manager";

        // Create a magic inspired by Mother of learning
        public List<Spell> KnowSpells { get; set; }
        public List<EffectType> KnowEffects { get; set; }
        public List<SpellAreaEffect> KnowArea { get; set; }
        public List<DamageTypes> KnowDamageTypes { get; set; }

        /// <summary>
        /// Any enchantment that affects something
        /// </summary>
        public List<ISpellEffect> Enchantments { get; set; }
        public string MagicColor { get; set; }

        public Magic()
        {
            KnowSpells = [];
            KnowEffects = [];
            KnowArea = [];
            KnowDamageTypes = [];
            Enchantments = [];
            MagicColor = ColorExtensions2.ColorMappings.GetRandomItemFromCollection().Key;
        }

        public static int CalculateSpellDamage(Actor entityStats, Spell spellCasted)
        {
            int baseDamage = (int)(spellCasted.Power + spellCasted.SpellLevel
                + entityStats.Mind.Inteligence + (entityStats.Soul.WillPower * 0.5));

            int rngDmg = Dice.Roll($"{spellCasted.SpellLevel}d{baseDamage}");
            return (int)(rngDmg * spellCasted.Proficiency);
        }

        public static bool PenetrateResistance(Spell spellCasted, MagiEntity caster, MagiEntity defender, int bonusLuck) =>
            (int)((0.3 * spellCasted.Proficiency) + (caster.GetShapingAbility(spellCasted.ShapingAbility) * 0.5)
            + caster.GetPenetration()) + bonusLuck >= defender.GetMagicResistance() * 2;

        public Spell? QuerySpell(string spellId)
        {
            return KnowSpells.Find(i => i.Id.Equals(spellId));
        }

        public bool AddToSpellList(Spell spell)
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
                        .Log($"Spell effect is null! Something went wrong \nSpell Name: {spell.Name} \nSpell Id: {spell.Id} \nSpell effects: {spell.Effects}");
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

        public bool AddToSpellList(IEnumerable<Spell> spells)
        {
            foreach (var spell in spells)
            {
                AddToSpellList(spell);
            }
            return KnowSpells.Count > 0;
        }

        public string GetMagicShapingAbility() => "Mana Shaping";
    }

    public class SpellComparator : IEqualityComparer<Spell>
    {
        public bool Equals(Spell? x, Spell? y)
        {
            return x!.Id.Equals(y!.Id);
        }

        public int GetHashCode([DisallowNull] Spell obj)
        {
            return obj.GetHashCode();
        }
    }
}
