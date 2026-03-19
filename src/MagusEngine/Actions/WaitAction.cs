using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.MapBus;
using MagusEngine.Services;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class WaitAction : IExecuteAction
    {
        private readonly int _time;
        private readonly MessageBusService _bus;

        public WaitAction(int time)
        {
            _bus = Locator.GetService<MessageBusService>();
            _time = time;
        }

        public bool Execute(Universe world)
        {
            Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(_time, true));

            return true;
        }
    }
}
