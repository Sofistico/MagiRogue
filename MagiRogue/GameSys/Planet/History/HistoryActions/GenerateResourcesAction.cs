using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    internal class GenerateResourcesAction : IHistoryAct
    {
        private readonly ResourceType resource;

        public GenerateResourcesAction(ResourceType resourceType)
        {
            this.resource = resourceType;
        }

        public bool? Act(HistoricalFigure figure)
        {
            throw new NotImplementedException();
        }
    }
}
