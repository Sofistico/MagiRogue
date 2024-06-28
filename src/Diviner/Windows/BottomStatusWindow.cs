using Arquimedes.Settings;
using MagusEngine;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using SadConsole;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace Diviner.Windows
{
    public class BottomStatusWindow : MagiBaseWindow
    {
        private readonly Player? player;
        private readonly Console statsConsole;

        private const int windowBorderThickness = 2;

        public BottomStatusWindow(int width, int heigth, string title) : base(width, heigth, title)
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
            Position = new Point(0, Locator.GetService<GlobalSettings>().ScreenHeight - Height);
        }

        ~BottomStatusWindow()
        {
            Locator.GetService<MessageBusService>().UnRegisterAllSubscriber(this);
        }
    }
}
