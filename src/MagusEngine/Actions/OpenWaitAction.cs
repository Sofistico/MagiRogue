using Arquimedes.Enumerators;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Services;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class OpenWaitAction : IExecuteAction
    {
        private readonly MessageBusService _messageBus;

        public OpenWaitAction()
        {
            _messageBus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            _messageBus.SendMessage<OpenWindowEvent>(new(WindowTag.Wait));
            return true;
        }
    }
}
