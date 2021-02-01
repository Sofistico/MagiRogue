using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Time
{
    public class EntityTimeNode : ITimeNode
    {
        public EntityTimeNode(uint entityId, long tick)
        {
            Tick = tick;
            EntityId = entityId;
        }

        public long Tick { get; }

        public uint EntityId { get; init; }
    }
}