using MagiRogue.System;
using MagiRogue.UI;

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
            // Pre options before creating the game, defines the title and if can resize
            SadConsole.Settings.WindowTitle = "MagiRogue";
            SadConsole.Settings.AllowWindowResize = true;
            // It's ugly, but it's the best
            SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.Stretch;
            // Let's see how this one can be done, will be used in a future serialization work
            SadConsole.Settings.AutomaticAddColorsToMappings = true;

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
            SadConsole.UI.Themes.Library.Default.SetControlTheme(typeof(UI.Controls.MagiButton),
                new SadConsole.UI.Themes.ButtonTheme());

            //Instantiate the UIManager
            UIManager = new UIManager();

            // Now let the UIManager create its consoles
            // so they can use the World data
            UIManager.InitMainMenu();
        }
    }
}