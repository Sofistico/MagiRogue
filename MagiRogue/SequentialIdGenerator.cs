using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue
{
    public static class SequentialIdGenerator
    {
        private static int siteId;
        private static int roadId;

        public static int SiteId { get => siteId++; }

        public static int RoadId { get => roadId++; }
    }
}