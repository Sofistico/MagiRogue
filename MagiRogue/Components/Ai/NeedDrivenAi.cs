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
        public IObjectWithComponents? Parent { get; set; }

        public (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog)
        {
            if (Parent.GoRogueComponents.Contains(typeof(NeedCollection)) && Parent is Actor actor)
            {
                var needs = actor.GetComponent<NeedCollection>();
                messageLog.PrintMessage("The needs are:");
                foreach (var item in needs)
                {
                    messageLog.PrintMessage(item.ToString());
                }
                return (true, 100);
            }
            return (false, -1);
        }
    }
}
