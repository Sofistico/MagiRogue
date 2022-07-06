using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Data.Serialization
{
    [DataContract]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class MaterialTemplate
    {
        // Only putting in here for the sake of future me, only need to use JsonProperty if the name will be diferrent
        // than whats in the json.
        //[JsonProperty("Id")]

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool Flamability { get; set; }

        [DataMember]
        public int Hardness { get; set; }

        [DataMember]
        public int? MPInfusionLimit { get; set; }

        [DataMember]
        public bool CanRegen { get; set; }

        [DataMember]
        public double Density { get; set; }

        [DataMember]
        public int? MeltingPoint { get; set; }

        [DataMember]
        public int? BoilingPoint { get; set; }

        [DataMember]
        public string LiquidTurnsInto { get; set; }

        [DataMember]
        public List<Trait> ConfersTraits { get; set; }

        [DataMember]
        public string Color { get; set; }

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
            };
        }

        public static string ReturnNameFromMaterial(string materialName, string objectName)
        {
            if (objectName.Contains(materialName))
            {
                return objectName;
            }
            string v = $"{materialName} {objectName}";
            return v;
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
            return DataManager.QueryMaterials(LiquidTurnsInto);
        }

        private string GetDebuggerDisplay()
        {
            return $"{Name} - {Id}";
        }
    }
}