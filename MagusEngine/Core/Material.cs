using Arquimedes.Enumerators;
using MagusEngine.Serialization;
using MagusEngine.Systems;
using MagusEngine.Systems.Physics;
using MagusEngine.Utils;
using MagusEngine.Utils.Extensions;
using System.Collections.Generic;
using System.Diagnostics;

namespace MagusEngine.Core
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class Material
    {
        private MagiColorSerialization? magiColorSerialization;

        public static Material? None => PhysicsManager.SetMaterial("null");

        public string Id { get; set; }
        public string Name { get; set; }

        public string? InheirtFrom { get; set; }

        public bool? Flamability { get; set; }

        public int? IgnationPoint { get; set; }

        public int? Hardness { get; set; }

        public int? MPInfusionLimit { get; set; }

        public bool? CanRegen { get; set; }

        public double? Density { get; set; }
        public double? DensityKgM3 => Density.HasValue ? MathMagi.GetDensityInKgM(Density.Value) : 0;

        public int? MeltingPoint { get; set; }

        public int? BoilingPoint { get; set; }

        public string? LiquidTurnsInto { get; set; }
        public string? SolidTurnsInto { get; set; }
        public string? GasTurnsInto { get; set; }

        public List<Trait>? ConfersTraits { get; set; }

        public string? Color { get; set; }

        public MaterialType? Type { get; set; }

        public int? HeatDamageTemp { get; set; }

        public int? ColdDamageTemp { get; set; }

        /// <summary>
        /// How sharp the material is. Used in cutting calculations. Does not allow an inferior
        /// metal to penetrate superior armor. Applying a value of at least 100 to a stone will
        /// allow weapons to be made from that stone.
        /// </summary>
        public double? MaxEdge { get; set; }

        public int? ShearYield { get; set; }
        public int? ShearFracture { get; set; }
        public double? ShearStrainAtYield { get; set; }

        /// <summary>
        /// Specifies how hard of an impact (in kilopascals) the material can withstand before it
        /// will start deforming permanently. Used for blunt-force combat.
        /// </summary>
        public int? ImpactYield { get; set; }

        // the same as above, but in mpa
        public double? ImpactYieldMpa => ImpactYield.HasValue ? ImpactYield / 1000 : 0;

        /// <summary>
        /// Specifies how hard of an impact the material can withstand before it will fail entirely.
        /// Used for blunt-force combat. Defaults to 10000.
        /// </summary>
        public int? ImpactFracture { get; set; }

        public double? ImpactFractureMpa => ImpactFracture.HasValue ? (double)ImpactFracture / 1000 : 0;

        /// <summary>
        /// How much force is needed for the material to be damaged? The deformation
        /// limit! More means it's more elastic, less means it's more rigid affects in combat
        /// whether the corresponding tissue is bruised (value >= 50000), torn (value between 25000
        /// and 49999), or fractured (value <= 24999)
        /// </summary>
        public double? ImpactStrainsAtYield { get; set; }

        public Material Copy()
        {
            return new Material()
            {
                BoilingPoint = BoilingPoint,
                Density = Density,
                CanRegen = CanRegen,
                Flamability = Flamability,
                Hardness = Hardness,
                Id = Id,
                MeltingPoint = MeltingPoint,
                MPInfusionLimit = MPInfusionLimit,
                Name = Name,
                Color = Color,
                ImpactStrainsAtYield = ImpactStrainsAtYield,
                ColdDamageTemp = ColdDamageTemp,
                ConfersTraits = ConfersTraits,
                HeatDamageTemp = HeatDamageTemp,
                IgnationPoint = IgnationPoint,
                InheirtFrom = InheirtFrom,
                LiquidTurnsInto = LiquidTurnsInto,
                MaxEdge = MaxEdge,
                Type = Type,
                ImpactFracture = ImpactFracture,
                ImpactYield = ImpactYield,
                ShearFracture = ShearFracture,
                ShearYield = ShearYield,
                ShearStrainAtYield = ShearStrainAtYield,
                GasTurnsInto = GasTurnsInto,
                SolidTurnsInto = SolidTurnsInto,
            };
        }

        public static string ReturnNameFromMaterial(string materialName, string objectName)
        {
            if (objectName?.Contains(materialName, System.StringComparison.OrdinalIgnoreCase) == true)
            {
                return objectName;
            }
            return $"{materialName.FirstLetterUpper()} {objectName}";
        }

        public string ReturnNameFromMaterial(string objectName)
        {
            return ReturnNameFromMaterial(Name, objectName);
        }

        public MagiColorSerialization ReturnMagiColor()
        {
            magiColorSerialization ??= new MagiColorSerialization(Color);
            return magiColorSerialization ?? default;
        }

        public Material GetMaterialThatLiquidTurns()
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

        public void CopyTo(Material mat)
        {
            var props = GetType().GetProperties();
            var sourceProps = GetType().GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var sourceProp = sourceProps[i];
                if (!prop.CanRead && !sourceProp.CanWrite)
                    continue;
                var propVal = prop.GetValue(this, null);
                var sourceVal = prop.GetValue(mat, null);
                if (propVal is null)
                    continue;
                if (sourceVal is not null)
                    continue;

                prop.SetValue(mat, propVal);
            }
        }
    }
}
