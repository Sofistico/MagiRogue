using GoRogue.GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MagiRogue.GameSys
{
    public sealed class EntityRegistry
    {
        private readonly Dictionary<Type, IComponentStore> _data = new();
        private readonly uint _maxComponents;

        public EntityRegistry(uint maxComponents)
        {
            _maxComponents = maxComponents;
        }

        public void AddComponent<T>(IGameObject obj, T component) => Assure<T>().Add(obj.ID, component);

        public void AddComponent<T>(uint id, T component) => Assure<T>().Add(id, component);

        public ComponentStore<T> Assure<T>()
        {
            Type type = typeof(T);
            if (_data.TryGetValue(type, out var store))
                return (ComponentStore<T>)store;

            var newStore = new ComponentStore<T>(_maxComponents);
            _data.Add(type, newStore);
            return newStore;
        }

        public void Destroy(IGameObject entity)
        {
            foreach (var store in _data.Values)
                store.RemoveIfContains(entity.ID);
        }

        public ref T GetComponent<T>(IGameObject obj) => ref Assure<T>().Get(obj.ID);

        public bool TryGetComponent<T>(IGameObject entity, ref T component)
        {
            var store = Assure<T>();
            if (store.Contains(entity.ID))
            {
                component = store.Get(entity.ID);
                return true;
            }

            return false;
        }

        public void RemoveComponent<T>(IGameObject entity) => Assure<T>().RemoveIfContains(entity.ID);

        public CompView<T> CompView<T>() => new CompView<T>(this);

        public CompView<T, U> CompView<T, U>() => new CompView<T, U>(this);

        public CompView<T, U, V> CompView<T, U, V>() => new CompView<T, U, V>(this);

        public void RemoveComponentAll()
        {
            _data.Clear();
        }
    }

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

    public class SparseSet : IEnumerable
    {
        private int size;
        private readonly uint[] dense;
        private readonly int[] sparse;

        public int Count => size;

        public SparseSet(uint maxValue)
        {
            size = 0;
            dense = new uint[maxValue];
            sparse = new int[maxValue];
        }

        public void Add(uint value)
        {
            if (!Contains(value))
            {
                dense[size] = value;
                sparse[value] = size;
                size++;
            }
        }

        public void Remove(uint value)
        {
            if (Contains(value))
            {
                dense[sparse[value]] = dense[size - 1];
                sparse[dense[size - 1]] = sparse[value];
                size--;
            }
        }

        public int Index(uint value) => sparse[value];

        public bool Contains(uint value)
        {
            return sparse[value] < size && dense[sparse[value]] == value;
        }

        public void Clear() => size = 0;

        public IEnumerator<uint> GetEnumerator()
        {
            for (var i = 0; i < size; i++)
            {
                yield return dense[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override bool Equals(object obj) => throw new Exception("Why are you comparing SparseSets?");

        public override int GetHashCode() => System.HashCode.Combine(size, dense, sparse, Count);
    }

    public interface IComponentStore
    {
        void RemoveIfContains(uint entityId);
    }

    #region views

    public readonly struct CompView<T> : IEnumerable<uint>
    {
        private readonly EntityRegistry registry;

        public CompView(EntityRegistry registry) => this.registry = registry;

        public IEnumerator<uint> GetEnumerator() => registry.Assure<T>().Set.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public readonly struct CompView<T, U> : IEnumerable<uint>
    {
        private readonly EntityRegistry registry;

        public CompView(EntityRegistry registry) => this.registry = registry;

        public IEnumerator<uint> GetEnumerator()
        {
            var store2 = registry.Assure<U>();
            foreach (var entity in registry.Assure<T>().Set)
            {
                if (!store2.Contains(entity)) continue;
                yield return entity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public readonly struct CompView<T, U, V> : IEnumerable<uint>
    {
        private readonly EntityRegistry registry;

        public CompView(EntityRegistry registry) => this.registry = registry;

        public IEnumerator<uint> GetEnumerator()
        {
            var store2 = registry.Assure<U>();
            var store3 = registry.Assure<V>();
            foreach (var entity in registry.Assure<T>().Set)
            {
                if (!store2.Contains(entity) || !store3.Contains(entity)) continue;
                yield return entity;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion views
}