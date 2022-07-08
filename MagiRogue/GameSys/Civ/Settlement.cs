using MagiRogue.Data.Enumerators;

namespace MagiRogue.GameSys.Civ
{
    public class Settlement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SettlementSize Size { get; set; }
        public int TotalPopulation { get; set; }
    }
}