using GoRogue.Components;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.Entities.Core;
using MagiRogue.Entities.Interfaces;
using MagiRogue.GameSys.Magic;
using MagiRogue.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MagusEngine.Serialization.EntitySerialization
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
            double weight, int size, string materialId, MagicManager magic, int condition = 100)
        {
            Name = name;
            ForegroundBackingField = new MagiColorSerialization(foreground);
            BackgroundBackingField = new MagiColorSerialization(background);
            ForegroundPackedValue = foreground;
            BackgroundPackedValue = background;
            Glyph = (char)glyph;
            Weight = weight;
            Condition = condition;
            Volume = size;
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
        public MagiColorSerialization ForegroundBackingField { get; set; }

        [JsonIgnore]
        public MagiColorSerialization BackgroundBackingField { get; set; }

        [DataMember]
        public string Foreground { get; set; }

        [DataMember]
        public string Background { get; set; }

        [DataMember]
        public char Glyph { get; set; }

        [DataMember]
        public double Weight { get; set; }

        [DataMember]
        public int Volume { get; set; }

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
        public DamageTypes DamageType { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<IActivable> UseAction { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Trait> Traits { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<List<string>> Qualities { get; set; }
        public EquipType EquipType { get; set; }
        public ItemType ItemType { get; set; }

        [DataMember]
        public int SpeedOfAttack { get; set; }

        [DataMember]
        public WeaponType WeaponType { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public int Length { get; set; }

        [DataMember]
        public int Broadness { get; set; }

        [DataMember]
        public List<Attack> Attacks { get; set; }

        [DataMember]
        public ArmorType ArmorType { get; set; }

        [DataMember]
        public int Coverage { get; set; }

        // Will need to see if it works, but so far the logic seems to check
        public static implicit operator ItemTemplate(Item item)
        {
            ItemTemplate itemSerialized = new(item.Name,
                item.AppearanceSingle.Appearance.Foreground.PackedValue,
                item.AppearanceSingle.Appearance.Background.PackedValue,
                item.AppearanceSingle.Appearance.Glyph,
                item.Weight,
                item.Volume,
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
                ItemType = item.ItemType,
                SpeedOfAttack = item.SpeedOfAttack,
                WeaponType = item.WeaponType,
                Attacks = item.Attacks,
                Broadness = item.Broadness,
                Height = item.Height,
                Length = item.Length,
                Id = item.ItemId,
                ArmorType = item.ArmorType,
                Coverage = item.Coverage,
            };
            return itemSerialized;
        }

        public static implicit operator Item(ItemTemplate itemTemplate)
        {
            int glyph = GlyphHelper.GlyphExistInDictionary(itemTemplate.Glyph)
                ? GlyphHelper.GetGlyph(itemTemplate.Glyph) : itemTemplate.Glyph;
            MagiColorSerialization foreground = new MagiColorSerialization(itemTemplate.Foreground);
            MagiColorSerialization background = new MagiColorSerialization(itemTemplate.Background);

            Item item = new(foreground.Color,
                background.Color,
                itemTemplate.Name,
                glyph,
                Point.None,
                itemTemplate.Volume,
                itemTemplate.Condition,
                materialId: itemTemplate.MaterialId)
            {
                BaseDmg = itemTemplate.BaseDamage,
                CanInteract = itemTemplate.CanInteract,
                ItemDamageType = itemTemplate.DamageType,
                UseAction = itemTemplate.UseAction,
                Description = itemTemplate.Description,
                Qualities = Quality.ReturnQualityList(itemTemplate.Qualities),
                ItemId = itemTemplate.Id,
                EquipType = itemTemplate.EquipType,
                ItemType = itemTemplate.ItemType,
                SpeedOfAttack = itemTemplate.SpeedOfAttack,
                WeaponType = itemTemplate.WeaponType,
                Attacks = itemTemplate.Attacks,
                ArmorType = itemTemplate.ArmorType,
                Coverage = itemTemplate.Coverage,
            };
            if (itemTemplate.Traits is not null)
                item.Traits = itemTemplate.Traits;
            if (itemTemplate.Condition != 100)
            {
                item.Condition = itemTemplate.Condition;
            }
            item.Description = itemTemplate.Description;
            if (item.Material?.ConfersTraits?.Count > 0)
            {
                item.Traits.AddRange(item.Material.ConfersTraits);
            }

            item.Broadness = itemTemplate.Broadness;
            item.Height = itemTemplate.Height;
            item.Length = itemTemplate.Length;

            return item;
        }
    }
}