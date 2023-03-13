using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Magic;
using MagiRogue.GameSys.Magic.Effects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Data.Serialization.EntitySerialization
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
                EffectType effect = StringToEnumEffectType((string)token["EffectType"]);
                var eff = EnumToEffect(effect, token);
                if (token["ConeCircleSpan"] != null)
                    eff.ConeCircleSpan = (double)token["ConeCircleSpan"];
                effectsList.Add(eff);
            }

            SpellBase createdSpell = new(
                (string)spell["SpellId"],
                (string)spell["SpellName"],
                StringToSchool((string)spell["MagicArt"]),
                (int)spell["SpellRange"],
                (int)spell["SpellLevel"],
                (double)spell["ManaCost"]);
            createdSpell.SetDescription((string)spell["Description"]);
            createdSpell.Effects = effectsList;
            if (spell.ContainsKey("Proficiency"))
            {
                createdSpell.Proficiency = (double)spell["Proficiency"];
            }
            if (spell.ContainsKey("Keywords"))
            {
                createdSpell.Keywords = JsonConvert.DeserializeObject<List<string>>(spell["Keywords"].ToString())!;
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
        /// This is used to convert a string to an enum of <see cref="EffectType">EffectType</see>
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        private static EffectType StringToEnumEffectType(string st)
        {
            return st switch
            {
                "DAMAGE" => EffectType.DAMAGE,
                "HASTE" => EffectType.HASTE,
                "MAGESIGHT" => EffectType.MAGESIGHT,
                "SEVER" => EffectType.SEVER,
                "TELEPORT" => EffectType.TELEPORT,
                "DEBUFF" => EffectType.DEBUFF,
                "BUFF" => EffectType.BUFF,
                _ => throw new ApplicationException($"It wasn't possible to convert the effect, " +
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

        private static DamageTypes StringToDamageType(string st)
        {
            return st switch
            {
                "None" => DamageTypes.None,
                "Blunt" => DamageTypes.Blunt,
                "Sharp" => DamageTypes.Sharp,
                "Point" => DamageTypes.Point,
                "Force" => DamageTypes.Force,
                "Fire" => DamageTypes.Fire,
                "Cold" => DamageTypes.Cold,
                "Lighting" => DamageTypes.Ligthing,
                "Poison" => DamageTypes.Poison,
                "Acid" => DamageTypes.Acid,
                "Shock" => DamageTypes.Shock,
                "Soul" => DamageTypes.Soul,
                "Mind" => DamageTypes.Mind,
                _ => throw new Exception($"This isn't a valid damage type, please use a valid one. Value used: {st}")
            };
        }

        /// <summary>
        /// Is used to instantiate and return any number of possible effects
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="jToken"></param>
        /// <returns></returns>
        private static ISpellEffect EnumToEffect(EffectType effect, JToken jToken)
        {
            bool? canMiss = (bool?)jToken["CanMiss"];
            bool? isHealing = (bool?)jToken["IsHealing"];
            int? raidus = (int?)jToken["Radius"];
            bool? resistable = (bool?)jToken["IsResistable"];
            return effect switch
            {
                EffectType.DAMAGE => new DamageEffect
                    ((int)jToken["BaseDamage"], StringToAreaEffect((string)jToken["AreaOfEffect"]),
                    StringToDamageType((string)jToken["SpellDamageType"]))
                {
                    CanMiss = canMiss ?? false,
                    IsHealing = isHealing ?? false,
                    Radius = raidus ?? 0,
                    IsResistable = resistable ?? false,
                },
                EffectType.HASTE => new HasteEffect(StringToAreaEffect((string)jToken["AreaOfEffect"]),
                    (int)jToken["HastePower"],
                    (int)jToken["Duration"],
                    StringToDamageType((string)jToken["SpellDamageType"])),
                EffectType.MAGESIGHT => new MageSightEffect((int)jToken["Duration"]),
                EffectType.SEVER => new SeverEffect(StringToAreaEffect((string)jToken["AreaOfEffect"]),
                    StringToDamageType((string)jToken["SpellDamageType"]),
                    (int)jToken["Radius"], (int)jToken["BaseDamage"]),
                EffectType.TELEPORT => new TeleportEffect(StringToAreaEffect((string)jToken["AreaOfEffect"]),
                    StringToDamageType((string)jToken["SpellDamageType"]),
                    (int)jToken["Radius"]),
                _ => null,
            };
        }

        private static ArtMagic StringToSchool(string st)
        {
            return st switch
            {
                "Projection" => ArtMagic.Projection, // 1
                "Negation" => ArtMagic.Negation, // 2
                "Animation" => ArtMagic.Animation, // 3
                "Divination" => ArtMagic.Divination, // 4
                "Alteration" => ArtMagic.Alteration, // 5
                "Wards" => ArtMagic.Wards, // 6
                "Dimensionalism" => ArtMagic.Dimensionalism, // 7
                "Conjuration" => ArtMagic.Conjuration, // 8
                "Illuminism" => ArtMagic.Illuminism, // 9
                "MedicalMagic" => ArtMagic.MedicalMagic, // 10
                "MindMagic" => ArtMagic.MindMagic, // 11
                "SoulMagic" => ArtMagic.SoulMagic, // 12
                "BloodMagic" => ArtMagic.BloodMagic, // 13
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

        public ArtMagic MagicArt { get; set; }

        public int SpellRange { get; set; }

        public double ManaCost { get; set; }
        public string SpellId { get; set; }
        public List<string> Keywords { get; set; } = new();

        public SpellTemplate(int spellLevel, List<ISpellEffect> effects, string spellName, string description,
            ArtMagic magicArt, int spellRange, double manaCost, string spellId,
            List<string> keywords)
        {
            SpellLevel = spellLevel;
            Effects = effects;
            SpellName = spellName;
            Description = description;
            MagicArt = magicArt;
            SpellRange = spellRange;
            ManaCost = manaCost;
            SpellId = spellId;
            Keywords = keywords;
        }

        public static implicit operator SpellBase(SpellTemplate spellTemplate)
        {
            SpellBase spell = new SpellBase(spellTemplate.SpellId, spellTemplate.SpellName,
                spellTemplate.MagicArt, spellTemplate.SpellRange, spellTemplate.SpellLevel,
                spellTemplate.ManaCost)
            {
                Proficiency =
                spellTemplate.Proficiency is null ? 0 : (double)spellTemplate.Proficiency
            };
            spell.Effects.AddRange(spellTemplate.Effects);
            spell.SetDescription(spellTemplate.Description);
            return spell;
        }

        public static implicit operator SpellTemplate(SpellBase spell)
        {
            SpellTemplate template =
                new SpellTemplate(spell.SpellLevel, spell.Effects, spell.SpellName,
                spell.Description, spell.MagicArt, spell.SpellRange, spell.ManaCost, spell.SpellId, spell.Keywords)
                {
                    Proficiency = spell.Proficiency
                };

            return template;
        }
    }
}