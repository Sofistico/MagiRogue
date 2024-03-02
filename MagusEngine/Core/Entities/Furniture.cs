using Arquimedes.Enumerators;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Entities.Interfaces;
using MagusEngine.Serialization;
using MagusEngine.Serialization.EntitySerialization;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagusEngine.Core.Entities
{
    [JsonConverter(typeof(FurnitureJsonConverter))]
    public class Furniture : MagiEntity, IJsonKey
    {
        public string Id { get; set; }
        public FurnitureType FurnitureType { get; set; }
        public int? MapIdConnection { get; set; }
        public int Durability { get; set; }
        public List<Trait> Traits { get; set; }
        public List<IActivable> UseActions { get; set; }
        public List<Quality> Qualities { get; set; }
        public List<Item> Inventory { get; set; }
        public Material Material { get; set; }

        public override double Weight
        {
            get
            {
                return MathMagi.GetWeightWithDensity(Material.Density ?? 0, Volume);
            }
        }

        public Furniture(Color foreground, Color background, int glyph, Point coord,
            FurnitureType type, string materialId, string name, string furId = null, int durability = 0)
            : base(foreground, background, glyph, coord, (int)MapLayer.FURNITURE)
        {
            Traits = [];
            UseActions = [];
            Qualities = [];
            FurnitureType = type;
            Material = DataManager.QueryMaterial(materialId);
            // makes sure that the furniture is named by it's material
            // which is quite stupid, if i say so myself...
            Name = Material.ReturnNameFromMaterial(name);
            Durability = (int)(Material?.Hardness ?? 1 * Material.Density) + durability;
            Id = furId;
            Inventory = [];
        }

        public override Furniture Copy()
        {
            var baseEntity = base.Copy();
            Furniture fur = new Furniture(baseEntity.SadCell.AppearanceSingle.Appearance.Foreground,
                baseEntity.SadCell.AppearanceSingle.Appearance.Background,
                baseEntity.SadCell.AppearanceSingle.Appearance.Glyph,
                baseEntity.Position,
                FurnitureType, Material.Id, Name, Id, Durability)
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
