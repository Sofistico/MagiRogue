using GoRogue.Messaging;
using MagiRogue.Entities;
using SadConsole;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using Console = SadConsole.Console;

namespace MagiRogue.UI.Windows
{
    public class StatusWindow : MagiBaseWindow
    {
        private readonly Player player;
        private readonly Console statsConsole;
        private readonly ScrollBar statusScroll;

        private const int windowBorderThickness = 2;

        public StatusWindow(int width, int heigth, string title) : base(width, heigth, title)
        {
            player = GameLoop.Universe.Player;

            statsConsole = new Console(width - windowBorderThickness, heigth - windowBorderThickness)
            {
                Position = new Point(1, 1),
                View = new Rectangle(0, 0, width - 1, heigth - windowBorderThickness),
                DefaultBackground = Color.Black
            };

            // enable mouse input
            UseMouse = false;

            Children.Add(statsConsole);
        }

        /*private void StatusScroll_ValueChanged(object? sender, EventArgs? e)
        {
            statsConsole.View = new Rectangle(0, statusScroll.Value + windowBorderThickness,
                statsConsole.Width, statsConsole.View.Height);
        }*/

        // Probably needs to create a way to make it update only when needed, by an event.
        public override void Update(TimeSpan time)
        {
            statsConsole.Clear();
            statsConsole.Print(0, 0, $"{player.Name}");
            statsConsole.Print(2, 0, ColoredString.Parser.Parse(player.GetStaminaStatus()));
            statsConsole.Print(3, 0, ColoredString.Parser.Parse(player.GetManaStatus()));

            base.Update(time);
        }

        public void ChangePositionToBottomPage()
        {
            Position = new Point(0, GameLoop.GameHeight - 3);
            BorderLineStyle = ICellSurface.ConnectedLineEmpty;
        }

        public void ChangePositionToUpMessageLog()
        {
            Position = new Point(0, GameLoop.GameHeight - 12);
            BorderLineStyle = ICellSurface.ConnectedLineThin;
        }
    }
}