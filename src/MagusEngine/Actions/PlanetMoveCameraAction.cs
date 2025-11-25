using MagusEngine.Actions.Interfaces;
using MagusEngine.Systems;

namespace Diviner.KeyboardActions
{
    public class PlanetMoveCameraAction : IExecuteAction
    {
        private readonly Point _delta;

        public PlanetMoveCameraAction(Point delta)
        {
            _delta = delta;
        }

        public bool Execute(UIManager ui, Universe world)
        {
            if (!CurrentMapIsPlanetView(world))
                return false;
            var console = ui.MapWindow.MapConsole;

            console.Surface.ViewPosition = console.Surface.ViewPosition.Translate(_delta);
            // Must return false, because there isn't any movement of the actor
            return false;
        }

        private static bool CurrentMapIsPlanetView(Universe world) =>
            world.WorldMap != null && world.WorldMap.AssocietatedMap == world.CurrentMap && world.Player == null;
    }
}
