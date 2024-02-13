namespace MagusEngine.Systems.Time
{
    public class PlayerTimeNode : ITimeNode
    {
        public long Tick { get; }
        public uint Id { get; }

        public PlayerTimeNode(long tick, uint id)
        {
            Tick = tick;
            Id = id;
        }
    }
}
