using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Planet.History;
using MagiRogue.Utils;

namespace MagiRogue.GameSys.Planet
{
    public class Discovery
    {
        public uint Id { get; set; }
        public int HFIdThatDiscovered { get; set; }
        public string WhatHappenead { get; set; }
        public string WhereHappnead { get; set; }
        public Research WhatWasResearched { get; set; }
        public bool IsActive { get; set; }

        public Discovery()
        {
            Id = GameLoop.IdGen.UseID();
        }

        public Discovery(int hFIdThatDiscovered,
            string whatHappenead,
            string whereHappnead,
            Research whatWasResearched)
        {
            Id = GameLoop.IdGen.UseID();
            HFIdThatDiscovered = hFIdThatDiscovered;
            WhatHappenead = whatHappenead;
            WhereHappnead = whereHappnead;
            WhatWasResearched = whatWasResearched;
        }
    }

    public class DiscoveryResearch
    {
        public Discovery DiscoveryBeingRes { get; set; }
        public int RequiredRP { get; set; } // RP = Research Points
        public int CurrentRP { get; set; }
        public bool Finished { get => CurrentRP >= RequiredRP; }

        public DiscoveryResearch(Discovery discovery, int currentRpIfAny = 0)
        {
            DiscoveryBeingRes = discovery;
            CurrentRP = currentRpIfAny;
            RequiredRP = discovery.WhatWasResearched.Difficulty * (Mrn.Exploding2D6Dice / 2);
        }
    }
}