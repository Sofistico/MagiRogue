using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Time
{
    public class PlayerTimeNode : ITimeNode
    {
        public PlayerTimeNode(long tick)
        {
            Tick = tick;
        }

        public long Tick { get; }
    }
}