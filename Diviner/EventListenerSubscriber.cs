using MagusEngine;
using MagusEngine.Services;

namespace Diviner
{
    public class EventListenerSubscriber
    {
        private readonly MessageBusService messageBusService;

        public EventListenerSubscriber()
        {
            messageBusService = Locator.GetService<MessageBusService>();
        }

        public void RegisterListeners()
        {
        }
    }
}
