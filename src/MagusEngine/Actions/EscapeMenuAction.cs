using MagusEngine.Actions.Interfaces;
using MagusEngine.Bus.UiBus;
using MagusEngine.Services;
using MagusEngine.Systems;

namespace MagusEngine.Actions
{
    public class EscapeMenuAction : IExecuteAction
    {
        private readonly MessageBusService _bus;

        public EscapeMenuAction()
        {
            _bus = Locator.GetService<MessageBusService>();
        }

        public bool Execute(Universe world)
        {
            _bus.SendMessage<ShowMainMenuMessage>();
            // if (!ui.NoPopWindow)
            //     return false;
            // ui.MainMenu.Show();
            // ui.MainMenu.IsFocused = true;
            return true;
        }
    }
}
