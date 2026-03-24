using System;
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
        private readonly Action<Item>? _action;

        public OpenInventoryAction(Action<Item>? action = null)
        {
            _bus = Locator.GetService<MessageBusService>();
            _action = action;
        }

        public bool Execute(Universe world)
        {
            Actor? controlledActor = (Actor?)world?.CurrentMap?.ControlledEntitiy;
            if (controlledActor is null)
                return false;

            _bus.SendMessage<InventoryActionBus>(new(controlledActor, _action));
            return true;
        }
    }
}
