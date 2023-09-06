using GoRogue.Messaging;
using System;

namespace MagusEngine.Services
{
    public class MessageBusService
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

        public void SendMessage<T>(T obj) where T : notnull
        {
            _messageBus.Send(obj);
        }

        public void SendMessage<T>() where T : new()
        {
            var instance = Activator.CreateInstance(typeof(T));
            _messageBus?.Send((T)instance);
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
