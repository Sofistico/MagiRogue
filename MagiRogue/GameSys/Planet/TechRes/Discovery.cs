using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Planet.History;
using System.Text;

namespace MagiRogue.GameSys.Planet.TechRes
{
    public class Discovery
    {
        public uint Id { get; set; }
        public int? HFIdThatDiscovered { get; set; }
        public string? WhatHappenead { get; set; }
        public string? WhereHappnead { get; set; }
        public Research WhatWasResearched { get; set; }
        public bool IsActive { get; set; }

        public Discovery()
        {
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
            IsActive = true;
        }

        public Legend ReturnLegendFromDiscovery(int year)
        {
            StringBuilder builder = new($"In the year {year} ");
            builder.Append(WhatHappenead);
            builder.Append(WhereHappnead);
            return new Legend(builder.ToString(), year);
        }
    }
}