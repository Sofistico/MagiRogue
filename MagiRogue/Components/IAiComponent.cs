using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.System;
using MagiRogue.UI;
using Troschuetz.Random;
using GoRogue.GameFramework.Components;

namespace MagiRogue.Components
{
    public interface IAiComponent : IGameObjectComponent
    {
        (bool sucess, long ticks) RunAi(Map map, MessageLogWindow messageLog);
    }
}