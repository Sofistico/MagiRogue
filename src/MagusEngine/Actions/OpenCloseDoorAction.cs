using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;

namespace MagusEngine.Actions
{
    public class OpenCloseDoorAction : IExecuteAction
    {
        private readonly MessageBusService _bus;
        private readonly Actor _actor;
        private readonly bool _close;

        public OpenCloseDoorAction(Actor actor, bool close)
        {
            _bus = Locator.GetService<MessageBusService>();
            _actor = actor;
            _close = close;
        }

        public bool Execute(Universe world)
        {
            if (_close)
            {
                bool sucess = ActionManager.CloseDoor(world.Player);
                _bus.SendMessage<ProcessTurnEvent>(new(TimeHelper.Interact, sucess));
                _bus.SendMessage<MapConsoleIsDirty>();
                return sucess;
            }
            else
            {
                return false;
            }
        }
    }
}
