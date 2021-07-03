using MagiRogue.Entities;
using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
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
            player = GameLoop.World.Player;

            statsConsole = new Console(width - windowBorderThickness, heigth - windowBorderThickness)
            {
                Position = new Point(1, 1),
                View = new Rectangle(0, 0, width - 1, heigth - windowBorderThickness),
                DefaultBackground = Color.Black
            };

            statusScroll = new ScrollBar
                (Orientation.Vertical, heigth - windowBorderThickness)
            {
                Position = new Point(statsConsole.Width + 1, statsConsole.Position.X)

                //IsEnabled = false
            };
            statusScroll.ValueChanged += StatusScroll_ValueChanged; ;
            Controls.Add(statusScroll);

            // enable mouse input
            UseMouse = true;

            Children.Add(statsConsole);
        }

        private void StatusScroll_ValueChanged(object sender, EventArgs e)
        {
            statsConsole.View = new Rectangle(0, statusScroll.Value + windowBorderThickness,
                statsConsole.Width, statsConsole.View.Height);
        }

        // Probably needs to create a way to make it update only when needed, by an event.
        public override void Update(TimeSpan time)
        {
            statsConsole.Print(0, 0, $"{player.Name}");
            statsConsole.Print(0, 2, $"Health: {(int)player.Stats.Health} / {player.Stats.MaxHealth}   ", Color.Red);
            statsConsole.Print(0, 3, $"Mana: {(int)player.Stats.PersonalMana} / {player.Stats.MaxPersonalMana}", Color.LightBlue);
            statsConsole.Print(0, 5, $"Speed: {player.Stats.Speed}");
            base.Update(time);
        }
    }
}