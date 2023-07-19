using GoRogue.Messaging;

namespace MagiRogue.Services
{
    public class MessageBusService : IService
    {
        private readonly MessageBus _messageBus;

        public MessageBusService()
        {
            _messageBus = new();
        }

        public void SendMessage(object obj)
        {
            _messageBus.Send(obj);
        }

        public void RegisterSubscriber<T>(ISubscriber<T> subscriber)
        {
            _messageBus.RegisterSubscriber(subscriber);
        }

        public void UnRegisterSubscriber<T>(ISubscriber<T> subscriber)
        {
            _messageBus.UnregisterSubscriber(subscriber);
        }
    }
}
