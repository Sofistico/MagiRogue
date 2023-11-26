using Arquimedes.Enumerators;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagusEngine.Core.Magic
{
    /// <summary>
    /// The base class for creating a new Spell, deals with what happens with the spell + defines
    /// the effects that it will have.
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
            get => (int)MathMagi.Round(Power * 6 / Proficiency);
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
        public string? Description { get; set; }

        /// <summary>
        /// What art of magic the spell is
        /// </summary>
        [JsonProperty(Order = 6)]
        public ArtMagic MagicArt { get; set; }

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
        public double MagicCost { get; set; }

        /// <summary>
        /// The id of the spell, required for quick look up and human redable serialization.
        /// </summary>
        [JsonProperty(Order = -2)]
        public string SpellId { get; set; }

        /// <summary>
        /// The total proficiency, goes up slowly as you use the spell or train with it in your
        /// downtime, makes it more effective and cost less, goes from 0.0(not learned) to
        /// 2.0(double effectiviness), for newly trained spell shoud be 0.5, see
        /// <see cref="MagicManager.ShapingSkill"/> for more details about the shaping of mana
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
            get => (int)(SpellLevel + MagicCost);
        }

        public List<string>? Keywords { get; set; } = new();
        public List<SpellContext>? Context { get; set; }
        public bool IgnoresWall { get; set; }
        public bool AffectsTile { get; set; }
        public string ShapingAbility { get; set; }

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
        /// <param name="magicArt">What school is this spell part of?</param>
        /// <param name="spellRange">The range of the spell</param>
        /// <param name="spellLevel">The level of the spell, going from 1 to 9</param>
        /// <param name="MagicCost">The mana cost of the spell, should be more than 0.1</param>
        public SpellBase(string spellId,
            string spellName,
            ArtMagic magicArt,
            int spellRange,
            int spellLevel = 1,
            double MagicCost = 0.1)
        {
            SpellId = spellId;
            SpellName = spellName;
            MagicArt = magicArt;
            SpellRange = spellRange;
            SpellLevel = spellLevel;
            MagicCost = MagicCost;
            Effects = [];
        }

        public bool CanCast(MagicManager magicSkills, Actor stats)
        {
            if (magicSkills.KnowSpells.Contains(this))
            {
                int reqShapingWithDiscount;
                try
                {
                    reqShapingWithDiscount = (int)(RequiredShapingSkill / (double)((stats.Soul.WillPower * 0.3) + 1));
                }
                catch (DivideByZeroException)
                {
                    Locator.GetService<MagiLog>().Log("Tried to divide your non existant will by zero! The universe almost exploded because of you");
                    return false;
                }

                bool canCast = reqShapingWithDiscount <= stats.GetShapingAbility(ShapingAbility);

                if (!canCast)
                {
                    TickProfiency();
                    errorMessage = "Can't cast the spell because you don't have the required shaping skills and/or the proficiency";
                }
                if (stats.Soul.CurrentMana <= MagicCost)
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
                MagiEntity entity = Find.CurrentMap.GetEntityAt<MagiEntity>(target);

                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{caster.Name} casted {SpellName}"));

                foreach (ISpellEffect effect in Effects)
                {
                    if ((entity is not null && (effect.AreaOfEffect is SpellAreaEffect.Self ||
                        !entity.Equals(caster)) && entity.CanBeAttacked) || effect.TargetsTile)
                    {
                        effect.ApplyEffect(target, caster, this);
                    }
                }

                caster.Soul.CurrentMana -= (float)MagicCost;
                TickProfiency();

                return true;
            }

            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new(errorMessage));
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
            if (target.Count == 0)
            {
                errorMessage = "Can't cast the spell, there must be an entity to target";
            }
            else if (CanCast(caster.Magic, caster) && target.Count > 0)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{caster.Name} casted {SpellName}"));

                foreach (var pos in target)
                {
                    MagiEntity entity = Find.CurrentMap.GetEntityAt<MagiEntity>(pos);
                    foreach (ISpellEffect effect in Effects)
                    {
                        effect.ApplyEffect(pos, caster, this);
                    }
                }

                caster.Soul.CurrentMana -= MagicCost;
                TickProfiency();

                return true;
            }
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new(errorMessage));

            return false;
        }

        public SpellBase Copy()
        {
            return new()
            {
                Description = Description,
                Effects = Effects,
                MagicCost = MagicCost,
                Proficiency = Proficiency,
                SpellId = SpellId,
                SpellLevel = SpellLevel,
                SpellName = SpellName,
                SpellRange = SpellRange,
                MagicArt = MagicArt,
                AffectsTile = AffectsTile,
                Context = Context,
                IgnoresWall = IgnoresWall,
                Keywords = Keywords,
            };
        }

        private void TickProfiency() => Proficiency = MathMagi.Round(Proficiency + 0.01);

        public override string ToString()
        {
            return new StringBuilder().Append(SpellName).Append(": ").Append(SpellLevel)
                .Append(", Range: ").Append(SpellRange)
                .Append(", Proficiency: ").Append(Proficiency).Append(" \r\n")
                .Append("Required shape skills: ").Append(RequiredShapingSkill)
                .AppendLine(MagicArt.ToString()).ToString();
        }

        public override int GetHashCode()
        {
            return SpellId.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }
    }
}
