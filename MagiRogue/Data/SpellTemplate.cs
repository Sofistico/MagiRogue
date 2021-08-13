using MagiRogue.System.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MagiRogue.System.Magic.Effects;
using Newtonsoft.Json.Converters;
using MagiRogue.Utils;
using SadRogue.Primitives;
using MagiRogue.Entities;

namespace MagiRogue.Data
{
    public class SpellJsonConverter : JsonConverter<SpellBase>
    {
        public override SpellBase ReadJson(JsonReader reader, Type objectType,
            SpellBase existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            SpellTemplate spell = serializer.Deserialize<SpellTemplate>(reader);
            var effectsList = new List<ISpellEffect>();

            foreach (ISpellEffect effect in spell.Effects)
            {
                switch (effect.EffectType)
                {
                    case EffectTypes.DAMAGE:
                        var dmgEffect = (IDamageSpellEffect)effect;
                        effectsList.Add(new DamageEffect
                            (dmgEffect.BaseDamage, dmgEffect.AreaOfEffect, dmgEffect.SpellDamageType,
                            dmgEffect.CanMiss, dmgEffect.IsHealing, dmgEffect.Radius));
                        break;

                    case EffectTypes.HASTE:
                        var haste = (IHasteEffect)effect;
                        effectsList.Add(new HasteEffect(haste.AreaOfEffect, haste.HastePower,
                            haste.Duration, haste.SpellDamageType));
                        break;

                    case EffectTypes.MAGESIGHT:
                        var timed = (ITimedEffect)effect;
                        effectsList.Add(new MageSightEffect(timed.Duration));
                        break;

                    case EffectTypes.SEVER:
                        effectsList.Add(new SeverEffect(effect.AreaOfEffect, effect.SpellDamageType,
                            effect.Radius, effect.BaseDamage));
                        break;

                    case EffectTypes.TELEPORT:
                        effectsList.Add
                            (new TeleportEffect(effect.AreaOfEffect, effect.SpellDamageType, effect.Radius));
                        break;

                    default:
                        break;
                }
            }

            spell.Effects = effectsList;

            return spell;
        }

        public override void WriteJson(JsonWriter writer, SpellBase value, JsonSerializer serializer)
        {
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            serializer.Serialize(writer, (SpellTemplate)value);
        }
    }

    /// <summary>
    /// This class will deal with the serialization of the spells and it's effects, will use a tag like CDDA
    /// does to determine the effects
    /// </summary>
    public class SpellTemplate
    {
        public int SpellLevel { get; set; }

        public List<ISpellEffect> Effects { get; set; }

        public string SpellName { get; set; }

        public string Description { get; set; }

        public MagicSchool SpellSchool { get; set; }

        public int SpellRange { get; set; }

        public double ManaCost { get; set; }
        public string SpellId { get; set; }

        public SpellTemplate(int spellLevel, List<ISpellEffect> effects, string spellName, string description,
            MagicSchool spellSchool, int spellRange, double manaCost, string spellId)
        {
            SpellLevel = spellLevel;
            Effects = effects;
            SpellName = spellName;
            Description = description;
            SpellSchool = spellSchool;
            SpellRange = spellRange;
            ManaCost = manaCost;
            SpellId = spellId;
        }

        public static implicit operator SpellBase(SpellTemplate spellTemplate)
        {
            SpellBase spell = new SpellBase(spellTemplate.SpellId, spellTemplate.SpellName,
                spellTemplate.SpellSchool, spellTemplate.SpellRange, spellTemplate.SpellLevel,
                spellTemplate.ManaCost);
            foreach (var item in spellTemplate.Effects)
            {
                spell.Effects.Add(item);
            }
            return spell;
        }

        public static implicit operator SpellTemplate(SpellBase spell)
        {
            SpellTemplate template =
                new SpellTemplate(spell.SpellLevel, spell.Effects, spell.SpellName,
                spell.Description, spell.SpellSchool, spell.SpellRange, spell.ManaCost, spell.SpellId);

            return template;
        }
    }

    public class EffectTemplate : ISpellEffect
    {
        public SpellAreaEffect AreaOfEffect { get; set; }
        public DamageType SpellDamageType { get; set; }
        public int Radius { get; set; }
        public bool TargetsTile { get; set; }
        public int BaseDamage { get; set; }
        public EffectTypes EffectType { get; set; }

        public EffectTemplate(ISpellEffect effect)
        {
            AreaOfEffect = effect.AreaOfEffect;
            SpellDamageType = effect.SpellDamageType;
            Radius = effect.Radius;
            TargetsTile = effect.TargetsTile;
            BaseDamage = effect.BaseDamage;
            EffectType = effect.EffectType;
        }

        public void ApplyEffect(Point target, Actor caster, SpellBase spellCasted)
        {
            throw new Exception("Ops... this shouldn't have happened, tried to acess an EffectTemplate");
        }
    }

    public enum EffectTypes
    {
        DAMAGE,
        HASTE,
        MAGESIGHT,
        SEVER,
        TELEPORT
    }
}