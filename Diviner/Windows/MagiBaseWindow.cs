using CommunityToolkit.HighPerformance;
using Diviner.Controls;
using Diviner.Enums;
using Diviner.Interfaces;
using SadConsole;
using SadConsole.UI;
using System.Diagnostics;

namespace Diviner.Windows
{
    // Will contain relevant custom code here, maybe
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class MagiBaseWindow : Window, IWindowTagContract
    {
        private List<MagiButton>? _selectionButtons;
        private MagiButton? lastFocusedButton;

        /// <summary>
        /// account for the thickness of the window border to prevent UI element spillover
        /// </summary>
        protected const int WindowBorderThickness = 2;

        public WindowTag Tag { get; set; }

        public MagiBaseWindow(int width, int height, string? title) : base(width, height)
        {
            // Ensure that the window background is the correct colour

            /*ThemeColors = GameLoop.UIManager.CustomColors;
            ThemeColors.ControlBack = Color.Black;
            ThemeColors.TitleText = Color.Red;
            ThemeColors.ModalBackground = Color.Black;
            ThemeColors.ControlHostBack = Color.Black;
            ThemeColors.ControlBackSelected = Color.DarkRed;
            ThemeColors.ControlBackLight = Color.LightSlateGray;
            ThemeColors.RebuildAppearances();*/

            // instantiete the inventory of the actor, passing the actor value if and when i implement helpers, to make it
            // possible to see and use their inventory.

            CanDrag = false;

            Title = title!;
        }

        public void AddToDictionary(MagiButton button)
        {
            SetupSelectionButtons(new List<MagiButton>() { button });
        }

        public void SetupSelectionButtons(params MagiButton[] buttons)
        {
            SetupSelectionButtons(buttons.ToList());
        }

        public void SetupSelectionButtons(List<MagiButton> buttonsSelectionAction)
        {
            _selectionButtons ??= new(buttonsSelectionAction);
            if (_selectionButtons.Count < 1)
            {
                return;
            }

            var buttons = buttonsSelectionAction.AsSpan();

            for (int i = 1; i < _selectionButtons.Count; i++)
            {
                buttons[i - 1].NextSelection = buttons[i];
                buttons[i].PreviousSelection = buttons[i - 1];
            }

            buttons[0].PreviousSelection = buttons[_selectionButtons.Count - 1];
            buttons[_selectionButtons.Count - 1].NextSelection = buttons[0];

            foreach (var button in buttons)
            {
                Controls.Add(button);
                button.MouseEnter += (_, __) => Controls.FocusedControl = button;
            }

            if (buttons[0].IsEnabled)
            {
                Controls.FocusedControl = buttons[0];
            }
            else
            {
                buttons[0].SelectNext();
            }
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"Nameof Screen: {Title}";
            }
        }

        public override void Update(TimeSpan time)
        {
            if (Controls.FocusedControl is not MagiButton focusedButton
                || focusedButton == lastFocusedButton)
            {
                base.Update(time);
                return;
            }

            lastFocusedButton = focusedButton;

            base.Update(time);
        }

        protected void PrintUpFromPosition(int x, int y, string text)
        {
            Surface.Print(x, y - 1, text);
        }

        protected void PrintUpFromPosition(Point pos, string text)
        {
            PrintUpFromPosition(pos.X, pos.Y, text);
        }
    }
}
