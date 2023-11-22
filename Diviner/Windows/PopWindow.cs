using MagusEngine;
using MagusEngine.Bus.UiBus;
using MagusEngine.Services;
using SadConsole;
using SadConsole.UI.Controls;
using Console = SadConsole.Console;

namespace Diviner.Windows
{
    /// <summary>
    /// This creates a base pop window, keep in mind to create a console in the right place the math
    /// is Width - ButtonWidth - 3, Height - 4, for the position = ButtonWidth + 2, 1
    /// </summary>
    public class PopWindow : MagiBaseWindow
    {
        public const int ButtonWidth = 40;

        private readonly Button _cancelButton;

        protected readonly Console _descriptionArea;

        /// <summary>
        /// This creates a base pop window, keep in mind to create a console in the right place the
        /// math is Width - ButtonWidth - 3, Height - 4, for the position = ButtonWidth + 2, 1
        /// </summary>
        /// <param name="title"></param>
        public PopWindow(string? title) : base(100, 20, title)
        {
            //GameLoop.UIManager.NoPopWindow = false;
            Locator.GetService<MessageBusService>().SendMessage<TogglePopWindowMessage>(new(false));

            IsFocused = true;

            CloseOnEscKey = true;

            Center();

            const string cancelButtonText = "Close(Esc)";

            int cancelButtonWidth = cancelButtonText.Length + 4;

            _cancelButton = new Button(cancelButtonWidth)
            {
                Text = cancelButtonText,
                Position = new Point(ButtonWidth + 1, Height - 2)
            };

            _cancelButton.Click += (_, __) => Hide();

            Controls.Add(_cancelButton);

            _descriptionArea = new Console(Width - ButtonWidth - 3, Height - 4)
            {
                Position = new Point(ButtonWidth + 2, 1)
            };

            Children.Add(_descriptionArea);
        }

        public PopWindow(int width, int height, string title) : base(width, height, title)
        {
            //GameLoop.UIManager.NoPopWindow = false;
            Locator.GetService<MessageBusService>().SendMessage<TogglePopWindowMessage>(new(false));

            IsFocused = true;

            CloseOnEscKey = true;

            Center();

            const string cancelButtonText = "Close(Esc)";

            int cancelButtonWidth = cancelButtonText.Length + 4;

            _cancelButton = new Button(cancelButtonWidth)
            {
                Text = cancelButtonText,
                Position = new Point(Width / 2, Height - 2)
            };

            _cancelButton.Click += (_, __) => Hide();

            Controls.Add(_cancelButton);

            _descriptionArea = new Console(Width - 3, Height - 4)
            {
                Position = new Point(Width / 2, 1),
                IsVisible = true
            };
            Children.Add(_descriptionArea);
        }

        public override void Hide()
        {
            //GameLoop.UIManager.NoPopWindow = true;
            Locator.GetService<MessageBusService>().SendMessage<TogglePopWindowMessage>(new(true));
            //GameLoop.UIManager.IsFocused = true;
            Locator.GetService<MessageBusService>().SendMessage<FocusUiManagerMessage>();

            base.Hide();
        }

        public override void Show(bool modal)
        {
            Locator.GetService<MessageBusService>().SendMessage<TogglePopWindowMessage>(new(false));
            IsFocused = true;

            _descriptionArea.Clear();
            _descriptionArea.IsVisible = true;

            base.Show(modal);
        }

        public void PrintConsole(Point pos, string text) => _descriptionArea.Print(pos.X, pos.Y, text);

        public void PrintConsole(int x, int y, string text) => _descriptionArea.Print(x, y, text);
    }
}
