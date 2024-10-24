﻿using MagusEngine.ECS.Interfaces;
using System.Collections.Generic;

namespace MagusEngine.ECS
{
    public class ComponentStore<T> : IComponentStore
    {
        private readonly T[] instances;
        private readonly bool initialized;

        public SparseSet Set { get; }
        public int Count => Set.Count;

        public ComponentStore(uint maxComponents)
        {
            Set = new SparseSet(maxComponents);
            instances = new T[maxComponents];
            initialized = true;
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

        public List<dynamic> GetIfContains(uint entityId)
        {
            List<dynamic> list = [];
            if (Contains(entityId)) list.Add(Get(entityId));
            return list;
        }

        private void Remove(uint entityId) => Set.Remove(entityId);
    }
}
