using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Planet.History;

namespace MagiRogue.GameSys.Planet
{
    public class Discovery
    {
        public int Id { get; set; }
        public int HFIdThatDiscovered { get; set; }
        public string WhatHappenead { get; set; }
        public string WhereHappnead { get; set; }
        public Research WhatWasResearched { get; set; }
        public int RequiredRP { get; set; } // RP = Research Points
        public int CurrentRP { get; set; }
        public bool IsActive { get; set; }
        public bool Finished { get => CurrentRP >= RequiredRP; }

        public Discovery()
        {
        }

        public Discovery(int id,
            int hFIdThatDiscovered,
            string whatHappenead,
            string whereHappnead,
            Research whatWasResearched)
        {
            Id = id;
            HFIdThatDiscovered = hFIdThatDiscovered;
            WhatHappenead = whatHappenead;
            WhereHappnead = whereHappnead;
            WhatWasResearched = whatWasResearched;
        }
    }
}