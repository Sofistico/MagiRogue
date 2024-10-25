using MagusEngine.Systems.Time;

namespace MagusEngine.Bus.MapBus
{
    public class AddTurnNode
    {
        public ITimeNode Node { get; set; }
        public long Tick { get; set; }

        public AddTurnNode(ITimeNode node, long tick = 0)
        {
            Node = node;
            Tick = tick;
        }
    }
}
