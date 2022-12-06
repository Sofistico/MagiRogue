using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Civ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    internal sealed class GenerateResourcesAction : IHistoryAct
    {
        private readonly ResourceType resource;

        public GenerateResourcesAction(ResourceType resourceType)
        {
            this.resource = resourceType;
        }

        public bool? Act(HistoricalFigure figure)
        {
            switch (resource)
            {
                case ResourceType.None:
                    return false;

                case ResourceType.Magical:

                    return GenerateMagicalResourcesForSite(figure);

                case ResourceType.Material:
                    break;

                case ResourceType.Monetary:
                    break;

                default:
                    return false;
            }

            return false;
        }

        private static bool GenerateMagicalResourcesForSite(HistoricalFigure figure)
        {
            Site site = Find.GetCurrentlyStayingSite(figure);
            if (site is not null)
            {
                site.MagicalResources += 10;
                return true;
            }
            return false;
        }
    }
}
