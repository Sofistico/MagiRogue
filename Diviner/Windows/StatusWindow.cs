using Arquimedes.Settings;
using GoRogue.Messaging;
using MagusEngine;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Systems;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace Diviner.Windows
{
    public class StatusWindow : MagiBaseWindow,
        ISubscriber<HideMessageLogMessage>,
        ISubscriber<AddMessageLog>
    {
        private readonly Player? player;
        private readonly Console statsConsole;

        private const int windowBorderThickness = 2;

        public StatusWindow(int width, int heigth, string title) : base(width, heigth, title)
        {
            player = Find.Universe.Player;

            statsConsole = new Console(width - windowBorderThickness, heigth - windowBorderThickness)
            {
                Position = new Point(1, 1),
            };
            statsConsole.Surface.View = new Rectangle(0, 0, width - 1, heigth - windowBorderThickness);
            statsConsole.Surface.DefaultBackground = Color.Black;
            // enable mouse input
            UseMouse = false;

            Children.Add(statsConsole);
        }

        // Probably needs to create a way to make it update only when needed, by an event.
        public override void Update(TimeSpan time)
        {
            int xOffSet = 0;
            statsConsole.Clear();
            xOffSet += player.Name.Length;
            statsConsole.Print(xOffSet, 0, player.Name);
            xOffSet += player.GetStaminaStatus().Length;
            statsConsole.Print(xOffSet, 0, ColoredString.Parser.Parse(player.GetStaminaStatus()));
            xOffSet += player.GetManaStatus().Length;
            statsConsole.Print(xOffSet, 0, ColoredString.Parser.Parse(player.GetManaStatus()));

            base.Update(time);
        }

        public void ChangePositionToBottomPage()
        {
            Position = new Point(1, Locator.GetService<GlobalSettings>().ScreenHeight - 3);
        }

        public void ChangePositionToUpMessageLog()
        {
            Position = new Point(1, Locator.GetService<GlobalSettings>().ScreenHeight - 12);
        }

        public void Handle(HideMessageLogMessage message)
        {
            ChangePositionToBottomPage();
        }

        public void Handle(AddMessageLog message)
        {
            ChangePositionToUpMessageLog();
        }
    }
}