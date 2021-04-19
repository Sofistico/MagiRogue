using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace MagiRogue.UI.Windows
{
    public abstract class PopWindow : MagiBaseWindow
    {
        private const int buttonWidth = 40;

        private readonly Button _cancelButton;
        private readonly Console _popConsole;
        private readonly Dictionary<char, Button> _hotKeys;

        private Action<Button> _onClick;

        public PopWindow(string title) : base(100, 20, title)
        {
            _hotKeys = new Dictionary<char, Button>();

            CloseOnEscKey = true;

            _popConsole = new Console(Width, Height);
            _popConsole.FillWithRandomGarbage();
            Children.Add(_popConsole);

            Center();
            IsFocused = true;
        }
    }
}