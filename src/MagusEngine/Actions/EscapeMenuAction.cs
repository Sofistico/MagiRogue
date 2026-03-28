using Arquimedes.Enumerators;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Services;
using MagusEngine.Systems;
using System.Net.NetworkInformation;

namespace MagusEngine.Actions
{
    public class EscapeMenuAction : IExecuteAction
    {
        private readonly MessageBusService _bus;

        public EscapeMenuAction()
        {
            _bus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            var targetCursor = world.CurrentMap.TargetCursor;
            if (targetCursor is not null)
            {
                targetCursor.EndTargetting();
                return true;
            }
            _bus.SendMessage<ShowMainMenuMessage>();
            return true;
        }
    }
}
