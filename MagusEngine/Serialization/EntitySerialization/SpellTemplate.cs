using Arquimedes.Enumerators;
using MagusEngine.Core.Magic;
using MagusEngine.Core.Magic.Effects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MagusEngine.Serialization.EntitySerialization
{
    public class SpellJsonConverter : JsonConverter<SpellBase>
    {
        public override SpellBase ReadJson(JsonReader reader, Type objectType,
            SpellBase? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject spell = JObject.Load(reader);
            IEnumerable<JToken> listEffectsJson = spell.SelectTokens("$.Effects[*]");
            var effectsList = new List<ISpellEffect>();

            foreach (JToken token in listEffectsJson)
            {
                EffectType effect = StringToEnumEffectType((string)token["EffectType"]!);
                var eff = EnumToEffect(effect, token);
                if (eff is null)
                    continue;
                if (token["ConeCircleSpan"] != null)
                    eff.ConeCircleSpan = (double)token["ConeCircleSpan"]!;
                if (token["EffectMessage"] != null)
                    eff.EffectMessage = token["EffectMessage"]?.ToString();
                effectsList.Add(eff);
            }

            SpellBase createdSpell = new(
                (string)spell["SpellId"]!,
                (string)spell["SpellName"]!,
                StringToSchool((string)spell["MagicArt"]!),
                (int)spell["SpellRange"]!,
                (int)spell["SpellLevel"]!,
                (double)spell["MagicCost"]!)
            {
                Description = spell["Description"]?.ToString(),
                Effects = effectsList,
                ShapingAbility = spell["ShapingAbility"]!.ToString()
            };
            if (spell.ContainsKey("Proficiency"))
                createdSpell.Proficiency = (double)spell["Proficiency"]!;
            if (spell.ContainsKey("Keywords"))
                createdSpell.Keywords = JsonConvert.DeserializeObject<List<string>>(spell["Keywords"]!.ToString())!;
            if (spell.ContainsKey("Context"))
                createdSpell.Context = JsonConvert.DeserializeObject<List<SpellContext>>(spell["Context"]!.ToString())!;
            if (spell.ContainsKey("AffectsTile"))
                createdSpell.AffectsTile = (bool)spell["AffectsTile"]!;

            return createdSpell;
        }

        public override void WriteJson(JsonWriter writer, SpellBase? value, JsonSerializer serializer)
        {
            //serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer, (SpellTemplate)value!);
        }

        /// <summary>
        /// This is used to convert a string to an enum of <see cref="EffectType">EffectType</see>
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        private static EffectType StringToEnumEffectType(string st)
        {
            return Enum.Parse<EffectType>(st);
        }

        private static SpellAreaEffect StringToAreaEffect(string st)
        {
            return Enum.Parse<SpellAreaEffect>(st);
        }

        /// <summary>
        /// Is used to instantiate and return any number of possible effects
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="jToken"></param>
        /// <returns></returns>
        private static ISpellEffect? EnumToEffect(EffectType effect, JToken jToken)
        {
            bool? canMiss = (bool?)jToken["CanMiss"];
            bool? isHealing = (bool?)jToken["IsHealing"];
            int? radius = (int?)jToken["Radius"];
            bool? resistable = (bool?)jToken["IsResistable"];
            string? damageId = (string?)jToken["SpellDamageTypeId"];
            return effect switch
            {
                EffectType.DAMAGE => new DamageEffect((int)jToken["BaseDamage"]!, StringToAreaEffect((string)jToken["AreaOfEffect"]!), damageId!)
                {
                    CanMiss = canMiss ?? false,
                    IsHealing = isHealing ?? false,
                    Radius = radius ?? 0,
                    IsResistable = resistable ?? false,
                },
                EffectType.HASTE => new HasteEffect(StringToAreaEffect((string)jToken["AreaOfEffect"]!),
                    (int)jToken["HastePower"]!,
                    (int)jToken["Duration"]!,
                    damageId!),
                EffectType.MAGESIGHT => new MageSightEffect((int)jToken["Duration"]!),
                EffectType.SEVER => new SeverEffect(StringToAreaEffect((string)jToken["AreaOfEffect"]!),
                    damageId!,
                    (int)jToken["Radius"]!, (int)jToken["BaseDamage"]!),
                EffectType.TELEPORT => new TeleportEffect(StringToAreaEffect((string)jToken["AreaOfEffect"]!),
                    damageId!,
                    (int)jToken["Radius"]!),
                EffectType.KNOCKBACK => JsonConvert.DeserializeObject<KnockbackEffect>(jToken.ToString()),
                EffectType.LIGHT => JsonConvert.DeserializeObject<LightEffect>(jToken.ToString()),
                _ => null,
            };
        }

        private static ArtMagic StringToSchool(string st)
        {
            return Enum.Parse<ArtMagic>(st);
        }
    }

    /// <summary>
    /// This class will deal with the serialization of the spells and it's effects, will use a tag
    /// like CDDA does to determine the effects
    /// </summary>
    public class SpellTemplate
    {
        public int SpellLevel { get; set; }

        public List<ISpellEffect> Effects { get; set; }

        public double? Proficiency { get; set; }

        public string SpellName { get; set; }

        public string? Description { get; set; }

        public ArtMagic MagicArt { get; set; }

        public int SpellRange { get; set; }

        public double MagicCost { get; set; }
        public string SpellId { get; set; }
        public List<SpellContext>? Context { get; set; } = [];
        public List<string>? Keywords { get; set; } = [];
        public string ShapingAbility { get; set; }

        public SpellTemplate(int spellLevel, List<ISpellEffect> effects, string spellName, string? description,
            ArtMagic magicArt, int spellRange, double MagicCost, string spellId,
            List<SpellContext>? context, string shapingAbility)
        {
            SpellLevel = spellLevel;
            Effects = effects;
            SpellName = spellName;
            Description = description;
            MagicArt = magicArt;
            SpellRange = spellRange;
            MagicCost = MagicCost;
            SpellId = spellId;
            Context = context;
            ShapingAbility = shapingAbility;
        }

        public static implicit operator SpellBase(SpellTemplate spellTemplate)
        {
            return new(spellTemplate.SpellId, spellTemplate.SpellName,
                spellTemplate.MagicArt, spellTemplate.SpellRange, spellTemplate.SpellLevel,
                spellTemplate.MagicCost)
            {
                Proficiency =
                spellTemplate.Proficiency is null ? 0 : (double)spellTemplate.Proficiency,
                Context = spellTemplate.Context,
                Keywords = spellTemplate.Keywords,
                Description = spellTemplate.Description,
                Effects = spellTemplate.Effects,
            };
        }

        public static implicit operator SpellTemplate(SpellBase spell)
        {
            return new SpellTemplate(spell.SpellLevel, spell.Effects, spell.SpellName,
                spell.Description, spell.MagicArt, spell.SpellRange, spell.MagicCost, spell.SpellId, spell.Context, spell.ShapingAbility)
            {
                Proficiency = spell.Proficiency,
                Keywords = spell.Keywords,
            };
        }
    }
}
