using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Planet.History.HistoryActions
{
    public interface IHistoryAct
    {
        public bool? Act(HistoricalFigure figure);
    }
}
