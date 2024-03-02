using System;

namespace MagusEngine.Systems.Time.Nodes
{
    public class ComponentTimeNode : ITimeNode
    {
        public long Tick { get; }
        public uint Id { get; }
        public Func<long> Action { get; }

        public ComponentTimeNode(long tick, uint id, Func<long> action)
        {
            Tick = tick;
            Id = id;
            Action = action;
        }
    }
}
