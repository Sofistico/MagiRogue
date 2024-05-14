using System.Collections.Generic;

namespace MagusEngine.ECS.Interfaces
{
    public interface IComponentStore
    {
        void RemoveIfContains(uint entityId);

        List<dynamic> GetIfContains(uint entityId);
    }
}