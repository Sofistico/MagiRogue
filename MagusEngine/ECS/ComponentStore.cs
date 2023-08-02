using MagusEngine.ECS.Interfaces;

namespace MagusEngine.ECS
{
    public class ComponentStore<T> : IComponentStore
    {
        private readonly T[] instances;

        public SparseSet Set { get; }
        public int Count => Set.Count;

        public ComponentStore(uint maxComponents)
        {
            Set = new SparseSet(maxComponents);
            instances = new T[maxComponents];
        }

        public void Add(uint entityId, T value)
        {
            Set.Add(entityId);
            instances[Set.Index(entityId)] = value;
        }

        public ref T Get(uint entityId) => ref instances[Set.Index(entityId)];

        public bool Contains(uint entityId) => Set.Contains(entityId);

        public void RemoveIfContains(uint entityId)
        {
            if (Contains(entityId)) Remove(entityId);
        }

        private void Remove(uint entityId) => Set.Remove(entityId);
    }
}