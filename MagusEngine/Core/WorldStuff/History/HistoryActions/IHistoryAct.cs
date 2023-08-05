using MagusEngine.Core.WorldStuff.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagusEngine.Core.WorldStuff.History.HistoryActions
{
    public interface IHistoryAct
    {
        public bool? Act(HistoricalFigure figure);
    }
}
