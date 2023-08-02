using SadConsole;
using SadConsole.UI.Controls;

namespace Diviner.Windows
{
    /// <summary>
    /// This creates a base pop window, keep in mind to create a console in the right place the math is
    /// Width - ButtonWidth - 3, Height - 4, for the position = ButtonWidth + 2, 1
    /// </summary>
    public class PopWindow : MagiBaseWindow
    {
        public const int ButtonWidth = 40;

        private readonly Button _cancelButton;

        protected readonly Console DescriptionArea;

        /// <summary>
        /// This creates a base pop window, keep in mind to create a console in the right place the math is
        /// Width - ButtonWidth - 3, Height - 4, for the position = ButtonWidth + 2, 1
        /// </summary>
        /// <param name="title"></param>
        public PopWindow(string title) : base(100, 20, title)
        {
            GameLoop.UIManager.NoPopWindow = false;

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

            DescriptionArea = new Console(Width - ButtonWidth - 3, Height - 4)
            {
                Position = new Point(ButtonWidth + 2, 1)
            };
            DescriptionArea.FillWithRandomGarbage(10);

            Children.Add(DescriptionArea);
        }

        public PopWindow(int width, int height, string title) : base(width, height, title)
        {
            GameLoop.UIManager.NoPopWindow = false;

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

            DescriptionArea = new Console(Width - 3, Height - 4)
            {
                Position = new Point(Width / 2, 1)
            };
            DescriptionArea.IsVisible = true;
            DescriptionArea.FillWithRandomGarbage(10);
            Children.Add(DescriptionArea);
        }

        public override void Hide()
        {
            GameLoop.UIManager.NoPopWindow = true;
            GameLoop.UIManager.IsFocused = true;

            base.Hide();
        }

        public override void Show(bool modal)
        {
            GameLoop.UIManager.NoPopWindow = false;
            IsFocused = true;

            DescriptionArea.Clear();

            base.Show(modal);
        }

        public void PrintConsole(Point pos, string text) => DescriptionArea.Print(pos.X, pos.Y, text);

        public void PrintConsole(int x, int y, string text) => DescriptionArea.Print(x, y, text);
    }
}