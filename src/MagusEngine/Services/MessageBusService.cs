using GoRogue.Messaging;
using System;
using System.Diagnostics;

namespace MagusEngine.Services
{
    public class MessageBusService
    {
        private readonly MessageBus _messageBus;

        public MessageBusService()
        {
            _messageBus = new();
        }

        [DebuggerStepThrough]
        public void SendMessage(object obj)
        {
            _messageBus.Send(obj);
        }

        [DebuggerStepThrough]
        public void SendMessage<T>(T obj) where T : notnull
        {
            _messageBus.Send(obj);
        }

        [DebuggerStepThrough]
        public void SendMessage<T>() where T : notnull, new()
        {
            var instance = Activator.CreateInstance(typeof(T));
            _messageBus?.Send((T)instance!);
        }

        /// <summary>
        /// <see cref="MessageBus.RegisterSubscriber"></see>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscriber"></param>
        public void RegisterSubscriber<T>(ISubscriber<T> subscriber)
        {
            _messageBus.RegisterSubscriber(subscriber);
        }

        public void RegisterAllSubscriber<T>(T subscriber) where T : notnull
        {
            _messageBus.RegisterAllSubscribers(subscriber);
        }

        public void UnRegisterAllSubscriber<T>(T subscriber) where T : notnull
        {
            try
            {
                _messageBus?.UnregisterAllSubscribers(subscriber);
            }
            catch (ArgumentException)
            {
                // ignore this error
            }
        }

        public void UnRegisterSubscriber<T>(ISubscriber<T> subscriber)
        {
            _messageBus.UnregisterSubscriber(subscriber);
        }
    }
}
