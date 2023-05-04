using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Serialization
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class MaterialTemplate
    {
        // Only putting in here for the sake of future me, only need to use JsonProperty if the name will be diferrent
        // than whats in the json.

        public string Id { get; set; }

        public string? InheirtFrom { get; set; }

        public string Name { get; set; }

        public bool Flamability { get; set; }

        public int? IgnationPoint { get; set; }

        public int Hardness { get; set; }

        public int? MPInfusionLimit { get; set; }

        public bool CanRegen { get; set; }

        public double Density { get; set; }

        public int? MeltingPoint { get; set; }

        public int? BoilingPoint { get; set; }

        // Will be removed!
        public string LiquidTurnsInto { get; set; }

        public List<Trait> ConfersTraits { get; set; }

        public string Color { get; set; }

        public MaterialType Type { get; set; }

        public int? HeatDamageTemp { get; set; }

        public int? ColdDamageTemp { get; set; }

        /// <summary>
        /// How much force is needed for the material to be damaged?
        /// The deformation limit!
        /// More means it's more elastic, less means it's more rigid
        /// affects in combat whether the corresponding tissue
        /// is bruised (value >= 50), torn (value between 25 and 49.999), or fractured (value <= 24.999)
        /// </summary>
        public double StrainsAtYield { get; set; } = 0;

        /// <summary>
        /// How sharp the material is. Used in cutting calculations. Does not allow an inferior metal to penetrate superior armor.
        /// Applying a value of at least 100 to a stone will allow weapons to be made from that stone.
        /// Defaults to 100.
        /// </summary>
        public double MaxEdge { get; set; } = 100;

        public int ShearYield { get; set; }
        public int ShearFracture { get; set; }

        /// <summary>
        /// Specifies how hard of an impact (in kilopascals)
        /// the material can withstand before it will start deforming permanently.
        /// Used for blunt-force combat. Defaults to 10000.
        /// </summary>
        public int ImpactYield { get; set; } = 10000;

        // the same as above, but in mpa
        public double ImpactYieldMpa => (double)ImpactYield / 1000;

        /// <summary>
        /// Specifies how hard of an impact the material can withstand before it will fail entirely.
        /// Used for blunt-force combat. Defaults to 10000.
        /// </summary>
        public int ImpactFracture { get; set; } = 10000;

        public double ImpactFractureMpa => (double)ImpactFracture / 1000;

        public MaterialTemplate Copy()
        {
            return new MaterialTemplate()
            {
                BoilingPoint = this.BoilingPoint,
                Density = this.Density,
                CanRegen = this.CanRegen,
                Flamability = this.Flamability,
                Hardness = this.Hardness,
                Id = this.Id,
                MeltingPoint = this.MeltingPoint,
                MPInfusionLimit = this.MPInfusionLimit,
                Name = this.Name,
                Color = this.Color,
                StrainsAtYield = this.StrainsAtYield,
                ColdDamageTemp = this.ColdDamageTemp,
                ConfersTraits = this.ConfersTraits,
                HeatDamageTemp = this.HeatDamageTemp,
                IgnationPoint = this.IgnationPoint,
                InheirtFrom = this.InheirtFrom,
                LiquidTurnsInto = this.LiquidTurnsInto,
                MaxEdge = this.MaxEdge,
                Type = this.Type,
                ImpactFracture = this.ImpactFracture,
                ImpactYield = this.ImpactYield,
                ShearFracture = this.ShearFracture,
                ShearYield = this.ShearYield,
            };
        }

        public static string ReturnNameFromMaterial(string materialName, string objectName)
        {
            if (objectName.Contains(materialName))
            {
                return objectName;
            }
            return $"{materialName} {objectName}";
        }

        public string ReturnNameFromMaterial(string objectName)
        {
            return ReturnNameFromMaterial(Name, objectName);
        }

        internal MagiColorSerialization ReturnMagiColor()
        {
            return new MagiColorSerialization(Color);
        }

        public MaterialTemplate GetMaterialThatLiquidTurns()
        {
            if (string.IsNullOrEmpty(LiquidTurnsInto))
                return null;
            return DataManager.QueryMaterial(LiquidTurnsInto);
        }

        private string GetDebuggerDisplay()
        {
            return $"{Name} - {Id}";
        }

        public MaterialState GetState(int temperature)
        {
            if (MeltingPoint is not null && temperature > MeltingPoint)
                return MaterialState.Liquid;
            else if (BoilingPoint is not null && temperature > BoilingPoint)
                return MaterialState.Gas;
            else
                return MaterialState.Solid;
        }
    }
}