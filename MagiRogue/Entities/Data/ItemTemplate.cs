using Microsoft.Xna.Framework;
using SadConsole.SerializedTypes;
using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MagiRogue.Entities.Data
{
    public class ItemJsonConverter : JsonConverter<Item>
    {
        public override Item ReadJson(JsonReader reader,
            Type objectType,
            Item existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) => serializer.Deserialize<ItemTemplate>(reader);

        public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer) => serializer.Serialize(writer, (ItemTemplate)value);
    }

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
        public ColorSerialized Foreground { get; internal set; }
        [DataMember]
        public ColorSerialized Background { get; internal set; }
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

        public static implicit operator ItemTemplate(Item item)
        {
        }

        public static implicit operator Item(ItemTemplate itemTemplate)
        {
        }
    }
}