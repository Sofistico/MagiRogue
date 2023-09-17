using MagusEngine.Services;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagusEngine
{
    public static class Locator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static object GetService(Type serviceType)
        {
            _services.TryGetValue(serviceType, out var obj);
            return obj;
        }

        public static T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public static void AddService<T>(T instance)
        {
            var type = typeof(T);
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            _services[type] = instance;
        }

        public static void InitializeSingletonServices()
        {
            AddService<MessageBusService>(new());
            AddService<IDGenerator>(new());
            AddService<SavingService>(new());
            AddService(new MagiLog());
        }
    }
}
