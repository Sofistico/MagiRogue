using System;

namespace MagusEngine.Systems.Time.Nodes
{
    public class ComponentTimeNode : ITimeNode
    {
        public long Tick { get; }
        public uint Id { get; }
        public Func<uint> Action { get; }

        public ComponentTimeNode(long tick, uint id, Func<uint> action)
        {
            Tick = tick;
            Id = id;
            Action = action;
        }
    }
}
