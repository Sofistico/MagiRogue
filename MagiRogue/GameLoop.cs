using MagiRogue.Commands;
using MagiRogue.System;
using MagiRogue.System.Physics;
using MagiRogue.UI;
using Microsoft.Xna.Framework;

namespace MagiRogue
{
    public class GameLoop
    {
        public const int GameWidth = 120;
        public const int GameHeight = 30;

        // Managers
        private static UIManager uIManager;
        private static World world;
        private static CommandManager commandManager;
        private static PhysicsManager physicsManager;

        public static UIManager UIManager { get => uIManager; set => uIManager = value; }
        public static World World { get => world; set => world = value; }
        public static CommandManager CommandManager { get => commandManager; set => commandManager = value; }
        public static PhysicsManager PhysicsManager { get => physicsManager; set => physicsManager = value; }

        private static void Main()
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
            // The world is being built before the main menu even appears
            //World = new World();

            //Instantiate a new CommandManager
            CommandManager = new CommandManager();

            //Instantiate a new PhysicsManager
            PhysicsManager = new PhysicsManager();

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