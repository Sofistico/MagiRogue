using GoRogue.Components.ParentAware;
using MagiRogue.Entities;
using MagiRogue.GameSys;
using MagiRogue.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Components.Ai
{
    public class NeedDrivenAi : IAiComponent
    {
        public List<Need> Needs { get; }
        public IObjectWithComponents? Parent { get; set; }

        public NeedDrivenAi(List<Need> needs)
        {
            Needs = needs;
        }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            throw new NotImplementedException();
        }
    }
}
