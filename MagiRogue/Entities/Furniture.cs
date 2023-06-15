using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Entities.Core;
using MagiRogue.Entities.Interfaces;
using MagiRogue.GameSys.Physics;
using MagiRogue.Utils;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagiRogue.Entities
{
    [JsonConverter(typeof(FurnitureJsonConverter))]
    public class Furniture : MagiEntity
    {
        public FurnitureType FurnitureType { get; set; }
        public int? MapIdConnection { get; set; }
        public string FurId { get; set; }
        public int Durability { get; set; }
        public List<Trait> Traits { get; set; }
        public List<IActivable> UseActions { get; set; }
        public List<Quality> Qualities { get; set; }
        public List<Item> Inventory { get; set; }
        public MaterialTemplate Material { get; set; }

        public override double Weight
        {
            get
            {
                return MathMagi.GetWeightWithDensity(Material.Density, Volume);
            }
        }

        public Furniture(Color foreground, Color background, int glyph, Point coord,
            FurnitureType type, string materialId, string name, string furId = null, int durability = 0)
            : base(foreground, background, glyph, coord, (int)MapLayer.FURNITURE)
        {
            Traits = new();
            UseActions = new();
            Qualities = new();
            FurnitureType = type;
            Material = PhysicsManager.SetMaterial(materialId);
            // makes sure that the furniture is named by it's material
            // which is quite stupid, if i say so myself...
            Name = Material.ReturnNameFromMaterial(name);
            Durability = (int)(Material.Hardness * Material.Density) + durability;
            FurId = furId;
            Inventory = new();
        }

        public override Furniture Copy()
        {
            var baseEntity = base.Copy();
            Furniture fur = new Furniture(baseEntity.AppearanceSingle.Appearance.Foreground,
                baseEntity.AppearanceSingle.Appearance.Background, baseEntity.AppearanceSingle.Appearance.Glyph,
                baseEntity.Position,
                FurnitureType, Material.Id, Name, FurId, Durability)
            {
                Traits = Traits,
                UseActions = UseActions,
                Qualities = Qualities,
                Material = Material,
                MapIdConnection = MapIdConnection,
            };
            return fur;
        }
    }
}