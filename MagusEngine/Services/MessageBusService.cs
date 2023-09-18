﻿using GoRogue.Messaging;
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
            _messageBus.UnregisterAllSubscribers(subscriber);
        }

        public void UnRegisterSubscriber<T>(ISubscriber<T> subscriber)
        {
            _messageBus.UnregisterSubscriber(subscriber);
        }
    }
}
