using MagusEngine.Actions.Interfaces;
using MagusEngine.Core.Entities;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class LookAction : IExecuteAction
    {
        public bool Execute(Universe world)
        {
            _targetCursor ??= new Target(_getPlayer.Position);

            if (_targetCursor.State == TargetState.LookMode)
            {
                _targetCursor.EndTargetting();
            }
            else
            {
                _targetCursor.StartTargetting();
            }

            return true;
        }
    }
}
