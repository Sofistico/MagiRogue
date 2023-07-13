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
    }
}
