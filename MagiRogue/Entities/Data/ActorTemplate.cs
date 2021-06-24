using SadRogue.Primitives;
using System;
using System.Runtime.Serialization;
using SadConsole.SerializedTypes;
using Newtonsoft.Json;

namespace MagiRogue.Entities.Data
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

        //[DataMember]
        public Color Foreground { get; set; }

        //[DataMember]
        public Color Background { get; set; }

        [DataMember]
        public ColoredGlyphSerialized ColoredGlyphSerialized { get; set; }

        public ActorTemplate(string name, Color foreground, Color background, int glyph,
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
            ColoredGlyphSerialized = new ColoredGlyphSerialized()
            {
                Background = background,
                Foreground = foreground,
                Glyph = glyph
            };
        }

        public ActorTemplate(Actor actor)
        {
            Name = actor.Name;
            Foreground = actor.Appearance.Foreground;
            Background = actor.Appearance.Background;
            Glyph = (char)actor.Appearance.Glyph;
            Stats = actor.Stats;
            Anatomy = actor.Anatomy;
            Description = actor.Description;
            Layer = actor.Layer;
            Size = actor.Size;
            Weight = actor.Weight;
            MaterialId = actor.Material.Id;
            ColoredGlyphSerialized = actor.Appearance;
        }

        public ActorTemplate()
        {
        }

        public static implicit operator Actor(ActorTemplate actorTemplate)
        {
            Actor actor =
                new Actor(actorTemplate.Name, actorTemplate.Foreground,
                actorTemplate.Background,
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