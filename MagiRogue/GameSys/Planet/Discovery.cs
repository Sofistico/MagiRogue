using MagiRogue.Data.Enumerators;

namespace MagiRogue.GameSys.Planet
{
    public class Discovery
    {
        public int Id { get; set; }
        public int HFIdThatDiscovered { get; set; }
        public string WhatHappenead { get; set; }
        public string WhereHappnead { get; set; }
        public Tech WhatWasDicovered { get; set; }

        public Discovery()
        {
        }

        public Discovery(int id,
            int hFIdThatDiscovered,
            string whatHappenead,
            string whereHappnead,
            Tech whatWasDicovered)
        {
            Id = id;
            HFIdThatDiscovered = hFIdThatDiscovered;
            WhatHappenead = whatHappenead;
            WhereHappnead = whereHappnead;
            WhatWasDicovered = whatWasDicovered;
        }
    }
}