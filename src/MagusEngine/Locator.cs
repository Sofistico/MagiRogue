using MagusEngine.Services;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagusEngine
{
    public static class Locator
    {
        private static readonly Dictionary<Type, object> _services = InitializeSingletonServices();

        public static object? GetService(Type serviceType)
        {
            _services.TryGetValue(serviceType, out var obj);
            return obj;
        }

        public static T GetService<T>()
        {
            var result = (T?)GetService(typeof(T));
            return result is null ? throw new ArgumentNullException("Trying to pass a service as null") : result;
        }

        public static void AddService<T>(T instance)
        {
            var type = typeof(T);
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            _services[type] = instance;
        }

        private static Dictionary<Type, object> InitializeSingletonServices()
        {
            return new()
            {
                { typeof(MessageBusService), new MessageBusService() },
                { typeof(IDGenerator), new IDGenerator() },
                { typeof(SavingService), new SavingService() },
                { typeof(MagiLog), new MagiLog() },
                // { typeof(EntityRegistry), new EntityRegistry(100000u) },
            };
        }
    }
}
