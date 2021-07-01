using GoRogue;
using MagiRogue.Utils;
using MagiRogue.System.Time;
using MagiRogue.Entities;
using SadRogue.Primitives;
using GoRogue.DiceNotation;
using System;
using System.Text;
using System.Collections.Generic;

namespace MagiRogue.System.Magic
{
    public class SpellBase
    {
        private double proficency;

        private int requiredShapingSkill;

        public List<ISpellEffect> Effects { get; set; }

        public string SpellName { get; set; }

        public string Description { get; private set; }

        public Point Target { get; set; }

        public MagicSchool SpellSchool { get; set; }

        public int SpellRange { get; set; }

        /// <summary>
        /// From 1 to 9
        /// </summary>
        public int SpellLevel { get; set; }

        public double ManaCost { get; set; }

        public string SpellId { get; set; }

        public double Proficency
        {
            get
            {
                if (proficency == 0)
                    proficency = 0.5;

                return proficency;
            }

            set
            {
                if (value >= 2.0)
                    proficency = 2.0;
                if (value <= 0.0)
                    proficency = 0.0;

                proficency = value;
            }
        } // multiplier, going from 0.0 to 2.0

        public SpellBase()
        {
        }

        public SpellBase(string spellId,
            string spellName,
            List<ISpellEffect> effects,
            MagicSchool spellSchool,
            int spellRange,
            int spellLevel = 1,
            double manaCost = 0.1f)
        {
            SpellId = spellId;
            SpellName = spellName;
            SpellSchool = spellSchool;
            SpellRange = spellRange;
            SpellLevel = spellLevel;
            ManaCost = manaCost;
            Effects = effects;
            requiredShapingSkill = (int)((spellLevel * manaCost) * 0.5);
        }

        public bool CanCast(Magic magicSkills, Stat stats)
        {
            if (magicSkills.KnowSpells.Contains(this) && stats.PersonalMana >= ManaCost)
            {
                requiredShapingSkill /= stats.SoulStat;

                return requiredShapingSkill < magicSkills.ShapingSkills;
            }
            return false;
        }

        public bool CastSpell(Point target, Actor caster)
        {
            if (CanCast(caster.Magic, caster.Stats) && target != Point.None)
            {
                Target = target;
                GameLoop.UIManager.MessageLog.Add($"{caster.Name} casted {SpellName}");
                foreach (ISpellEffect effect in Effects)
                {
                    effect.ApplyEffect(target, caster.Stats);
                }

                caster.Stats.PersonalMana -= (float)ManaCost;

                return true;
            }

            GameLoop.UIManager.MessageLog.Add("Couldn't cast the spell");

            return false;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public override string ToString()
        {
            string bobBuilder = new StringBuilder().Append(SpellName).Append(": ").Append(SpellLevel)
                .Append($", Range: {SpellRange}")
                .AppendLine(SpellSchool.ToString())
                .AppendLine(Description).ToString();

            return bobBuilder;
        }
    }

    public enum SpellTypeEnum
    {
        Damage, // Can be the same as healing
        Teleport,
        Summon,
        Divination,
        Dispel,
        Haste,
        Animation,
        Ward,
        Transformation,
        Illusion,
        Control,
        Soul,
        Telekinesis,
        Ritual,
        Dimension,
        Buff,
        Debuff
    }

    public enum SpellAreaEffect
    {
        Target,
        Ball,
        Shape,
        Beam,
        Level,
        World
    }
}