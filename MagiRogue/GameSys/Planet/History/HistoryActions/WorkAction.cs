using MagiRogue.Data.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    internal class WorkAction : IHistoryAct
    {
        private readonly WorkType work;

        public WorkAction(WorkType workType)
        {
            this.work = workType;
        }

        public bool? Act(HistoricalFigure figure)
        {
            throw new NotImplementedException();
        }
    }
}
