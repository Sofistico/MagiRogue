using System;

namespace MagusEngine.Systems.Time.Nodes
{
    public class ComponentNode : ITimeNode
    {
        public long Tick { get; }
        public uint Id { get; }
        public Func<uint> Action { get; }

        public ComponentNode(long tick, uint id, Func<uint> action)
        {
            Tick = tick;
            Id = id;
            Action = action;
        }
    }
}
