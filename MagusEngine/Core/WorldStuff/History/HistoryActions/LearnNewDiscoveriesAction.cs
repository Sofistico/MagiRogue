using MagiRogue.Data;
using MagiRogue.Utils;
using MagusEngine.Core.Civ;
using MagusEngine.Core.WorldStuff.History;
using MagusEngine.Core.WorldStuff.TechRes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Core.WorldStuff.History.HistoryActions
{
    internal sealed class LearnNewDiscoveriesAction : IHistoryAct
    {
        public bool? Act(HistoricalFigure figure)
        {
            return LearnNewDiscoveriesKnowToTheSite(figure);
        }

        private static bool LearnNewDiscoveriesKnowToTheSite(HistoricalFigure figure)
        {
            int? currentSiteId = figure.GetCurrentStayingSiteId(Find.Sites);
            if (currentSiteId.HasValue)
            {
                int familiarityBonus = 0;
                Site currentSite = Find.Sites.Find(i => i.Id == currentSiteId);
                if (figure.GetLivingSiteId().HasValue && currentSiteId == figure.GetLivingSiteId().Value)
                    familiarityBonus = Mrn.Exploding2D6Dice * 2;
                for (int i = 0; i < currentSite.DiscoveriesKnow.Count; i++)
                {
                    Discovery disc = currentSite.DiscoveriesKnow[i];
                    figure.AddDiscovery(disc);
                }
                return true;
            }
            return false;
        }
    }
}
