using GoRogue;
using MagiRogue.Utils;
using MagiRogue.System.Time;
using MagiRogue.Entities;
using GoRogue.DiceNotation;

namespace MagiRogue.System.Magic
{
    public class SpellBase
    {
        private double proficency;
        private int requiredShapingSkill;

        public string SpellName { get; set; }
        public string Description { get; set; }
        public SpellEffect SpellEffect { get; set; }
        public SpellAreaEffect AreaOfEffect { get; set; }
        public Coord Target { get; set; }
        public DamageType SpellDamageType { get; set; }
        public TimeDefSpan SpellDuration { get; set; }
        public MagicSchool SpellSchool { get; set; }

        /// <summary>
        /// From 1 to 9
        /// </summary>
        public int SpellLevel { get; set; }
        public double ManaCost { get; set; }
        public double Proficiency
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

        public SpellBase(string spellName, SpellEffect spellEffect,
            SpellAreaEffect areaOfEffect, DamageType damageType, TimeDefSpan spellDuration,
            MagicSchool spellScholl,
            int spellPower = 1, double manaCost = 0.1)
        {
            SpellName = spellName;
            SpellEffect = spellEffect;
            AreaOfEffect = areaOfEffect;
            SpellDamageType = damageType;
            SpellDuration = spellDuration;
            SpellLevel = spellPower;
            ManaCost = manaCost;
            SpellSchool = spellScholl;
            requiredShapingSkill = (int)((spellPower * manaCost) * 0.5);
        }

        public void ChangeDamageType(DamageType newType) => SpellDamageType = newType;

        public int CalculateDamage(Stat entityStats, Magic magicStats)
        {
            int baseDamage = (int)(SpellLevel + (entityStats.MindStat * 0.5) + (entityStats.SoulStat * 0.5));

            int rngDmg = Dice.Roll($"{SpellLevel}d{baseDamage}");

            int damageAfterModifiers = (int)(rngDmg * proficency);

            return damageAfterModifiers;
        }

        public bool CanCast(Magic magicSkills, Stat stats)
        {
            requiredShapingSkill /= stats.SoulStat;

            return requiredShapingSkill < magicSkills.ShapingSkills;
        }

        public void SetTarget(Coord target)
        {
            switch (AreaOfEffect)
            {
                case SpellAreaEffect.Target:
                    break;

                case SpellAreaEffect.Point:
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
                    break;

                default:
                    break;
            }
        }
    }

    public enum SpellEffect
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
        Dimension
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