using MagusEngine.ECS.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MagusEngine.ECS
{
    // will be removed, too much bugs
    public sealed class EntityRegistry
    {
        private readonly Dictionary<Type, IComponentStore> _data = [];
        private readonly uint _maxComponents;

        public EntityRegistry(uint maxComponents)
        {
            _maxComponents = maxComponents;
        }

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

        public void Destroy(uint entity)
        {
            foreach (var store in _data.Values)
                store.RemoveIfContains(entity);
        }

        // TODO: This method probably won't work and is wrong, will need heavy testing and refactoring and reflection as well
        public List<dynamic> GetAllContains(uint entity)
        {
            List<dynamic> list = [];
            foreach (var store in _data.Values)
                list.AddRange(store.GetIfContains(entity));
            return list;
        }

        public ref T GetComponent<T>(uint obj) => ref Assure<T>().Get(obj);

        public bool TryGetComponent<T>(uint entity, out T? component)
        {
            var store = Assure<T>();
            if (store.Contains(entity))
            {
                component = store.Get(entity);
                return true;
            }
            component = default;
            return false;
        }

        public bool Contains<T>(uint entity) where T : class
        {
            var store = Assure<T>();
            return store.Contains(entity);
        }

        public void RemoveComponent<T>(uint entity) => Assure<T>().RemoveIfContains(entity);

        public CompView<T> CompView<T>() => new(this);

        public CompView<T, U> CompView<T, U>() => new(this);

        public CompView<T, U, V> CompView<T, U, V>() => new(this);

        public void RemoveComponentAll()
        {
            _data.Clear();
        }
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
