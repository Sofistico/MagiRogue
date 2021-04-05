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

namespace MagiRogue.UI.PopUpWindow
{
    public class PopConsole
    {
        public static DynamicContextMenu CurrentMenu { get; set; }
        public static bool IsOpen => CurrentMenu != null;

        public PopConsole()
        {
        }

        public static void ShowMenu(Point location, List<(string, Action)> items, string title)
        {
            if (CurrentMenu != null)
            {
                CurrentMenu.Hide();
                CurrentMenu = null;
                return;
            }
            CurrentMenu = new DynamicContextMenu(items.Max(item => item.Item1.Length) + 4, items.Count + 2, title, items)
            {
                Position = location
            };
            CurrentMenu.Show();
        }

        public class DynamicContextMenu : MagiBaseWindow
        {
            private List<char> alphabet = Enumerable.Range('a', 27).Select(x => (char)x).ToList();

            internal DynamicContextMenu(int width, int height, string title, List<(string, Action)> items)
                : base(width, height, title)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    (string text, Action action) = items[i];

                    StringBuilder str = new StringBuilder(alphabet[i].ToString());

                    str.Append(" - ");
                    str.Append(text);

                    text = str.ToString();

                    var button = new Button(width - 2)
                    {
                        Text = text,
                        Position = new Point(1, i + 1)
                    };

                    button.Click += (sender, e) =>
                    {
                        action();
                        CurrentMenu = null;
                        Hide();
                    };

                    Add(button);
                }

                Closed += DynamicContextMenu_Closed;
            }

            private void DynamicContextMenu_Closed(object sender, EventArgs e)
            {
                if (CurrentMenu == this)
                {
                    CurrentMenu = null;
                    Hide();
                }
            }

            public override bool ProcessMouse(MouseConsoleState state)
            {
                if (state.IsOnConsole && state.Mouse.LeftClicked)
                {
                    base.ProcessMouse(state);
                    return true;
                }

                return base.ProcessMouse(state);
            }
        }
    }
}