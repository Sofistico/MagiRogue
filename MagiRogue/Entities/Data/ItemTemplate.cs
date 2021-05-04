using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;

namespace MagiRogue.Entities.Data
{
    [DataContract]
    [Serializable]
    public class ItemTemplate
    {
        /// <summary>
        /// An item creator, defines the template in how it should be created, acess this to create new items.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="weight"></param>
        /// <param name="condition">Defaults to 100%</param>
        public ItemTemplate(string name, Color foreground, Color background, int glyph, float weight, string description, int condition = 100)
        {
            Name = name;
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Weight = weight;
            Condition = condition;
            Description = description;
        }

        public ItemTemplate()
        {
        }

        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; internal set; }
        [DataMember]
        public Color Foreground { get; internal set; }
        [DataMember]
        public Color Background { get; internal set; }
        [DataMember]
        public int Glyph { get; internal set; }
        [DataMember]
        public float Weight { get; internal set; }
        [DataMember]
        public int Size { get; set; }
        [DataMember]
        public int Condition { get; internal set; }
        [DataMember]
        public string Description { get; set; }
    }
}