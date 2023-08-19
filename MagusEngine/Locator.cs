using MagusEngine.Services;
using SadRogue.Primitives;

namespace MagusEngine
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

        public static void AddService(Type type)
        {
            if (!type.IsAbstract && !type.IsSealed)
                throw new ArgumentNullException(nameof(type));
            _services[type] = type.GetConstructor(Array.Empty<Type>())!;
        }

        public static void InitializeSingletonServices()
        {
            AddService<MessageBusService>(new());
            AddService<IDGenerator>(new());
            AddService<SavingService>(new());
            AddService(typeof(MagiLog));
        }
    }
}
