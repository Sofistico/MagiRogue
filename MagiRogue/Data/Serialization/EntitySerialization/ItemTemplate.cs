using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using MagiRogue.Utils;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Serialization.EntitySerialization
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
        public List<IActivable> UseAction { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Trait> Traits { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<List<string>> Qualities { get; set; }
        public EquipType EquipType { get; set; }

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
                UseAction = item.UseAction,
                Description = item.Description,
                Traits = item.Traits,
                Qualities = Quality.ReturnQualityListAsString(item.Qualities),
                EquipType = item.EquipType,
            };

            return itemSerialized;
        }

        public static implicit operator Item(ItemTemplate itemTemplate)
        {
            int glyph = GlyphHelper.GlyphExistInDictionary(itemTemplate.Glyph) ?
                GlyphHelper.GetGlyph(itemTemplate.Glyph) : itemTemplate.Glyph;
            MagiColorSerialization foreground = new MagiColorSerialization(itemTemplate.Foreground);
            Color background = !string.IsNullOrEmpty(itemTemplate.Background) ? itemTemplate.BackgroundBackingField.Color : Color.Transparent;

            Item item = new(foreground.Color,
                background,
                itemTemplate.Name,
                glyph,
                Point.None,
                itemTemplate.Size,
                itemTemplate.Weight,
                itemTemplate.Condition,
                materialId: itemTemplate.MaterialId)
            {
                BaseDmg = itemTemplate.BaseDamage,
                CanInteract = itemTemplate.CanInteract,
                ItemDamageType = itemTemplate.DamageType,
                UseAction = itemTemplate.UseAction,
                Description = itemTemplate.Description,
                Traits = itemTemplate.Traits,
                Qualities = Quality.ReturnQualityList(itemTemplate.Qualities),
                ItemId = itemTemplate.Id,
                EquipType = itemTemplate.EquipType,
            };
            if (itemTemplate.Condition != 100)
            {
                item.Condition = itemTemplate.Condition;
            }
            item.Description = itemTemplate.Description;

            return item;
        }
    }
}