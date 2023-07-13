using MagiRogue.Services;
using System;
using System.Collections.Generic;

namespace MagiRogue
{
    public static class Locator
    {
        private static readonly Dictionary<Type, IService> _services = new();

        public static IService GetService(Type serviceType)
        {
            return _services[serviceType];
        }

        public static IService GetService<T>() where T : IService
        {
            return GetService(typeof(T));
        }

        public static bool AddService<T>(T instance) where T : IService
        {
            var type = typeof(T);
            return _services.TryAdd(type, instance);
        }
    }
}
