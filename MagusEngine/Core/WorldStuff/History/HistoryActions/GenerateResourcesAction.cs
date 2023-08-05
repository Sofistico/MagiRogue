using MagiRogue.Data;
using MagiRogue.Data.Enumerators;
using MagusEngine.Core.Civ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Core.WorldStuff.History.HistoryActions
{
    internal sealed class GenerateResourcesAction : IHistoryAct
    {
        private readonly ResourceType resource;

        public GenerateResourcesAction(ResourceType resourceType)
        {
            resource = resourceType;
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
