namespace MagiRogue.Entities.Materials
{
    public class Material
    {
        // Only putting in here for the sake of future me, only need to use JsonProperty if the name will be diferrent
        // than whats in the json.
        //[JsonProperty("Id")]

        public string Id { get; set; }
        public string Name { get; set; }
        public bool Flamability { get; set; }
        public int Hardness { get; set; }
        public int? MP_Infusion_Limit { get; set; }
        public bool CanRegen { get; set; }
        public double Density { get; set; }
        public int? Melting_Point { get; set; }
        public int? Boiling_Point { get; set; }
    }
}