using MagiRogue.Data.Enumerators;
using MagusEngine.Core.Civ;
using MagusEngine.Core.WorldStuff.History;
using System.Text;

namespace MagusEngine.Core.WorldStuff.TechRes
{
    public class Discovery
    {
        public int Id { get; set; }
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
            Id = SequentialIdGenerator.DiscoveryId;
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

        public static Discovery ReturnDiscoveryFromResearch(Research res, HistoricalFigure figure, Site site)
        {
            return new Discovery(figure.Id,
                $"{figure.Name} finished research on ",
                $"in the {site.Name}",
                res);
        }
    }
}