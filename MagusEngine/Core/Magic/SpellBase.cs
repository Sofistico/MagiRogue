using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
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
    public class SpellBase : IJsonKey, INamed
    {
        private double proficency;
        private string errorMessage = "Can't cast the spell";

        /// <summary>
        /// The id of the spell, required for quick look up and human redable serialization.
        /// </summary>
        public string Id { get; set; }

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
        public List<ISpellEffect> Effects { get; set; }

        /// <summary>
        /// Spell name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the spell
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// What art of magic the spell is
        /// </summary>
        public ArtMagic MagicArt { get; set; }

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
        public double MagicCost { get; set; }

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

        public List<string>? Keywords { get; set; } = [];
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
        /// <param name="Name">The name of the spell</param>
        /// <param name="magicArt">What school is this spell part of?</param>
        /// <param name="spellRange">The range of the spell</param>
        /// <param name="spellLevel">The level of the spell, going from 1 to 9</param>
        /// <param name="magicCost">The mana cost of the spell, should be more than 0.1</param>
        public SpellBase(string spellId,
            string name,
            ArtMagic magicArt,
            int spellRange,
            string shapingAbility,
            int spellLevel = 1,
            double magicCost = 0.1)
        {
            Id = spellId;
            Name = name;
            MagicArt = magicArt;
            SpellRange = spellRange;
            SpellLevel = spellLevel;
            MagicCost = magicCost;
            Effects = [];
            ShapingAbility = shapingAbility;
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
                bool canCast = false;

                if (stats.Soul.CurrentMana <= MagicCost)
                {
                    errorMessage = "You don't have enough mana to cast the spell!";
                    return canCast;
                }

                canCast = reqShapingWithDiscount <= stats.GetShapingAbility(ShapingAbility);

                if (!canCast)
                {
                    TickProfiency();
                    errorMessage = "Can't cast the spell because you don't have the required shaping skills and/or the proficiency";
                }

                return canCast;
            }

            return false;
        }

        /// <summary>
        /// Single spell targeting
        /// </summary>
        /// <param name="target">The target pos</param>
        /// <param name="caster">the caster</param>
        /// <returns>whetever the cast was successful</returns>
        public bool CastSpell(Point target, Actor caster)
        {
            if (!CanCast(caster.Magic, caster))
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new(errorMessage));
                errorMessage = "Can't cast the spell";

                return false;
            }

            MagiEntity? entity = Find.CurrentMap?.GetEntityAt<MagiEntity>(target);
            caster.Soul.CurrentMana -= (float)MagicCost;

            if (entity is null)
                return false;

            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{caster.Name} casted {Name}"));

            foreach (ISpellEffect effect in Effects)
            {
                if ((entity.CanBeAttacked && (effect.AreaOfEffect is SpellAreaEffect.Self || !entity.Equals(caster)))
                    || effect.TargetsTile)
                {
                    effect.ApplyEffect(target, caster, this);
                }
            }

            TickProfiency();

            return true;
        }

        /// <summary>
        /// Multi spell targetting
        /// </summary>
        /// <param name="target">The list of pos</param>
        /// <param name="caster">the caster</param>
        /// <returns>whetever the cast was successful</returns>
        public bool CastSpell(List<Point> target, Actor caster)
        {
            if (target.Count == 0)
            {
                errorMessage = "Can't cast the spell, there must be an entity to target";
            }
            else if (CanCast(caster.Magic, caster) && target.Count > 0)
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{caster.Name} casted {Name}"));

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
                Id = Id,
                SpellLevel = SpellLevel,
                Name = Name,
                SpellRange = SpellRange,
                MagicArt = MagicArt,
                AffectsTile = AffectsTile,
                Context = Context,
                IgnoresWall = IgnoresWall,
                Keywords = Keywords,
                ShapingAbility = ShapingAbility,
            };
        }

        public SpellBase Copy(double proficiency)
        {
            var copy = Copy();
            copy.Proficiency = proficiency;
            return copy;
        }

        private void TickProfiency() => Proficiency = MathMagi.Round(Proficiency + 0.01);

        public override string ToString()
        {
            return new StringBuilder().Append(Name).Append(": ").Append(SpellLevel)
                .Append(", Range: ").Append(SpellRange)
                .Append(", Proficiency: ").Append(Proficiency).Append(" \r\n")
                .Append("Required shape skills: ").Append(RequiredShapingSkill)
                .AppendLine(MagicArt.ToString()).ToString();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }
    }
}
