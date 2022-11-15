using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    public class BuildSiteAction : IHistoryAct
    {
        private readonly SiteType site;

        public BuildSiteAction(SiteType siteType)
        {
            this.site = siteType;
        }

        public bool? Act(HistoricalFigure figure)
        {
            throw new NotImplementedException();
        }
    }
}
