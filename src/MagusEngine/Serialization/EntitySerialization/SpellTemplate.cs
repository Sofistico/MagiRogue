using Arquimedes.Enumerators;
using MagusEngine.Core.Magic;
using MagusEngine.Core.Magic.Effects;
using MagusEngine.Core.Magic.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MagusEngine.Serialization.EntitySerialization
{
    public class SpellJsonConverter : JsonConverter<Spell>
    {
        public override Spell ReadJson(JsonReader reader, Type objectType,
            Spell? existingValue, bool hasExistingValue, JsonSerializer serializer)
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
                //if (token["ConeCircleSpan"] != null)
                //    eff.ConeCircleSpan = (double)token["ConeCircleSpan"]!;
                //if (token["EffectMessage"] != null)
                //    eff.EffectMessage = token["EffectMessage"]?.ToString();
                effectsList.Add(eff);
            }

            Spell createdSpell = new(
                (string)spell["Id"]!,
                (string)spell["Name"]!,
                StringToSchool((string)spell["MagicArt"]!),
                (int)spell["SpellRange"]!,
                spell["ShapingAbility"]!.ToString(),
                (int)spell["SpellLevel"]!,
                (double)spell["MagicCost"]!)
            {
                Description = spell["Description"]?.ToString(),
                Effects = effectsList,
                //Steps = JsonConvert.DeserializeObject<List<IMagicStep>>(spell["Steps"]!.ToString())!,
            };
            if (spell.ContainsKey("Proficiency"))
                createdSpell.Proficiency = (double)spell["Proficiency"]!;
            if (spell.ContainsKey("Keywords"))
                createdSpell.Keywords = JsonConvert.DeserializeObject<List<string>>(spell["Keywords"]!.ToString())!;
            if (spell.ContainsKey("Context"))
                createdSpell.Context = JsonConvert.DeserializeObject<List<SpellContext>>(spell["Context"]!.ToString())!;
            if (spell.ContainsKey("Back"))
                createdSpell.Back = spell["Back"]!.ToString();
            if (spell.ContainsKey("Fore"))
                createdSpell.Fore = spell["Fore"]!.ToString();
            if (spell.ContainsKey("Glyphs"))
                createdSpell.Glyphs = spell["Glyphs"]?.ToObject<char[]>()!;
            if (spell.ContainsKey("Velocity"))
                createdSpell.Velocity = (int)spell["Velocity"]!;
            if (spell.ContainsKey("Manifestation"))
                createdSpell.Manifestation = spell["Manifestation"]!.ToObject<SpellManifestation>()!;

            return createdSpell;
        }

        public override void WriteJson(JsonWriter writer, Spell? value, JsonSerializer serializer)
        {
            //serializer.Formatting = Formatting.Indented;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer, value!);
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

        /// <summary>
        /// Is used to instantiate and return any number of possible effects
        /// </summary>
        private static ISpellEffect? EnumToEffect(EffectType effect, JToken jToken)
        {
            return effect switch
            {
                EffectType.DAMAGE => jToken.ToObject<DamageEffect>(),
                EffectType.HASTE => jToken.ToObject<HasteEffect>()!,
                EffectType.MAGESIGHT => jToken.ToObject<MageSightEffect>(),
                EffectType.SEVER => jToken.ToObject<SeverEffect>(),
                EffectType.TELEPORT => jToken.ToObject<TeleportEffect>(),
                EffectType.KNOCKBACK => jToken.ToObject<KnockbackEffect>(),
                EffectType.LIGHT => jToken.ToObject<LightEffect>(),
                EffectType.MEMISSION => jToken.ToObject<MEssionEffect>(),
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

        public string Name { get; set; }

        public string? Description { get; set; }

        public ArtMagic MagicArt { get; set; }

        public int SpellRange { get; set; }

        public double MagicCost { get; set; }
        public string Id { get; set; }
        public List<SpellContext>? Context { get; set; } = [];
        public List<string>? Keywords { get; set; } = [];
        public string ShapingAbility { get; set; }
        public SpellCostType CostType { get; set; } = SpellCostType.Mana;
        public SpellManifestation Manifestation { get; set; }
        public string Fore { get; set; } = "{Caster}";
        public string Back { get; set; } = "Transparent";
        public char[] Glyphs { get; set; } = ['*'];
        public List<IMagicStep> Steps { get; set; }
        public int Velocity { get; set; } // is in ticks
        public bool AffectsTile { get; set; }

        public SpellTemplate()
        {
        }

        public SpellTemplate(int spellLevel, List<ISpellEffect> effects, string name, string? description,
            ArtMagic magicArt, int spellRange, double magicCost, string spellId,
            List<SpellContext>? context, string shapingAbility)
        {
            SpellLevel = spellLevel;
            Effects = effects;
            Name = name;
            Description = description;
            MagicArt = magicArt;
            SpellRange = spellRange;
            MagicCost = magicCost;
            Id = spellId;
            Context = context;
            ShapingAbility = shapingAbility;
        }

        public static implicit operator Spell(SpellTemplate spellTemplate)
        {
            return new(spellTemplate.Id, spellTemplate.Name,
                spellTemplate.MagicArt, spellTemplate.SpellRange, spellTemplate.ShapingAbility, spellTemplate.SpellLevel,
                spellTemplate.MagicCost)
            {
                Proficiency = spellTemplate.Proficiency is null ? 0 : (double)spellTemplate.Proficiency,
                Context = spellTemplate.Context,
                Keywords = spellTemplate.Keywords,
                Description = spellTemplate.Description,
                Effects = spellTemplate.Effects,
                Back = spellTemplate.Back,
                CostType = spellTemplate.CostType,
                Fore = spellTemplate.Fore,
                Glyphs = spellTemplate.Glyphs,
                Manifestation = spellTemplate.Manifestation,
                Steps = spellTemplate.Steps,
                Velocity = spellTemplate.Velocity,
            };
        }

        public static implicit operator SpellTemplate(Spell spell)
        {
            return new SpellTemplate(spell.SpellLevel, spell.Effects, spell.Name,
                spell.Description, spell.MagicArt, spell.SpellRange, spell.MagicCost, spell.Id, spell.Context, spell.ShapingAbility)
            {
                Proficiency = spell.Proficiency,
                Keywords = spell.Keywords,
            };
        }
    }
}
