using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;
using SadConsole.SerializedTypes;
using Newtonsoft.Json;

namespace MagiRogue.Entities.Data
{
    public sealed class ActorJsonConverter : JsonConverter<Actor>
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
        [DataMember]
        public ColorSerialized Foreground { get; set; }
        [DataMember]
        public ColorSerialized Background { get; set; }
        [DataMember]
        public char Glyph { get; set; }
        [DataMember]
        public Stat Stats { get; set; }
        [DataMember]
        public Anatomy Anatomy { get; set; }
        [DataMember]
        public int Layer { get; set; }
        [DataMember]
        public string Description { get; set; }

        public ActorTemplate(Color foreground, Color background, int glyph, int layer, Stat stats, Anatomy anatomy, string description)
        {
            Foreground = foreground;
            Background = background;
            Glyph = (char)glyph;
            Stats = stats;
            Anatomy = anatomy;
            Description = description;
            Layer = layer;
        }

        public ActorTemplate(Color foreground, Color background, int glyph, int layer, string description)
        {
            Foreground = foreground;
            Background = background;
            Glyph = (char)glyph;
            Layer = layer;
            Description = description;
        }

        public static implicit operator Actor(ActorTemplate actorTemplate)
        {
            Actor actor = new Actor(actorTemplate.Foreground,
                actorTemplate.Background,
                actorTemplate.Glyph,
                new GoRogue.Coord());

            return actor;
        }

        public static implicit operator ActorTemplate(Actor actor)
        {
            ActorTemplate actorTemplate = new ActorTemplate(actor.Animation[0].Foreground,
               actor.Animation[0].Background,
               actor.Animation[0].Glyph,
               actor.Layer,
               actor.Stats,
               actor.Anatomy,
               actor.Description);

            return actorTemplate;
        }
    }
}