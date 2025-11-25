using MagusEngine;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class ThrowItemAction : IExecuteAction
    {
        private Target? _targetCursor;
        private readonly MessageBusService _bus;

        public ThrowItemAction()
        {
            _bus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            Actor? controlledActor = (Actor?)world?.CurrentMap?.ControlledEntitiy;
            if (controlledActor is null)
                return false;
            _bus.SendMessage<>
            ui.InventoryScreen.ShowItems(controlledActor, item =>
            {
                _targetCursor ??= new Target(controlledActor.Position);
                if (item is null)
                {
                    _bus.SendMessage<AddMessageLog>(new("No item selected!"));
                    return;
                }
                _targetCursor.OnSelectItem(item, controlledActor);
                ui.InventoryScreen.Hide();
                controlledActor.Inventory.Remove(item);
            });
            return true;
        }
    }
}
