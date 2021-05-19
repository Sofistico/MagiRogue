using Microsoft.Xna.Framework;
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
        public Stat Stats { get; set; }
        [DataMember]
        public Anatomy Anatomy { get; set; }
        [DataMember]
        public int Layer { get; set; }
        [DataMember]
        public ColorSerialized Foreground { get; set; }
        [DataMember]
        public ColorSerialized Background { get; set; }
        [DataMember]
        public int Size { get; set; }
        [DataMember]
        public float Weight { get; set; }

        public ActorTemplate(string name, Color foreground, Color background, int glyph,
            int layer, Stat stats, Anatomy anatomy, string description, int size, float weight)
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
                GoRogue.Coord.NONE)
                {
                    Description = actorTemplate.Description,
                    Stats = actorTemplate.Stats,
                    Anatomy = actorTemplate.Anatomy,
                    Size = actorTemplate.Size,
                    Weight = actorTemplate.Weight
                };

            return actor;
        }

        public static implicit operator ActorTemplate(Actor actor)
        {
            ActorTemplate actorTemplate = new ActorTemplate(actor.Name, actor.Animation[0].Foreground,
               actor.Animation[0].Background,
               actor.Animation[0].Glyph,
               actor.Layer,
               actor.Stats,
               actor.Anatomy,
               actor.Description,
               actor.Size,
               (float)actor.Weight);

            return actorTemplate;
        }
    }
}