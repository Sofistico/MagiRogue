using MagusEngine.Systems.Time;

namespace MagusEngine.Bus.MapBus
{
    public class AddTurnNode
    {
        public ITimeNode Node { get; set; }

        public AddTurnNode(ITimeNode node)
        {
            Node = node;
        }
    }
}
