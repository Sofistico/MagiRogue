using GoRogue.DiceNotation.Terms;
using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using MagiRogue.Entities.Core;
using MagiRogue.Entities.Interfaces;
using MagiRogue.GameSys.Magic;
using MagiRogue.GameSys.Physics;
using MagiRogue.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Data.Serialization.EntitySerialization
{
    public class FurnitureJsonConverter : JsonConverter<Furniture>
    {
        public override Furniture? ReadJson(JsonReader reader, Type objectType, Furniture? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<FurnitureTemplate>(reader);
        }

        public override void WriteJson(JsonWriter writer, Furniture? value, JsonSerializer serializer)
        {
            serializer.NullValueHandling = NullValueHandling.Ignore;
            FurnitureTemplate template = (FurnitureTemplate)value;
            serializer.Serialize(writer, template);
        }
    }

    public class FurnitureTemplate
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

        public int Height { get; set; }

        public int Length { get; set; }

        public int Broadness { get; set; }

        public int Durability { get; set; }

        public string Description { get; set; }

        public string MaterialId { get; set; }

        public MagicManager MagicStuff { get; set; }

        public const string EntityType = "Furniture";

        public uint ForegroundPackedValue { get; set; }

        public uint BackgroundPackedValue { get; set; }
        public Point Position { get; set; }
        public FurnitureType FurnitureType { get; set; }
        public List<IActivable> UseActions { get; set; }
        public List<Trait> Traits { get; set; }
        public List<List<string>> Qualities { get; set; }
        public int? MapIdConnection { get; set; }
        public List<object> Inventory { get; set; }

        public FurnitureTemplate()
        {
            // empty constructor for json serializer
        }

        public FurnitureTemplate(
            string name,
            string foreground,
            string background,
            char glyph,
            double weight,
            int size,
            string description,
            string materialId,
            MagicManager magicStuff,
            Point position,
            FurnitureType furnitureType)
        {
            Name = name;
            ForegroundBackingField = new MagiColorSerialization(foreground);
            BackgroundBackingField = new MagiColorSerialization(background);
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Weight = weight;
            Volume = size;
            Description = description;
            MaterialId = materialId;
            MagicStuff = magicStuff;
            ForegroundPackedValue = ForegroundBackingField.Color.PackedValue;
            BackgroundPackedValue = BackgroundBackingField.Color.PackedValue;
            Position = position;
            FurnitureType = furnitureType;
        }

        public FurnitureTemplate(
            string name,
            uint foreground,
            uint background,
            char glyph,
            double weight,
            int size,
            string description,
            string materialId,
            MagicManager magicStuff,
            Point position,
            FurnitureType furnitureType)
        {
            Name = name;
            Glyph = glyph;
            Weight = weight;
            Volume = size;
            Description = description;
            MaterialId = materialId;
            MagicStuff = magicStuff;
            ForegroundPackedValue = foreground;
            BackgroundPackedValue = background;
            ForegroundBackingField = new MagiColorSerialization(foreground);
            BackgroundBackingField = new MagiColorSerialization(background);
            Position = position;
            FurnitureType = furnitureType;
        }

        private static void DetermineInvFromJson(FurnitureTemplate template, Furniture objFur)
        {
            List<Item> inv = new();
            if (template.Inventory is null)
                return;
            for (int i = 0; i < template.Inventory.Count; i++)
            {
                var obj = template.Inventory[i];
                if (obj is JArray array)
                {
                    for (int z = 0; z < ((int)array[1]); z++)
                    {
                        inv.Add(DataManager.QueryItemInData(array[0].ToString()));
                    }
                }
                else
                {
                    inv.Add(DataManager.QueryItemInData(obj.ToString()));
                }
            }
            objFur.Inventory = inv;
        }

        private static void DetermineInventoryFromItem(Furniture fur, FurnitureTemplate template)
        {
            List<object> invIds = new();
            for (int i = 0; i < fur.Inventory.Count; i++)
            {
                var item = fur.Inventory[i];

                if (invIds.Any(v => v.ToString().Contains(item.ItemId)))
                    continue;
                int invCount = fur.Inventory.Where(i => i.ItemId.Equals(item.ItemId)).Count();
                if (invCount > 1)
                {
                    invIds.Add(new JArray(item.ItemId, invCount));
                }
                else
                {
                    invIds.Add(item.ItemId);
                }
            }

            template.Inventory = invIds;
        }

        public static implicit operator Furniture(FurnitureTemplate template)
        {
            if (template.Id == null && template.MagicStuff == null)
                return null;
            if (string.IsNullOrEmpty(template.MaterialId))
            {
                throw new Exception($"Tried to create a furniture with no material! \nName: {template.Name} Type: {template.FurnitureType}");
            }

            if (template.Foreground == null && template.Background == null)
            {
                // TODO: will transplate to entity as a static method
                var material = PhysicsManager.SetMaterial(template.MaterialId);
                template.BackgroundBackingField = new MagiColorSerialization(Color.Black);
                template.ForegroundBackingField = material.ReturnMagiColor();
            }

            int glpyh = GlyphHelper.GlyphExistInDictionary(template.Glyph) ? GlyphHelper.GetGlyph(template.Glyph)
                : template.Glyph;

            var objFur = new Furniture(template.ForegroundBackingField.Color,
                template.BackgroundBackingField.Color, glpyh, template.Position,
                template.FurnitureType, template.MaterialId, template.Name, template.Id, template.Durability)
            {
                UseActions = template.UseActions,
                Magic = template.MagicStuff,
                MapIdConnection = template.MapIdConnection,
                Volume = template.Volume,
                Description = template.Description,
                Qualities = Quality.ReturnQualityList(template.Qualities),
            };
            DetermineInvFromJson(template, objFur);
            if (template.Traits is not null)
            {
                objFur.Traits.AddRange(template.Traits);
            }
            if (objFur.Material.ConfersTraits is not null && objFur.Material.ConfersTraits.Count > 0)
            {
                objFur.Traits.AddRange(objFur.Material.ConfersTraits);
            }

            objFur.Broadness = template.Broadness;
            objFur.Height = template.Height;
            objFur.Length = template.Length;

            return objFur;
        }

        public static implicit operator FurnitureTemplate(Furniture fur)
        {
            var template = new FurnitureTemplate(fur.Name,
                fur.AppearanceSingle.Appearance.Foreground.PackedValue,
                fur.AppearanceSingle.Appearance.Background.PackedValue,
                fur.AppearanceSingle.Appearance.GlyphCharacter,
                fur.Weight,
                fur.Volume,
                fur.Description,
                fur.Material.Id,
                fur.Magic,
                fur.Position,
                fur.FurnitureType)
            {
                Durability = fur.Durability,
                UseActions = fur.UseActions,
                Traits = fur.Traits,
                MapIdConnection = fur.MapIdConnection,
                Id = fur.FurId,
                Qualities = Quality.ReturnQualityListAsString(fur.Qualities),
            };
            DetermineInventoryFromItem(fur, template);

            template.Broadness = fur.Broadness;
            template.Height = fur.Height;
            template.Length = fur.Length;

            return template;
        }
    }
}