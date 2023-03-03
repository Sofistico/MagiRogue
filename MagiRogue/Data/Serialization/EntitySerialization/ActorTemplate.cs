using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class ActorJsonConverter : JsonConverter<Actor>
    {
        public override void WriteJson(JsonWriter writer, Actor value, JsonSerializer serializer)
        {
            ActorTemplate actor = (ActorTemplate)value;
            serializer.NullValueHandling = NullValueHandling.Ignore;
            //actor.SerialId = (int)GameLoop.IdGen.UseID();
            serializer.Serialize(writer, actor);
        }

        public override Actor ReadJson(JsonReader reader,
            Type objectType,
            Actor existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
            => serializer.Deserialize<ActorTemplate>(reader);
    }

    [Serializable]
    public class ActorTemplate
    {
        /// <summary>
        /// Different from the id of GoRogue, this is for easy acess and navigation in the json, should be unique for
        /// each actor and human redable.
        /// </summary>
        public string ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public char Glyph { get; set; }

        public int GlyphInt { get; set; }

        /// <summary>
        /// <inheritdoc cref="MagiEntity.Volume"/>
        /// </summary>

        public int Volume { get; set; }

        public int Height { get; set; }

        public int Length { get; set; }

        public int Broadness { get; set; }

        public double Weight { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int Layer { get; set; }

        [JsonIgnore]
        public MagiColorSerialization ForegroundBackingField { get; internal set; }

        [JsonIgnore]
        public MagiColorSerialization BackgroundBackingField { get; internal set; }

        public string Foreground { get; internal set; }

        public uint ForegroundPackedValue { get; internal set; }

        public string Background { get; internal set; }

        public uint BackgroundPackedValue { get; internal set; }

        public List<AbilityTemplate> Abilities { get; set; } = new();

        public List<ItemTemplate> Inventory { get; set; } = new();

        public const string EntityType = "Actor";

        public Point Position { get; set; }

        public bool? IsPlayer { get; set; }

        public MagicManager MagicStuff { get; set; }

        public List<EquipTemplate> Equip { get; set; }

        public Mind Mind { get; set; }

        public Soul Soul { get; set; }

        public Body Body { get; set; }

        public ActorState State { get; set; }

        /// <summary>
        /// Is used in the serialization of the actor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="layer"></param>
        /// <param name="stats"></param>
        /// <param name="anatomy"></param>
        /// <param name="description"></param>
        /// <param name="size"></param>
        /// <param name="weight"></param>
        /// <param name="materialId"></param>
        public ActorTemplate(string name, uint foreground, uint background, int glyph,
            int layer, Body body, int size, double weight,
            List<AbilityTemplate> abilities, MagicManager magic, Soul soul, Mind mind)
        {
            Name = name;
            Glyph = (char)glyph;
            Body = body;
            Soul = soul;
            Mind = mind;
            Layer = layer;
            Volume = size;
            Weight = weight;
            MagicStuff = magic;

            ForegroundBackingField = new MagiColorSerialization(foreground);
            BackgroundBackingField = new MagiColorSerialization(background);
            ForegroundPackedValue = foreground;
            BackgroundPackedValue = background;

            Abilities = abilities;
            Equip = new();
        }

        public ActorTemplate(Actor actor)
        {
            Name = actor.Name;
            ForegroundBackingField = new MagiColorSerialization(actor.Appearance.Foreground);
            BackgroundBackingField = new MagiColorSerialization(actor.Appearance.Background);
            Glyph = (char)actor.Appearance.Glyph;
            Body = actor.Body;
            Description = actor.Description;
            Layer = actor.Layer;
            Volume = actor.Volume;
            Weight = actor.Weight;
            Abilities = new();
            AddToAbilities(actor);
            Equip = new();
            Soul = actor.Soul;
            Mind = actor.Mind;
        }

        private void AddToAbilities(Actor actor)
        {
            for (int i = 0; i < actor.Mind.Abilities.Count; i++)
            {
                var actorAbility = actor.Mind.Abilities[i];
                var abilityTemplaye = new AbilityTemplate()
                {
                    Ability = actorAbility.ReturnAbilityEnumFromString(),
                    Score = actorAbility.Score,
                };
                Abilities.Add(abilityTemplaye);
            }
        }

        public ActorTemplate()
        {
        }

        //public Actor Copy()
        //{
        //    return new Actor();
        //}

        public static implicit operator Actor(ActorTemplate actorTemplate)
        {
            if (actorTemplate is null)
                return null;
            int glyph = GlyphHelper.GlyphExistInDictionary(actorTemplate.Glyph) ?
                GlyphHelper.GetGlyph(actorTemplate.Glyph) : actorTemplate.Glyph;

            Actor actor =
                new Actor(actorTemplate.Name, actorTemplate.ForegroundBackingField.Color,
                actorTemplate.BackgroundBackingField.Color, glyph,
                Point.None)
                {
                    Description = actorTemplate.Description,
                    Body = actorTemplate.Body,
                    Volume = actorTemplate.Volume,
                    Weight = actorTemplate.Weight,
                    State = actorTemplate.State
                };
            if (actorTemplate.Abilities?.Count > 0)
            {
                for (int i = 0; i < actorTemplate.Abilities.Count; i++)
                {
                    var abilityTemplate = actorTemplate.Abilities[i];
                    actor.Mind.AddAbilityToDictionary(abilityTemplate);
                }
            }
            if (!actorTemplate.Position.Equals(Point.None))
                actor.Position = actorTemplate.Position;
            if (actorTemplate.Inventory is not null)
            {
                for (int i = 0; i < actorTemplate.Inventory.Count; i++)
                {
                    actor.Inventory.Add(actorTemplate.Inventory[i]);
                }
            }
            // needs to add the equiped itens.
            actor.Magic = actorTemplate.MagicStuff;
            if (actorTemplate.Equip is not null)
            {
                for (int i = 0; i < actorTemplate.Equip.Count; i++)
                {
                    EquipTemplate equip = actorTemplate.Equip[i];
                    actor.AddToEquipment(actorTemplate.Inventory.Find(i => i.Id.Equals(equip.ItemEquipped)));
                }
            }

            if (actorTemplate.IsPlayer == true)
                actor.IsPlayer = true;

            if (actorTemplate.ForegroundPackedValue == 0 && !string.IsNullOrEmpty(actorTemplate.Foreground))
            {
                actor.Appearance.Foreground = new MagiColorSerialization(actorTemplate.Foreground).Color;
                actor.Appearance.Background = new MagiColorSerialization(actorTemplate.Background).Color;
            }
            else
            {
                actor.Appearance.Foreground =
                    new MagiColorSerialization(actorTemplate.ForegroundPackedValue).Color;
                actor.Appearance.Background =
                    new MagiColorSerialization(actorTemplate.BackgroundPackedValue).Color;
            }
            actor.Description = actorTemplate.Description;
            actor.Mind = actorTemplate.Mind;
            actor.Soul = actorTemplate.Soul;

            actor.Height = actorTemplate.Height;
            actor.Broadness = actorTemplate.Broadness;
            actor.Length = actorTemplate.Length;

            return actor;
        }

        public static implicit operator ActorTemplate(Actor actor)
        {
            var abilitylist = new List<AbilityTemplate>();

            if (actor.Mind.Abilities?.Count > 0)
            {
                foreach (var item in actor.Mind.Abilities.Values)
                {
                    abilitylist.Add(item);
                }
            }

            ActorTemplate actorTemplate = new ActorTemplate(actor.Name,
                actor.Appearance.Foreground.PackedValue,
               actor.Appearance.Background.PackedValue,
               actor.Appearance.Glyph,
               actor.Layer,
               actor.Body,
               actor.Volume,
               actor.Weight,
               abilitylist,
               actor.Magic,
               actor.Soul,
               actor.Mind);
            if (!string.IsNullOrWhiteSpace(actor.Description))
                actorTemplate.Description = actor.Description;

            actorTemplate.IsPlayer = actor is Player ? true : null;

            actorTemplate.Position = actor.Position;
            for (int a = 0; a < actor.Inventory.Count; a++)
            {
                actorTemplate.Inventory.Add(actor.Inventory[a]);
            }

            foreach (string limb in actor.GetEquipment().Keys)
            {
                actorTemplate.Equip.Add(new EquipTemplate(actor.GetEquipment()[limb].ItemId, limb));
            }
            actorTemplate.GlyphInt = actor.Appearance.Glyph;

            actorTemplate.Length = actor.Length;
            actorTemplate.Height = actor.Height;
            actorTemplate.Broadness = actor.Broadness;
            actorTemplate.State = actor.State;
            return actorTemplate;
        }
    }
}