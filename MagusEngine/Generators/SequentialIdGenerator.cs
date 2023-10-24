using Arquimedes.Enumerators;
using System;
using System.Collections.Generic;

namespace MagusEngine.Generators
{
    public static class SequentialIdGenerator
    {
        #region Ids

        private const int siteId = 0;
        private const int roadId = 1;
        private const int hfId = 2;
        private const int civId = 3;
        private const int mythId = 4;
        private const int abilityId = 5;
        private const int discoveryId = 6;
        private const int riverId = 7;

        #endregion Ids

        private static readonly Dictionary<int, int> idsDirectory;

        public static int SiteId { get => idsDirectory[siteId]++; }

        public static int RoadId { get => idsDirectory[roadId]++; }

        public static int HistoricalFigureId { get => idsDirectory[hfId]++; }
        public static int CivId { get => idsDirectory[civId]++; }
        public static int MythId { get => idsDirectory[mythId]++; }
        public static int AbilityId { get => idsDirectory[abilityId]++; }
        public static int DiscoveryId => idsDirectory[discoveryId]++;

        public static int RiverId => idsDirectory[riverId]++;

        static SequentialIdGenerator()
        {
            int totalAbilityEnum = Enum.GetNames<AbilityName>().Length;
            // Here it must be defined any new ids, to properly initialize itself!
            idsDirectory = new Dictionary<int, int>
            {
                { siteId, 0 },
                { roadId, 0 },
                { hfId, 0 },
                { civId, 0 },
                { mythId, 0 },
                { abilityId, totalAbilityEnum },
                { riverId, 0},
            };
        }
    }
}