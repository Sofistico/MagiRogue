using MagiRogue.System.Magic;
using MagiRogue.System.Magic.Effects;
using MagiRogue.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MagiRogue.Data.Serialization
{
    public class SpellJsonConverter : JsonConverter<SpellBase>
    {
        public override SpellBase ReadJson(JsonReader reader, Type objectType,
            SpellBase existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject spell = JObject.Load(reader);
            IEnumerable<JToken> listEffectsJson = spell.SelectTokens("$.Effects[*]");
            var effectsList = new List<ISpellEffect>();

            foreach (JToken token in listEffectsJson)
            {
                EffectTypes effect = StringToEnumEffectType((string)token["EffectType"]);
                effectsList.Add(EnumToEffect(effect, token));
            }

            SpellBase createdSpell = new(
                (string)spell["SpellId"],
                (string)spell["SpellName"],
                StringToSchool((string)spell["SpellSchool"]),
                (int)spell["SpellRange"],
                (int)spell["SpellLevel"],
                (double)spell["ManaCost"]);
            createdSpell.SetDescription((string)spell["Description"]);
            createdSpell.Effects = effectsList;
            if (spell.ContainsKey("Proficiency"))
            {
                createdSpell.Proficiency = (double)spell["Proficiency"];
            }

            return createdSpell;
        }

        public override void WriteJson(JsonWriter writer, SpellBase value, JsonSerializer serializer)
        {
            serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer, (SpellTemplate)value);
        }

        /// <summary>
        /// This is used to convert a string to an enum of <see cref="EffectTypes">EffectType</see>
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        private static EffectTypes StringToEnumEffectType(string st)
        {
            return st switch
            {
                "DAMAGE" => EffectTypes.DAMAGE,
                "HASTE" => EffectTypes.HASTE,
                "MAGESIGHT" => EffectTypes.MAGESIGHT,
                "SEVER" => EffectTypes.SEVER,
                "TELEPORT" => EffectTypes.TELEPORT,
                _ => throw new Exception($"It wasn't possible to convert the effect, " +
                    $"please use only the provided effects types.\nEffect used: {st}"),
            };
        }

        private static SpellAreaEffect StringToAreaEffect(string st)
        {
            return st switch
            {
                "Target" => SpellAreaEffect.Target,
                "Self" => SpellAreaEffect.Self,
                "Ball" => SpellAreaEffect.Ball,
                "Beam" => SpellAreaEffect.Beam,
                "Cone" => SpellAreaEffect.Cone,
                "Level" => SpellAreaEffect.Level,
                "World" => SpellAreaEffect.World,
                _ => throw new Exception($"It wasn't possible to understand this spell area, is it a valid area?/n area = {st}")
            };
        }

        private static DamageType StringToDamageType(string st)
        {
            return st switch
            {
                "None" => DamageType.None,
                "Blunt" => DamageType.Blunt,
                "Sharp" => DamageType.Sharp,
                "Point" => DamageType.Point,
                "Force" => DamageType.Force,
                "Fire" => DamageType.Fire,
                "Cold" => DamageType.Cold,
                "Poison" => DamageType.Poison,
                "Acid" => DamageType.Acid,
                "Shock" => DamageType.Shock,
                "Soul" => DamageType.Soul,
                "Mind" => DamageType.Mind,
                _ => throw new Exception($"This isn't a valid damage type, please use a valid one. Value used: {st}")
            };
        }

        /// <summary>
        /// Is used to instantiate and return any number of possible effects
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="jToken"></param>
        /// <returns></returns>
        private static ISpellEffect EnumToEffect(EffectTypes effect, JToken jToken)
        {
            return effect switch
            {
                EffectTypes.DAMAGE => new DamageEffect
                    ((int)jToken["BaseDamage"], StringToAreaEffect((string)jToken["AreaOfEffect"]),
                    StringToDamageType((string)jToken["SpellDamageType"]),
                    (bool)jToken["CanMiss"], (bool)jToken["IsHealing"], (int)jToken["Radius"],
                    isResistable: (bool)jToken["IsResistable"]),
                EffectTypes.HASTE => new HasteEffect(StringToAreaEffect((string)jToken["AreaOfEffect"]),
                    (int)jToken["HastePower"],
                    (int)jToken["Duration"],
                    StringToDamageType((string)jToken["SpellDamageType"])),
                EffectTypes.MAGESIGHT => new MageSightEffect((int)jToken["Duration"]),
                EffectTypes.SEVER => new SeverEffect(StringToAreaEffect((string)jToken["AreaOfEffect"]),
                    StringToDamageType((string)jToken["SpellDamageType"]),
                    (int)jToken["Radius"], (int)jToken["BaseDamage"]),
                EffectTypes.TELEPORT => new TeleportEffect(StringToAreaEffect((string)jToken["AreaOfEffect"]),
                    StringToDamageType((string)jToken["SpellDamageType"]),
                    (int)jToken["Radius"]),
                _ => null,
            };
        }

        private static MagicSchool StringToSchool(string st)
        {
            return st switch
            {
                "Projection" => MagicSchool.Projection, // 1
                "Negation" => MagicSchool.Negation, // 2
                "Animation" => MagicSchool.Animation, // 3
                "Divination" => MagicSchool.Divination, // 4
                "Alteration" => MagicSchool.Alteration, // 5
                "Wards" => MagicSchool.Wards, // 6
                "Dimensionalism" => MagicSchool.Dimensionalism, // 7
                "Conjuration" => MagicSchool.Conjuration, // 8
                "Illuminism" => MagicSchool.Illuminism, // 9
                "MedicalMagic" => MagicSchool.MedicalMagic, // 10
                "MindMagic" => MagicSchool.MindMagic, // 11
                "SoulMagic" => MagicSchool.SoulMagic, // 12
                "BloodMagic" => MagicSchool.BloodMagic, // 13
                _ => throw new Exception($"Are you sure the school is correct? it must be a valid one./n School used: {st}")
            };
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

        public double? Proficiency { get; set; }

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
                spellTemplate.ManaCost)
            {
                Proficiency =
                spellTemplate.Proficiency is null ? 0 : (double)spellTemplate.Proficiency
            };
            foreach (var item in spellTemplate.Effects)
            {
                spell.Effects.Add(item);
            }
            spell.SetDescription(spellTemplate.Description);
            return spell;
        }

        public static implicit operator SpellTemplate(SpellBase spell)
        {
            SpellTemplate template =
                new SpellTemplate(spell.SpellLevel, spell.Effects, spell.SpellName,
                spell.Description, spell.SpellSchool, spell.SpellRange, spell.ManaCost, spell.SpellId)
                {
                    Proficiency = spell.Proficiency
                };

            return template;
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