using Arquimedes.Enumerators;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Services;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class PlanetMoveCameraAction : IExecuteAction
    {
        private readonly Point _delta;
        private readonly MessageBusService _bus;

        public PlanetMoveCameraAction(Point delta)
        {
            _delta = delta;
            _bus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            if (!CurrentMapIsPlanetView(world))
                return false;
            _bus.SendMessage<ScrollConsoleMessage>(new(_delta, WindowTag.Map));
            // Must return false, because there isn't any movement of the actor
            return false;
        }

        private static bool CurrentMapIsPlanetView(Universe world) =>
            world.WorldMap != null && world.WorldMap.AssocietatedMap == world.CurrentMap && world.Player == null;
    }
}
