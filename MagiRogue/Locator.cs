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

        public static T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public static void AddService<T>(T instance) where T : IService
        {
            var type = typeof(T);
            _services[type] = instance;
        }

        public static void InitializeCommonServices()
        {
            AddService<MessageBusService>(new());
        }
    }
}
