using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagiRogue.GameSys.Magic
{
    /// <summary>
    /// The base class for creating a new Spell, deals with what happens with the spell + defines the
    /// effects that it will have.
    /// </summary>
    [JsonConverter(typeof(SpellJsonConverter))]
    public class SpellBase
    {
        private double proficency;
        private string errorMessage = "Can't cast the spell";

        /// <summary>
        /// The required shaping skill to cast the spell at it's most basic parameters.
        /// </summary>
        public int RequiredShapingSkill
        {
            get => (int)MathMagi.Round(Power * 2 / Proficiency);
        }

        /// <summary>
        /// All the effects that the spell can have
        /// </summary>
        [JsonProperty(Order = 8)]
        public List<ISpellEffect> Effects { get; set; }

        /// <summary>
        /// Spell name
        /// </summary>
        [JsonProperty(Order = -1)]
        public string SpellName { get; set; }

        /// <summary>
        /// Description of the spell
        /// </summary>
        [JsonProperty(Order = 0)]
        public string Description { get; private set; }

        /// <summary>
        /// What school the spell is
        /// </summary>
        [JsonProperty(Order = 6)]
        public MagicSchool SpellSchool { get; set; }

        /// <summary>
        /// The range that the spell can act, can be anything from 0 - self to 999 - map
        /// </summary>
        [JsonProperty(Order = 7)]
        public int SpellRange { get; set; }

        /// <summary>
        /// From 1 to 9
        /// </summary>
        [JsonProperty(Order = 4)]
        public int SpellLevel { get; set; }

        /// <summary>
        /// The total mana cost of the spell, ranging from 0.1 for simple feats of magic to anything beyond
        /// </summary>
        [JsonProperty(Order = 5)]
        public double ManaCost { get; set; }

        /// <summary>
        /// The id of the spell, required for quick look up and human redable serialization.
        /// </summary>
        [JsonProperty(Order = -2)]
        public string SpellId { get; set; }

        /// <summary>
        /// The total proficiency, goes up slowly as you use the spell or train with it in your downtime, makes
        /// it more effective and cost less, goes from 0.0(not learned) to 2.0(double effectiviness),
        /// for newly trained spell shoud be 0.5, see <see cref="MagicManager.ShapingSkill"/> for more details about
        /// the shaping of mana
        /// </summary>
        public double Proficiency
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

        /// <summary>
        /// Normally can be referred as the base damage of a spell, without any kind of spell caster modifier
        /// </summary>
        public int Power
        {
            get => (int)(SpellLevel + ManaCost);
        }

        /// <summary>
        /// Empty constructor, a waste of space
        /// </summary>
        public SpellBase()
        {
        }

        /// <summary>
        /// The spell being created.
        /// </summary>
        /// <param name="spellId">Should be something unique with spaces separated by _</param>
        /// <param name="spellName">The name of the spell</param>
        /// <param name="spellSchool">What school is this spell part of?</param>
        /// <param name="spellRange">The range of the spell</param>
        /// <param name="spellLevel">The level of the spell, going from 1 to 9</param>
        /// <param name="manaCost">The mana cost of the spell, should be more than 0.1</param>
        public SpellBase(string spellId,
            string spellName,
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
            Effects = new List<ISpellEffect>();
        }

        public bool CanCast(MagicManager magicSkills, Actor stats)
        {
            if (magicSkills.KnowSpells.Contains(this))
            {
                int reqShapingWithDiscount;
                try
                {
                    reqShapingWithDiscount = RequiredShapingSkill / stats.Soul.WillPower;
                }
                catch (DivideByZeroException)
                {
                    GameLoop.AddMessageLog("Tried to divide your non existant soul by zero! The universe almost exploded because of you");
                    return false;
                }

                bool canCast = reqShapingWithDiscount <= magicSkills.ShapingSkill;

                if (!canCast)
                {
                    TickProfiency();
                    errorMessage = "Can't cast the spell because you don't have the required shaping skills and/or the proficiency";
                }
                if (stats.Soul.CurrentMana <= ManaCost)
                {
                    errorMessage = "You don't have enough mana to cast the spell!";
                    canCast = false;
                }

                return canCast;
            }

            return false;
        }

        /// <summary>
        /// Single spell targeting
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <returns></returns>
        public bool CastSpell(Point target, Actor caster)
        {
            if (CanCast(caster.Magic, caster) && target != Point.None)
            {
                Entity entity = GameLoop.GetCurrentMap().GetEntityAt<Entity>(target);

                GameLoop.AddMessageLog($"{caster.Name} casted {SpellName}");

                foreach (ISpellEffect effect in Effects)
                {
                    if (entity is not null && (effect.AreaOfEffect is SpellAreaEffect.Self ||
                        !entity.Equals(caster)) && entity.CanBeAttacked || effect.TargetsTile)
                    {
                        effect.ApplyEffect(target, caster, this);
                    }
                }

                caster.Soul.CurrentMana -= (float)ManaCost;
                TickProfiency();

                return true;
            }

            GameLoop.AddMessageLog(errorMessage);
            errorMessage = "Can't cast the spell";

            return false;
        }

        /// <summary>
        /// Multi spell targetting
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <returns></returns>
        public bool CastSpell(List<Point> target, Actor caster)
        {
            if (CanCast(caster.Magic, caster) && target.Count > 0)
            {
                GameLoop.AddMessageLog($"{caster.Name} casted {SpellName}");

                foreach (var pos in target)
                {
                    Entity entity = GameLoop.GetCurrentMap().GetEntityAt<Entity>(pos);
                    foreach (ISpellEffect effect in Effects)
                    {
                        if (entity is not null && entity.CanBeAttacked)
                            effect.ApplyEffect(pos, caster, this);
                    }
                }

                caster.Soul.CurrentMana -= ManaCost;
                TickProfiency();

                return true;
            }
            errorMessage = "Can't cast the spell, there must be an entity to target";
            GameLoop.AddMessageLog(errorMessage);
            errorMessage = "Can't cast the spell";

            return false;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public SpellBase Copy()
        {
            SpellBase copy = new SpellBase()
            {
                Description = this.Description,
                Effects = this.Effects,
                ManaCost = this.ManaCost,
                Proficiency = this.Proficiency,
                SpellId = this.SpellId,
                SpellLevel = this.SpellLevel,
                SpellName = this.SpellName,
                SpellRange = this.SpellRange,
                SpellSchool = this.SpellSchool
            };

            return copy;
        }

        private void TickProfiency() => Proficiency = MathMagi.Round(Proficiency + 0.01);

        public override string ToString()
        {
            string bobBuilder = new StringBuilder().Append(SpellName).Append(": ").Append(SpellLevel)
                .Append($", Range: {SpellRange}")
                .Append($", Proficiency: {Proficiency} \r\n")
                .Append($"Required shape skills: {RequiredShapingSkill}")
                .AppendLine(SpellSchool.ToString()).ToString();

            return bobBuilder;
        }
    }
}