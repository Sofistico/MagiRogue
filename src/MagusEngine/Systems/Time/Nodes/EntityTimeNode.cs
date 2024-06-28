namespace MagusEngine.Systems.Time.Nodes
{
    public class EntityTimeNode : ITimeNode
    {
        public long Tick { get; }

        public uint Id { get; }

        public EntityTimeNode(uint entityId, long tick)
        {
            Tick = tick;
            Id = entityId;
        }
    }
}
