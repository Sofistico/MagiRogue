using MagiRogue.Entities;
using MagiRogue.GameSys.Magic;
using MagiRogue.GameSys.Physics;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Utils;

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

        public float Weight { get; set; }

        public int Size { get; set; }

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

        public FurnitureTemplate()
        {
            // empty constructor for json serializer
        }

        public FurnitureTemplate(
            string name,
            string foreground,
            string background,
            char glyph,
            float weight,
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
            Size = size;
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
            float weight,
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
            Size = size;
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
                template.BackgroundBackingField = new MagiColorSerialization(Color.Transparent);
                template.ForegroundBackingField = material.ReturnMagiColor();
            }

            var objFur = new Furniture(template.ForegroundBackingField.Color,
                template.BackgroundBackingField.Color, template.Glyph, template.Position,
                template.FurnitureType, template.MaterialId, template.Name, template.Id,
                template.Weight, template.Durability)
            {
                UseActions = template.UseActions,
                Traits = template.Traits,
                Magic = template.MagicStuff,
                MapIdConnection = template.MapIdConnection,
                Size = template.Size,
                Description = template.Description,
                Qualities = Quality.ReturnQualityList(template.Qualities),
            };

            return objFur;
        }

        public static implicit operator FurnitureTemplate(Furniture fur)
        {
            var template = new FurnitureTemplate(fur.Name,
                fur.Appearance.Foreground.PackedValue,
                fur.Appearance.Background.PackedValue,
                fur.Appearance.GlyphCharacter,
                fur.Weight,
                fur.Size,
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

            return template;
        }
    }
}