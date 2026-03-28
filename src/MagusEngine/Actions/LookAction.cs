using Arquimedes.Enumerators;
using MagusEngine.Actions.Interfaces;
using MagusEngine.Core.Entities;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class LookAction : IExecuteAction
    {
        public bool Execute(Universe world)
        {
            var getPlayer = Find.ControlledEntity!;
            var targetCursor = world!.CurrentMap!.TargetCursor ??= new Target(getPlayer.Position);

            if (targetCursor.State == TargetState.LookMode)
            {
                targetCursor.EndTargetting();
            }
            else
            {
                targetCursor.StartTargetting();
            }

            return true;
        }
    }
}
