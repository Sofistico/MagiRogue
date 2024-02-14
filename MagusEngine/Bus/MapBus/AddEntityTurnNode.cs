namespace MagusEngine.Bus.MapBus
{
    public class AddEntityTurnNode
    {
        public long Ticks { get; set; }
        public uint EntityId { get; set; }

        public AddEntityTurnNode(long ticks, uint id)
        {
            Ticks = ticks;
            EntityId = id;
        }
    }
}
