using MagiRogue.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagiRogue.System.Magic
{
    public class SpellBase
    {
        private double proficency;

        /// <summary>
        /// The required shaping skill to cast the spell at it's most basic parameters.
        /// </summary>
        public int RequiredShapingSkill
        {
            get => (int)((SpellLevel + ManaCost) * 2 / Proficency);
        }

        /// <summary>
        /// All the effects that the spell can have
        /// </summary>
        public List<ISpellEffect> Effects { get; set; }

        /// <summary>
        /// Spell name
        /// </summary>
        public string SpellName { get; set; }

        /// <summary>
        /// Description of the spell
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The target of the spell
        /// </summary>
        public Point Target { get; set; }

        /// <summary>
        /// What school the spell is
        /// </summary>
        public MagicSchool SpellSchool { get; set; }

        /// <summary>
        /// The range that the spell can act, can be anything from 0 - self to 999 - map
        /// </summary>
        public int SpellRange { get; set; }

        /// <summary>
        /// From 1 to 9
        /// </summary>
        public int SpellLevel { get; set; }

        /// <summary>
        /// The total mana cost of the spell, ranging from 0.1 for simple feats of magic to anything beyond
        /// </summary>
        public double ManaCost { get; set; }

        /// <summary>
        /// The id of the spell, required for quick look up and human redable serialization.
        /// </summary>
        public string SpellId { get; set; }

        /// <summary>
        /// The total proficiency, goes up slowly as you use the spell or train with it in your downtime, makes
        /// it more effective and cost less, goes from 0.0(not learned) to 2.0(double effectiviness),
        /// for newly trained spell shoud be 0.5, see <see cref="Magic.ShapingSkills"/> for more details about
        /// the shaping of mana
        /// </summary>
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

        /// <summary>
        /// The spell being created.
        /// </summary>
        /// <param name="spellId">Should be something unique with spaces separated by _</param>
        /// <param name="spellName">The name of the spell</param>
        /// <param name="effects">All the effects that the spell will have</param>
        /// <param name="spellSchool">What school is this spell part of?</param>
        /// <param name="spellRange">The range of the spell</param>
        /// <param name="spellLevel">The level of the spell, going from 1 to 9</param>
        /// <param name="manaCost">The mana cost of the spell, should be more than 0.1</param>
        public SpellBase(string spellId,
            string spellName,
            List<ISpellEffect> effects,
            MagicSchool spellSchool,
            int spellRange,
            int spellLevel = 1,
            float manaCost = 0.1f)
        {
            SpellId = spellId;
            SpellName = spellName;
            SpellSchool = spellSchool;
            SpellRange = spellRange;
            SpellLevel = spellLevel;
            ManaCost = manaCost;
            Effects = effects;
        }

        public bool CanCast(Magic magicSkills, Stat stats)
        {
            if (magicSkills.KnowSpells.Contains(this) && stats.PersonalMana >= ManaCost)
            {
                int reqShapingWithDiscount = RequiredShapingSkill / stats.SoulStat;

                return reqShapingWithDiscount < magicSkills.ShapingSkills;
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
                Proficency = Math.Round(Proficency += 0.01, 2);

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
                .Append($", Range: {SpellRange} \n")
                .AppendLine(SpellSchool.ToString()).ToString();

            return bobBuilder;
        }
    }

    public enum SpellAreaEffect
    {
        Self,
        Target,
        Ball,
        Shape,
        Beam,
        Level,
        World
    }
}