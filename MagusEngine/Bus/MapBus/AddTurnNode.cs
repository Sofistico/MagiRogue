using MagusEngine.Systems.Time;

namespace MagusEngine.Bus.MapBus
{
    public class AddTurnNode<T> where T : ITimeNode
    {
        public T Node { get; set; }

        public AddTurnNode(T node)
        {
            Node = node;
        }
    }
}
