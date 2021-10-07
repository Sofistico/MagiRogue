using MagiRogue.Entities;
using Newtonsoft.Json;
using SadConsole.SerializedTypes;
using SadRogue.Primitives;
using System;
using System.Runtime.Serialization;

namespace MagiRogue.Data
{
    public class ActorJsonConverter : JsonConverter<Actor>
    {
        public override void WriteJson(JsonWriter writer, Actor value, JsonSerializer serializer)
            => serializer.Serialize(writer, (ActorTemplate)value);

        public override Actor ReadJson(JsonReader reader,
            Type objectType,
            Actor existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
            => serializer.Deserialize<ActorTemplate>(reader);
    }

    [DataContract]
    [Serializable]
    public class ActorTemplate
    {
        /// <summary>
        /// Different from the id of GoRogue, this is for easy acess and navigation in the json, should be unique for
        /// each actor and human redable.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public char Glyph { get; set; }

        [DataMember]
        public int Size { get; set; }

        [DataMember]
        public float Weight { get; set; }

        [DataMember]
        public string MaterialId { get; set; }

        [DataMember]
        public Stat Stats { get; set; }

        [DataMember]
        public Anatomy Anatomy { get; set; }

        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int Layer { get; set; }

        [JsonIgnore]
        public MagiColorSerialization ForegroundBackingField { get; internal set; }

        [JsonIgnore]
        public MagiColorSerialization BackgroundBackingField { get; internal set; }

        public string Foreground { get; internal set; }

        public string Background { get; internal set; }

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
        public ActorTemplate(string name, Color foreground, Color background, int glyph,
            int layer, Stat stats, Anatomy anatomy, string description, int size, float weight, string materialId)
        {
            Name = name;
            Glyph = (char)glyph;
            Stats = stats;
            Anatomy = anatomy;
            Description = description;
            Layer = layer;
            Size = size;
            Weight = weight;
            MaterialId = materialId;

            ForegroundBackingField = new MagiColorSerialization(foreground);
            BackgroundBackingField = new MagiColorSerialization(background);
        }

        public ActorTemplate(string name, string foreground, string background, int glyph,
           int layer, Stat stats, Anatomy anatomy, string description, int size, float weight, string materialId)
        {
            Name = name;
            Foreground = foreground;
            Background = background;
            Glyph = (char)glyph;
            Stats = stats;
            Anatomy = anatomy;
            Description = description;
            Layer = layer;
            Size = size;
            Weight = weight;
            MaterialId = materialId;

            ForegroundBackingField = new MagiColorSerialization(foreground);
            BackgroundBackingField = new MagiColorSerialization(background);
        }

        public ActorTemplate(Actor actor)
        {
            Name = actor.Name;
            ForegroundBackingField = new MagiColorSerialization(actor.Appearance.Foreground);
            BackgroundBackingField = new MagiColorSerialization(actor.Appearance.Background);
            Glyph = (char)actor.Appearance.Glyph;
            Stats = actor.Stats;
            Anatomy = actor.Anatomy;
            Description = actor.Description;
            Layer = actor.Layer;
            Size = actor.Size;
            Weight = actor.Weight;
            MaterialId = actor.Material.Id;
        }

        public ActorTemplate()
        {
        }

        public static implicit operator Actor(ActorTemplate actorTemplate)
        {
            Actor actor =
                new Actor(actorTemplate.Name, actorTemplate.ForegroundBackingField.Color,
                actorTemplate.BackgroundBackingField.Color,
                actorTemplate.Glyph,
                Point.None)
                {
                    Description = actorTemplate.Description,
                    Stats = actorTemplate.Stats,
                    Anatomy = actorTemplate.Anatomy,
                    Size = actorTemplate.Size,
                    Weight = actorTemplate.Weight,
                    Material = System.Physics.PhysicsManager.SetMaterial(actorTemplate.MaterialId)
                };

            return actor;
        }

        public static implicit operator ActorTemplate(Actor actor)
        {
            ActorTemplate actorTemplate = new ActorTemplate(actor.Name, actor.Appearance.Foreground,
               actor.Appearance.Background,
               actor.Appearance.Glyph,
               actor.Layer,
               actor.Stats,
               actor.Anatomy,
               actor.Description,
               actor.Size,
               actor.Weight,
               actor.Material.Id);

            return actorTemplate;
        }
    }
}