using MagusEngine.Actions.Interfaces;
using MagusEngine.Systems;

namespace Diviner.KeyboardActions
{
    public class EscapeMenuAction : IExecuteAction
    {
        public bool Execute(UIManager ui, Universe world)
        {
            if (!ui.NoPopWindow)
                return false;
            ui.MainMenu.Show();
            ui.MainMenu.IsFocused = true;
            return true;
        }
    }
}
