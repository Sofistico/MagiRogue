using Microsoft.Xna.Framework;
using MagiRogue.System;
using MagiRogue.UI;
using MagiRogue.Commands;

namespace MagiRogue
{
    public class GameLoop
    {
        public const int GameWidth = 120;
        public const int GameHeight = 30;

        // Managers
        public static UIManager UIManager;

        public static World World;

        public static CommandManager CommandManager;

        static void Main()
        {
            // Setup the engine and creat the main window.
            SadConsole.Game.Create(GameWidth, GameHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            //Start the game.
            SadConsole.Game.Instance.Run();

            // Code here will not run until the game window closes.
            SadConsole.Game.Instance.Dispose();
        }

        private static void Init()
        {
            //Instantiate the UIManager
            UIManager = new UIManager();

            // Build the world!
            World = new World();

            //Instantiate a new CommandManager
            CommandManager = new CommandManager();

            // Now let the UIManager create its consoles
            // so they can use the World data
            UIManager.Init();
        }

        private static void Update(GameTime time)
        {
            // Necessary for the game to update
        }
    }
}