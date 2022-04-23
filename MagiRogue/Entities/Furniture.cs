using MagiRogue.GameSys;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Entities
{
    public class Furniture : Entities.Entity
    {
        public List<IActivable> FurnitureEffects { get; set; }
        public FurnitureType FurnitureType { get; }
        public Map MapConnection { get; internal set; }

        public Furniture(Color foreground, Color background, int glyph, Point coord, int layer,
            FurnitureType type)
            : base(foreground, background, glyph, coord, layer)
        {
            FurnitureEffects = new();
            FurnitureType = type;
            Name = type.ToString();
        }
    }

    /// <summary>
    /// What type of furniture is it?
    /// </summary>
    public enum FurnitureType
    {
        /// <summary>
        /// Any kind really, to generic to say
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