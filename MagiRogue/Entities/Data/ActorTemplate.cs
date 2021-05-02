using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace MagiRogue.Entities.Data
{
    [DataContract]
    [Serializable]
    public class ActorTemplate
    {
        [DataMember]
        public Color Foreground { get; set; }
        [DataMember]
        public Color Background { get; set; }
        [DataMember]
        public int Glyph { get; set; }
        [DataMember]
        public Stat Stats { get; set; }
        [DataMember]
        public Anatomy Anatomy { get; set; }
        [DataMember]
        public int Layer { get; set; }
        [DataMember]
        public string Description { get; set; }

        public ActorTemplate(Color foreground, Color background, int glyph, Stat stats, Anatomy anatomy, string description)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Stats = stats;
            Anatomy = anatomy;
            Description = description;
        }

        public ActorTemplate(Color foreground, Color background, int glyph, int layer, string description)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Layer = layer;
            Description = description;
        }
    }
}