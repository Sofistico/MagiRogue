using System.Collections.Generic;

namespace MagiRogue
{
    public static class SequentialIdGenerator
    {
        #region Ids

        private const int siteId = 0;
        private const int roadId = 1;

        #endregion Ids

        private static readonly Dictionary<int, int> idsDirectory;

        public static int SiteId { get => idsDirectory[siteId]++; }

        public static int RoadId { get => idsDirectory[roadId]++; }

        static SequentialIdGenerator()
        {
            // Here it must be defined any new ids, to properly initialize itself!
            idsDirectory = new Dictionary<int, int>
            {
                { siteId, 0 },
                { roadId, 0 }
            };
        }
    }
}