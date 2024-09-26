namespace MagusEngine.Systems.Time
{
    public interface ITimeSystem
    {
        void RegisterNode(ITimeNode node);

        void RemoveNode(ITimeNode node);

        ITimeNode? NextNode();
    }
}