﻿using MagiRogue.Data.Serialization;
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

namespace MagiRogue.Entities
{
    [JsonConverter(typeof(FurnitureJsonConverter))]
    public class Furniture : Entities.Entity
    {
        public FurnitureType FurnitureType { get; }
        public int? MapIdConnection { get; set; }
        public string FurId { get; set; }
        public int Durability { get; set; }
        public List<Trait> Traits { get; internal set; }
        public List<IActivable> UseActions { get; internal set; }

        public Furniture(Color foreground, Color background, int glyph, Point coord,
            FurnitureType type, string materialId, string name, string furId = null,
            float weight = 0, int durability = 0)
            : base(foreground, background, glyph, coord, (int)MapLayer.FURNITURE)
        {
            Traits = new();
            UseActions = new();
            FurnitureType = type;
            Material = PhysicsManager.SetMaterial(materialId);
            // makes sure that the furniture is named by it's material
            Name = Material.ReturnNameFromMaterial(name);
            Durability = (int)(Material.Hardness * Material.Density) + durability;
            FurId = furId;
            Weight = MathF.Round((float)Material.Density * weight, 2);
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