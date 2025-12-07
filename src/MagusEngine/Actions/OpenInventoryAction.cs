using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class OpenInventoryAction : IExecuteAction
    {
        private readonly MessageBusService _bus;

        public OpenInventoryAction()
        {
            _bus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            Actor? controlledActor = (Actor?)world?.CurrentMap?.ControlledEntitiy;
            if (controlledActor is null)
                return false;

            _bus.SendMessage<InventoryActionBus>(new(controlledActor, null));
            return true;
        }
    }
}
