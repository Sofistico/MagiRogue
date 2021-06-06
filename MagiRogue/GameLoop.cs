using MagiRogue.System;
using MagiRogue.System.Physics;
using MagiRogue.UI;
using Microsoft.Xna.Framework;

namespace MagiRogue
{
    public static class GameLoop
    {
        public const int GameWidth = 120;
        public const int GameHeight = 30;

        // Fields for the managers
        private static UIManager uIManager;
        private static World world;

        // Managers
        public static UIManager UIManager { get => uIManager; set => uIManager = value; }
        public static World World { get => world; set => world = value; }

        private static void Main()
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create(GameWidth, GameHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.Instance.OnStart = Init;

            //Start the game.
            SadConsole.Game.Instance.Run();

            // Code here will not run until the game window closes.
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            // Makes so that no excpetion happens for a custom control
            SadConsole.Themes.Library.Default.SetControlTheme(typeof(UI.Controls.MagiButton),
                new SadConsole.Themes.ButtonTheme());

            //Instantiate the UIManager
            UIManager = new UIManager();

            // Now let the UIManager create its consoles
            // so they can use the World data
            UIManager.InitMainMenu();
        }
    }
}