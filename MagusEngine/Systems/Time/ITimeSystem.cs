namespace MagusEngine.Systems.Time
{
    public interface ITimeSystem
    {
        void RegisterEntity(ITimeNode node);

        void DeRegisterEntity(ITimeNode node);

        ITimeNode? NextNode();
    }
}