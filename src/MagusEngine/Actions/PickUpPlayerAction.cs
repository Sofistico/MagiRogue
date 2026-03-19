using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.MapBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;

namespace MagusEngine.Actions
{
    public class PickUpPlayerAction : IExecuteAction
    {
        private readonly Actor _player;
        private readonly MessageBusService _bus;

        public PickUpPlayerAction(Actor player)
        {
            _player = player;
            _bus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            Item item = world.CurrentMap!.GetEntityAt<Item>(world.CurrentMap.ControlledEntitiy!.Position)!;
            bool sucess = ActionManager.PickUp(world.Player, item!);
            _bus.SendMessage<ProcessTurnEvent>(new(TimeHelper.Interact, sucess));
            return sucess;
        }
    }
}
