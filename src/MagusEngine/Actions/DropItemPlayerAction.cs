using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;

namespace MagusEngine.Actions
{
    public class DropItemPlayerAction : IExecuteAction
    {
        private readonly MessageBusService _bus;

        public DropItemPlayerAction()
        {
            _bus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            var controlledActor = (Actor)world!.CurrentMap!.ControlledEntitiy!;
            _bus.SendMessage<InventoryActionBus>(new(controlledActor, item =>
            {
                var sucess = ActionManager.DropItem(item, controlledActor.Position, world.CurrentMap);
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.Interact, sucess));
            }));
            return false;
        }
    }
}
