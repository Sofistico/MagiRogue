using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arquimedes.Enumerators;
using Arquimedes.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Components.EntityComponents.Projectiles;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic.Interfaces;
using MagusEngine.Core.MapStuff;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Utils;
using Newtonsoft.Json;
using SadRogue.Primitives;

namespace MagusEngine.Core.Magic
{
    /// <summary>
    /// The base class for creating a new Spell, deals with what happens with the spell + defines
    /// the effects that it will have.
    /// </summary>
    [JsonConverter(typeof(SpellJsonConverter))]
    public sealed class Spell : IJsonKey, INamed, ISpell
    {
        private const double _maxSpellProficiency = 10.0;
        [JsonProperty(PropertyName = "Proficiency")]
        private double _proficency;
        [JsonIgnore]
        private string _errorMessage = "Can't cast the spell";
        [JsonIgnore]
        private Lazy<bool> _ignoresWall = null!;

        /// <summary>
        /// The id of the spell, required for quick look up and human redable serialization.
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// The required shaping skill to cast the spell at it's most basic parameters.
        /// </summary>
        public int RequiredShapingSkill => (int)MathMagi.Round(Power * 6 / Proficiency);

        /// <summary>
        /// All the effects that the spell can have
        /// </summary>
        public List<ISpellEffect> Effects { get; set; } = [];

        /// <summary>
        /// Spell name
        /// </summary>
        public string Name { get; set; } = null!;

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
        /// The total cost of the spell, ranging from 0.1 for simple feats of magic to anything beyond, should be scalated to the specified cost
        /// </summary>
        public double MagicCost { get; set; }

        public SpellCostType CostType { get; set; } = SpellCostType.Mana;
        public SpellManifestation Manifestation { get; set; }

        [JsonIgnore]
        /// <summary>
        /// The total proficiency, goes up slowly as you use the spell or train with it in your
        /// downtime, makes it more effective and cost less, goes from 0.0(not learned) to
        /// maxProficiency(x effectiviness, works as a percentage multiplier), for newly trained spell shoud be 0.01, see
        /// <see cref="Magic.ShapingSkill"/> for more details about the shaping of mana
        /// </summary>
        public double Proficiency
        {
            get
            {
                if (_proficency == 0)
                    _proficency = 0.01;

                return _proficency;
            }

            set
            {
                if (value >= _maxSpellProficiency)
                    _proficency = _maxSpellProficiency;
                else if (value <= 0.0)
                    _proficency = 0.0;
                else
                    _proficency = value;
            }
        }

        /// <summary>
        /// Normally can be referred as the base damage of a spell, without any kind of spell caster modifier
        /// </summary>
        public int Power => (int)((SpellLevel + MagicCost) * Proficiency);

        public List<string>? Keywords { get; set; } = [];
        public List<SpellContext>? Context { get; set; }
        public bool AffectsTile => Effects.Any(static i => i.TargetsTile);
        public string ShapingAbility { get; set; } = null!;
        public string Fore { get; set; } = "{caster}";
        public string Back { get; set; } = "Black";
        public char[] Glyphs { get; set; } = ['*'];
        public List<IMagicStep> Steps { get; set; } = [];

        public int Velocity { get; set; } = 10; // is in ticks

        public bool IgnoresWall => _ignoresWall.Value;

        /// <summary>
        /// Empty constructor, a waste of space
        /// </summary>
        [JsonConstructor]
        public Spell()
        {
        }

        /// <summary>
        /// The spell being created.
        /// </summary>
        /// <param name="spellId">Should be something unique with spaces separated by _</param>
        /// <param name="name">The name of the spell</param>
        /// <param name="magicArt">What school is this spell part of?</param>
        /// <param name="spellRange">The range of the spell</param>
        /// <param name="spellLevel">The level of the spell, going from 1 to 9</param>
        /// <param name="magicCost">The mana cost of the spell, should be more than 0.1</param>
        public Spell(string spellId,
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
            _ignoresWall = new(() => Effects.Any(i => i.IgnoresWall));
        }

