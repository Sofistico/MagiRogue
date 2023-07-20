using MagiRogue.Services;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagiRogue
{
    public static class Locator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static object GetService(Type serviceType)
        {
            return _services[serviceType];
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

        public static void InitializeCommonServices()
        {
            AddService<MessageBusService>(new());
            AddService<IDGenerator>(new());
        }
    }
}
