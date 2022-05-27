using MagiRogue.Data.Serialization;
using MagiRogue.GameSys;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    [JsonConverter(typeof(FurnitureJsonConverter))]
    public class Furniture : Entities.Entity
    {
        public List<IActivable> FurnitureEffects { get; set; }
        public FurnitureType FurnitureType { get; }
        public int? MapIdConnection { get; set; }
        public string FurId { get; set; }
        public int Durability { get; set; }

        public Furniture(Color foreground, Color background, int glyph, Point coord,
            FurnitureType type, string furId = null)
            : base(foreground, background, glyph, coord, (int)MapLayer.FURNITURE)
        {
            FurnitureEffects = new();
            FurnitureType = type;
            Name = type.ToString();
            Durability += (int)(Material.Hardness * Material.Density);
            FurId = furId;
            Weight *= (float)Material.Density;
        }
    }

    /// <summary>
    /// What type of furniture is it?
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FurnitureType
    {
        /// <summary>
        /// Any kind really, too generic to say
        /// </summary>
        General,
        /// <summary>
        /// Stairs down
        /// </summary>
        StairsDown,
        /// <summary>
        /// Stairs up
        /// </summary>
        StairsUp,
        /// <summary>
        /// chair
        /// </summary>
        Chair,
        /// <summary>
        /// Can be a eating table, a studying table, a hitting table
        /// </summary>
        Table,
        /// <summary>
        /// To store books and dust
        /// </summary>
        BookCase
    }
}