        public bool CanCast(Actor actor, bool tickProficiency = true)
        {
            var magicSkills = actor.GetComponent<Magic>();
            if (magicSkills.KnowSpells.Contains(this))
            {
                int reqShapingWithDiscount;
                try
                {
                    reqShapingWithDiscount = (int)(RequiredShapingSkill / (double)((actor.Soul.WillPower * 0.3) + 1));
                }
                catch (DivideByZeroException)
                {
                    MagiLog.Log("Tried to divide your non existant will by zero! The universe almost exploded because of you");
                    return false;
                }
                bool canCast = false;

                if (actor.Soul.CurrentMana <= MagicCost)
                {
                    _errorMessage = "You don't have enough mana to cast the spell!";
                    return canCast;
                }

                canCast = reqShapingWithDiscount <= actor.GetShapingAbility(ShapingAbility);

                if (!canCast && tickProficiency)
                {
                    TickProfiency();
                    _errorMessage = "Can't cast the spell because you don't have the required shaping skills and/or the proficiency";
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
        public bool CastSpell(Point target, Actor caster, MagiEntity? selectedEntity = null)
        {
            if (!CanCast(caster))
            {
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new(_errorMessage));
                _errorMessage = "Can't cast the spell";

                return false;
            }

            selectedEntity ??= Find.CurrentMap?.GetEntityAt<MagiEntity>(target);

            if (selectedEntity is null && !AffectsTile)
            {
                if (Manifestation == SpellManifestation.Instant)
                    Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("Can't cast the spell, there must be an entity to target"));

                return false;
            }

            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new($"{caster.Name} casted {Name}"));

            ApplyEffects(target, caster, selectedEntity);

            HandleCost(caster);

            TickProfiency();

            return true;
        }

        private void HandleCost(Actor caster)
        {
            switch (CostType)
            {
                case SpellCostType.Mana:
                    caster.Soul.CurrentMana -= MagicCost;
                    break;

                case SpellCostType.Stamina:
                    caster.Body.Stamina -= MagicCost;
                    break;

                case SpellCostType.Blood:
                    caster.ActorAnatomy.BloodCount -= MagicCost;
                    break;

                case SpellCostType.Soul:
                    caster.Soul.SenseOfSelf -= (int)MagicCost;
                    break;
            }
        }

        private void ApplyEffects(Point target, Actor caster, MagiEntity? entity = null)
        {
            foreach (ISpellEffect effect in Effects)
            {
                switch (effect.AreaOfEffect)
                {
                    case SpellAreaEffect.Self:
                        effect.ApplyEffect(caster.Position, caster, this);
                        continue;
                    case SpellAreaEffect.Target:
                        entity ??= Find.CurrentMap?.GetEntityAt<MagiEntity>(target);
                        if (!CanTarget(caster, entity, effect))
                        {
                            continue;
                        }
                        effect.ApplyEffect(target, caster, this);
                        break;

                    default:
                        foreach (var pos in TargetHelper.SpellAreaHelper(this, caster.Position, target)!)
                        {
                            MagiEntity? entityAt = Find.CurrentMap?.GetEntityAt<MagiEntity>(pos);
                            if (!CanTarget(caster, entityAt, effect))
                                continue;

                            effect.ApplyEffect(pos, caster, this);
                        }
                        break;
                }
            }
        }

        private static bool CanTarget(Actor caster, MagiEntity? entity, ISpellEffect effect)
        {
            return (entity?.CanInteract == true && !entity.Equals(caster)) || effect.TargetsTile;
        }

        public Spell Copy()
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
                Context = Context,
                Keywords = Keywords,
                ShapingAbility = ShapingAbility,
                Back = Back,
                Fore = Fore,
                CostType = CostType,
                Manifestation = Manifestation,
                Glyphs = Glyphs,
                Steps = Steps,
                Velocity = Velocity,
                _ignoresWall = _ignoresWall,
            };
        }

        public Spell Copy(double proficiency)
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
                .Append(", Proficiency: ").Append(Proficiency).Append(" \n")
                .Append("Required shape skills: ").Append(RequiredShapingSkill)
                .AppendLine(MagicArt.ToString()).ToString();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is not null && (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && string.Equals(Id, ((Spell)obj).Id, StringComparison.Ordinal)));
        }

        public SpellEntity GetSpellEntity(MagiEntity caster, Direction dir, Point pos)
        {
            string foreground = Fore.Equals("{caster}", StringComparison.Ordinal) ? caster.GetComponent<Magic>().MagicColor : Fore;
            var entity = new SpellEntity(Id, foreground, Back, dir.TranslateDirToGlyph(Glyphs), this, pos, caster)
            {
                Name = Name,
            };
            return entity;
        }
    }
}
