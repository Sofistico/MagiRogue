using MagusEngine.Actions.Interfaces;
using MagusEngine.Core.Entities;
using MagusEngine.Systems;

namespace Diviner.KeyboardActions
{
    public class OpenInventoryAction : IExecuteAction
    {
        public bool Execute(UIManager ui, Universe world)
        {
            Actor? controlledActor = (Actor?)world?.CurrentMap?.ControlledEntitiy;
            if (controlledActor is null)
                return false;
            ui.InventoryScreen.ShowItems(controlledActor);
            return true;
        }
    }
}
