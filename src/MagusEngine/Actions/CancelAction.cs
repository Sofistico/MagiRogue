using MagusEngine.Actions.Interfaces;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class CancelAction : IExecuteAction
    {
        public bool Execute(Universe world)
        {
            var targetCursor = world?.CurrentMap?.TargetCursor;
            targetCursor?.EndTargetting();

            world?.CurrentMap?.TargetCursor = null;

            return true;
        }
    }
}
