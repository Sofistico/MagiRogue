using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using MagiRogue.Utils;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Serialization
{
    public class ItemJsonConverter : JsonConverter<Item>
    {
        public override Item ReadJson(JsonReader reader,
            Type objectType,
            Item existingValue,
            bool hasExistingValue,
            JsonSerializer serializer) => serializer.Deserialize<ItemTemplate>(reader);

        public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer)
        {
            var item = (ItemTemplate)value;
            //item.SerialId = (int)GameLoop.IdGen.UseID();
            serializer.Serialize(writer, item);
        }
    }

    [DataContract]
    [Serializable]
    public class ItemTemplate
    {
        /// <summary>
        /// An item creator, defines the template in how it should be created, acess this to create new items.
        /// This is used for the deserialization.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="weight"></param>
        /// <param name="condition">Defaults to 100%</param>
        public ItemTemplate(string name, string foreground, string background, int glyph,
            float weight, int size, string description, string materialId, int condition = 100)
        {
            Name = name;
            Foreground = foreground;
            Background = background;
            Glyph = (char)glyph;
            Weight = weight;
            Condition = condition;
            Description = description;
            Size = size;
            MaterialId = materialId;
            Material = GameSys.Physics.PhysicsManager.SetMaterial(materialId);

            ForegroundBackingField = new MagiColorSerialization(foreground);
            BackgroundBackingField = new MagiColorSerialization(background);
        }

        /// <summary>
        /// An item creator, defines the template in how it should be created, acess this to create new items.
        /// This is used in the serialization of the item.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="weight"></param>
        /// <param name="condition">Defaults to 100%</param>
        public ItemTemplate(string name, uint foreground, uint background, int glyph,
            float weight, int size, string materialId, MagicManager magic, int condition = 100)
        {
            Name = name;
            ForegroundBackingField = new MagiColorSerialization(foreground);
            BackgroundBackingField = new MagiColorSerialization(background);
            ForegroundPackedValue = foreground;
            BackgroundPackedValue = background;
            Glyph = (char)glyph;
            Weight = weight;
            Condition = condition;
            Size = size;
            MaterialId = materialId;
            Material = GameSys.Physics.PhysicsManager.SetMaterial(materialId);
            MagicStuff = magic;
        }

        public ItemTemplate()
        {
        }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; internal set; }

        [JsonIgnore]
        public MagiColorSerialization ForegroundBackingField { get; internal set; }

        [JsonIgnore]
        public MagiColorSerialization BackgroundBackingField { get; internal set; }

        [DataMember]
        public string Foreground { get; internal set; }

        [DataMember]
        public string Background { get; internal set; }

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

        public MaterialTemplate Material { get; set; }

        [DataMember]
        public MagicManager MagicStuff { get; set; }

        [DataMember]
        public const string EntityType = "Item";

        [DataMember]
        public uint ForegroundPackedValue { get; internal set; }

        [DataMember]
        public uint BackgroundPackedValue { get; internal set; }

        [DataMember]
        public Point Position { get; set; }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int BaseDamage { get; private set; }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool CanInteract { get; private set; }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DamageType DamageType { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<IActivable> Actives { get; set; }

        // Will need to see if it works, but so far the logic seems to check
        public static implicit operator ItemTemplate(Item item)
        {
            ItemTemplate itemSerialized = new(item.Name,
                item.Appearance.Foreground.PackedValue,
                item.Appearance.Background.PackedValue,
                item.Appearance.Glyph,
                item.Weight,
                item.Size,
                item.Material.Id,
                item.Magic,
                item.Condition
                )
            {
                CanInteract = item.CanInteract,
                BaseDamage = item.BaseDmg,
                DamageType = item.ItemDamageType,
                Actives = item.Actives,
            };

            return itemSerialized;
        }

        public static implicit operator Item(ItemTemplate itemTemplate)
        {
            Item item = new(itemTemplate.ForegroundBackingField.Color,
                itemTemplate.BackgroundBackingField.Color,
                itemTemplate.Name,
                itemTemplate.Glyph,
                Point.None,
                itemTemplate.Size,
                itemTemplate.Weight,
                itemTemplate.Condition)
            {
                BaseDmg = itemTemplate.BaseDamage,
                CanInteract = itemTemplate.CanInteract,
                ItemDamageType = itemTemplate.DamageType,
                Actives = itemTemplate.Actives
            };

            item.Material = GameSys.Physics.PhysicsManager.SetMaterial(itemTemplate.MaterialId);
            item.Description = itemTemplate.Description;

            return item;
        }
    }
}