using MagiRogue.Data.Serialization;
using MagiRogue.GameSys;
using MagiRogue.GameSys.Physics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Utils;
using MagiRogue.Data.Serialization.EntitySerialization;
using MagiRogue.Data.Enumerators;

namespace MagiRogue.Entities
{
    [JsonConverter(typeof(FurnitureJsonConverter))]
    public class Furniture : Entities.Entity
    {
        public FurnitureType FurnitureType { get; set; }
        public int? MapIdConnection { get; set; }
        public string FurId { get; set; }
        public int Durability { get; set; }
        public List<Trait> Traits { get; internal set; }
        public List<IActivable> UseActions { get; internal set; }
        public List<Quality> Qualities { get; internal set; }

        public Furniture(Color foreground, Color background, int glyph, Point coord,
            FurnitureType type, string materialId, string name, string furId = null,
            float weight = 0, int durability = 0)
            : base(foreground, background, glyph, coord, (int)MapLayer.FURNITURE)
        {
            Traits = new();
            UseActions = new();
            Qualities = new();
            FurnitureType = type;
            Material = PhysicsManager.SetMaterial(materialId);
            // makes sure that the furniture is named by it's material
            Name = Material.ReturnNameFromMaterial(name);
            Durability = (int)(Material.Hardness * Material.Density) + durability;
            FurId = furId;
            Weight = MathMagi.Round((float)Material.Density * weight);
        }

        public override Furniture Copy()
        {
            var baseEntity = base.Copy();
            Furniture fur = new Furniture(baseEntity.Appearance.Foreground,
                baseEntity.Appearance.Background, baseEntity.Appearance.Glyph,
                baseEntity.Position,
                FurnitureType, Material.Id, Name, FurId, Weight, Durability)
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