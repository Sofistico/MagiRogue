using Arquimedes.Enumerators;
using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Entities.Interfaces;
using MagusEngine.Core.Magic;
using MagusEngine.Systems;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MagusEngine.Serialization.EntitySerialization
{
    public class ItemJsonConverter : JsonConverter<Item>
    {
        public override Item? ReadJson(JsonReader reader,
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

    public class ItemTemplate : IJsonKey
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public MagiColorSerialization ForegroundBackingField { get; set; }

        [JsonIgnore]
        public MagiColorSerialization BackgroundBackingField { get; set; }

        public string Foreground { get; set; }

        public string Background { get; set; }

        public char Glyph { get; set; }

        public double Weight { get; set; }

        public int Volume { get; set; }

        public int Condition { get; set; } = 100;

        public string Description { get; set; }

        public string? MaterialId { get; set; }
        public Trait? MaterialTrait { get; set; }
        public MaterialType? MaterialType { get; set; }

        public Material Material { get; set; }

        public MagicManager MagicStuff { get; set; }

        public const string EntityType = "Item";

        public uint ForegroundPackedValue { get; set; }

        public uint BackgroundPackedValue { get; set; }

        public Point Position { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int BaseDamage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool CanInteract { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DamageTypes DamageType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<IActivable> UseAction { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Trait> Traits { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<List<string>> Qualities { get; set; }
        public EquipType EquipType { get; set; }
        public ItemType ItemType { get; set; }

        public int SpeedOfAttack { get; set; }

        public WeaponType WeaponType { get; set; } = WeaponType.Misc;

        public int Height { get; set; }

        public int Length { get; set; }

        public int Broadness { get; set; }

        public List<Attack> Attacks { get; set; }

        public ArmorType ArmorType { get; set; }

        public int Coverage { get; set; }

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
            Material = DataManager.QueryMaterial(materialId)!;
            MagicStuff = magic;
        }

        public ItemTemplate()
        {
        }

        public static implicit operator ItemTemplate(Item? item)
        {
            return new(item.Name,
                item.SadCell.AppearanceSingle.Appearance.Foreground.PackedValue,
                item.SadCell.AppearanceSingle.Appearance.Background.PackedValue,
                item.SadCell.AppearanceSingle.Appearance.Glyph,
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
                MaterialTrait = item.MaterialTrait,
                MaterialType = item.MaterialType,
            };
        }

        public static implicit operator Item?(ItemTemplate? itemTemplate)
        {
            if (itemTemplate is null)
                return null;
            int glyph = GlyphHelper.GlyphExistInDictionary(itemTemplate.Glyph) ? GlyphHelper.GetGlyph(itemTemplate.Glyph) : itemTemplate.Glyph;
            Material material;
            if (itemTemplate.Material != null)
                material = itemTemplate.Material;
            else if (!itemTemplate.MaterialId.IsNullOrEmpty())
                material = DataManager.QueryMaterial(itemTemplate.MaterialId);
            else
                material = DataManager.QueryMaterialWithType(itemTemplate.MaterialType.Value);
            MagiColorSerialization foreground = new(itemTemplate.Foreground ?? material.Color);
            MagiColorSerialization background = new(itemTemplate.Background ?? "Black");

            Item item = new(foreground.Color,
                background.Color,
                itemTemplate.Name,
                glyph,
                Point.None,
                itemTemplate.Volume,
                itemTemplate.Condition,
                materialId: material.Id)
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
                MaterialTrait = itemTemplate.MaterialTrait,
                MaterialType = material.Type,
            };
            if (itemTemplate.Traits is not null)
                item.Traits = itemTemplate.Traits;
            if (itemTemplate.Condition != 100)
                item.Condition = itemTemplate.Condition;
            item.Description = itemTemplate.Description;
            if (item?.Material?.ConfersTraits?.Count > 0)
                item.Traits.AddRange(item.Material.ConfersTraits);

            item!.Broadness = itemTemplate.Broadness;
            item!.Height = itemTemplate.Height;
            item!.Length = itemTemplate.Length;

            return item;
        }
    }
}
