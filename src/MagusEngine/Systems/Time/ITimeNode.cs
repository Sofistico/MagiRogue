namespace MagusEngine.Systems.Time
{
    public interface ITimeNode
    {
        public long Tick { get; }
        public uint Id { get; }
    }
}