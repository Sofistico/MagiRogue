using GoRogue;
using MagiRogue.Utils;
using MagiRogue.System.Time;
using MagiRogue.Entities;
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

        public List<SpellEffects> Effects { get; set; }
        public string SpellName { get; set; }
        public string Description { get; set; }
        public Coord Target { get; set; }
        public MagicSchool SpellSchool { get; set; }
        public int SpellRange { get; set; }
        /// <summary>
        /// From 1 to 9
        /// </summary>
        public int SpellLevel { get; set; }
        public double ManaCost { get; set; }
        public double Proficency
        {
            get
            {
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

        public SpellBase(string spellName,
            List<SpellEffects> effects,
            MagicSchool spellSchool,
            int spellRange,
            int spellLevel = 1,
            double manaCost = 0.1)
        {
            SpellName = spellName;
            SpellSchool = spellSchool;
            SpellRange = spellRange;
            SpellLevel = spellLevel;
            ManaCost = manaCost;
            Effects = effects;
            requiredShapingSkill = (int)((spellLevel * manaCost) * 0.5);
        }

        public int CalculateDamage(Stat entityStats)
        {
            int baseDamage = (int)(SpellLevel + (entityStats.MindStat * 0.5) + (entityStats.SoulStat * 0.5));

            int rngDmg = Dice.Roll($"{SpellLevel}d{baseDamage}");

            int damageAfterModifiers = (int)(rngDmg * Proficency);

            return damageAfterModifiers;
        }

        public bool CanCast(Magic magicSkills, Stat stats)
        {
            if (magicSkills.KnowSpells.Contains(this))
            {
                requiredShapingSkill /= stats.SoulStat;

                return requiredShapingSkill < magicSkills.ShapingSkills;
            }
            return false;
        }

        public void CastSpell(Coord target, Actor caster)
        {
            if (CanCast(caster.Magic, caster.Stats))
            {
                Target = target;
                foreach (SpellEffects effect in Effects)
                {
                    effect.DoEffect(target);
                }
            }
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public override string ToString()
        {
            string bobBuilder = new StringBuilder().Append(SpellName).Append(": ")
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
        Point,
        Ball,
        Shape,
        Beam,
        Level,
        World
    }
}