﻿using System.Runtime.Serialization;

namespace MagiRogue.Entities.Materials
{
    [DataContract]
    public class Material
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
    }
}