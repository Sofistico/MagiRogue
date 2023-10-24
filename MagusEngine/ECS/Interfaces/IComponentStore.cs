namespace MagusEngine.ECS.Interfaces
{
    public interface IComponentStore
    {
        void RemoveIfContains(uint entityId);
    }
}