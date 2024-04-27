using Arquimedes.Interfaces;
using Diviner.Controls;
using MagusEngine;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
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
        protected readonly Dictionary<char, object> _hotKeys = [];

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

        /// <summary>
        /// This creates a base pop window, keep in mind to create a console in the right place the
        /// math is Width - ButtonWidth - 3, Height - 4, for the position = ButtonWidth + 2, 1
        /// </summary>
        /// <param name="title"></param>
        public PopWindow(string? title) : this(100, 20, title)
        {
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

        protected List<MagiButton> BuildHotKeysButtons<T>(List<T> listItems, 
            Action<T> action,
            Func<T, bool>? isEnabledFunc = null) where T : INamed
        {
            _hotKeys.Clear();

            int yCount = 1;

            var orderedItems = listItems.OrderBy(i => i.Name).ToArray();
            var controlList = new List<MagiButton>(orderedItems.Length);
            for (int i = 0; i < orderedItems.Length; i++)
            {
                var hotkeyLetter = (char)(96 + yCount);
                var item = orderedItems[i];
                _hotKeys.Add(hotkeyLetter, item);
                var button = new MagiButton(ButtonWidth - 2)
                {
                    Text = $"{hotkeyLetter}. {item.Name}",
                    Position = new Point(1, yCount++),
                    IsEnabled = isEnabledFunc?.Invoke(item) ?? true,
                    Action = () => action(item)
                };
                button.Click += (_, __) => button.Action.Invoke();
                button.Focused += (_, __) => button.Action.Invoke();

                controlList.Add(button);
            }

            for (int i = 1; i < controlList.Count; i++)
            {
                controlList[i - 1].NextSelection = controlList[i];
                controlList[i].PreviousSelection = controlList[i - 1];
            }

            return controlList;
        }
    }
}
