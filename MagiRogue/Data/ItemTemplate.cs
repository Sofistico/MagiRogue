using MagiRogue.Entities;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Runtime.Serialization;

namespace MagiRogue.Data
{
    public class ItemJsonConverter : JsonConverter<Item>
    {
        public override Item ReadJson(JsonReader reader,
            Type objectType,
            Item existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) => serializer.Deserialize<ItemTemplate>(reader);

        public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer)
            => serializer.Serialize(writer, (ItemTemplate)value);
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
        public ItemTemplate(string name, Color foreground, Color background, int glyph,
            float weight, int size, string description, int condition = 100)
        {
            Name = name;
            Foreground = foreground;
            Background = background;
            Glyph = (char)glyph;
            Weight = weight;
            Condition = condition;
            Description = description;
            Size = size;
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
        public char Glyph { get; internal set; }

        [DataMember]
        public float Weight { get; internal set; }

        [DataMember]
        public int Size { get; set; }

        [DataMember]
        public int Condition { get; internal set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string MaterialId { get; set; }

        // Will need to see if it works, but so far the logic seems to check
        public static implicit operator ItemTemplate(Item item)
        {
            ItemTemplate itemSerialized = new ItemTemplate(item.Name,
                item.Appearance.Foreground,
                item.Appearance.Background,
                item.Appearance.Glyph,
                item.Weight,
                item.Size,
                item.Description,
                item.Condition
                )
            {
                MaterialId = item.Material.Id
            };

            return itemSerialized;
        }

        public static implicit operator Item(ItemTemplate itemTemplate)
        {
            Item item = new Item(itemTemplate.Foreground,
                itemTemplate.Background,
                itemTemplate.Name,
                itemTemplate.Glyph,
                Point.None,
                itemTemplate.Size,
                itemTemplate.Weight,
                itemTemplate.Condition);
            item.Material = System.Physics.PhysicsManager.SetMaterial(itemTemplate.MaterialId);

            return item;
        }
    }
}