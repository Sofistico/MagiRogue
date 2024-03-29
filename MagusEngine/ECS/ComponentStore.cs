﻿using GoRogue.Messaging;
using MagusEngine.Bus.ComponentBus;
using MagusEngine.ECS.Interfaces;
using MagusEngine.Services;

namespace MagusEngine.ECS
{
    public class ComponentStore<T> : IComponentStore,
        ISubscriber<ComponentAddedCommand<T>>,
        ISubscriber<ComponentRemovedCommand>
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
            Locator.GetService<MessageBusService>().RegisterAllSubscriber(this);
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

        public void Handle(ComponentAddedCommand<T> message)
        {
            if (initialized)
                Add(message.Id, message.Component);
        }

        public void Handle(ComponentRemovedCommand message)
        {
            if (initialized)
                Remove(message.Id);
        }

        ~ComponentStore()
        {
            Locator.GetService<MessageBusService>().UnRegisterAllSubscriber(this);
        }
    }
}
