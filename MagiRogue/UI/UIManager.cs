using GoRogue;
using MagiRogue.Commands;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Time;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using System;
using System.Collections.Generic;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MagiRogue.UI
{
    // Creates/Holds/Destroys all consoles used in the game
    // and makes consoles easily addressable from a central place.
    public class UIManager : ContainerConsole
    {
        #region Managers

        // Here are the managers
        public ScrollingConsole MapConsole;
        public Window MapWindow;
        public MessageLogWindow MessageLog;
        public InventoryWindow InventoryScreen;
        public StatusWindow StatusConsole;
        public MainMenuWindow MainMenu;

        #endregion Managers

        #region Field

        private static Player GetPlayer => GameLoop.World.Player;

        #endregion Field

        #region ConstructorAndInitCode

        public UIManager()
        {
            // must be set to true
            // or will not call each child's Draw method
            IsVisible = true;
            IsFocused = true;

            // The UIManager becomes the only
            // screen that SadConsole processes
            Parent = Global.CurrentScreen;
        }

        // Initiates the game by means of going to the menu first
        public void InitMainMenu()
        {
            MainMenu = new MainMenuWindow(GameLoop.GameWidth, GameLoop.GameHeight);
            Children.Add(MainMenu);
            MainMenu.Show();
            MainMenu.Position = new Point(0, 0);

            CreateConsoles();
        }

        public void StartGame()
        {
            GameLoop.World = new World();

            // Hides the main menu, so that it's possible to interact with the other windows.
            MainMenu.Hide();

            //Message Log initialization
            MessageLog = new MessageLogWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2);

            // Inventory initialization
            InventoryScreen = new InventoryWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Inventory Window");
            Children.Add(InventoryScreen);
            InventoryScreen.Hide();
            InventoryScreen.Position = new Point(GameLoop.GameWidth / 2, 0);

            StatusConsole = new StatusWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Status Window");
            Children.Add(StatusConsole);
            StatusConsole.Position = new Point(GameLoop.GameWidth / 2, 0);
            StatusConsole.Show();

            // Load the map into the MapConsole
            LoadMap(GameLoop.World.CurrentMap);

            // Now that the MapConsole is ready, build the Window
            CreateMapWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight, "Game Map");
            UseMouse = true;

            // Start the game with the camera focused on the player
            CenterOnActor(GameLoop.World.Player);
        }

        public void StartTestGame()
        {
            GameLoop.World = new World(true);

            // Hides the main menu, so that it's possible to interact with the other windows.
            MainMenu.Hide();

            //Message Log initialization
            MessageLog = new MessageLogWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2);

            // Inventory initialization
            InventoryScreen = new InventoryWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Inventory Window");
            Children.Add(InventoryScreen);
            InventoryScreen.Hide();
            InventoryScreen.Position = new Point(GameLoop.GameWidth / 2, 0);

            StatusConsole = new StatusWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Status Window");
            Children.Add(StatusConsole);
            StatusConsole.Position = new Point(GameLoop.GameWidth / 2, 0);
            StatusConsole.Show();

            // Load the map into the MapConsole
            LoadMap(GameLoop.World.CurrentMap);

            // Now that the MapConsole is ready, build the Window
            CreateMapWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight, "Game Map");
            UseMouse = true;

            // Start the game with the camera focused on the player
            CenterOnActor(GameLoop.World.Player);
        }

        #endregion ConstructorAndInitCode

        #region HelperMethods

        // Creates all child consoles to be managed
        private void CreateConsoles()
        {
            // Temporarily create a console with *no* tile data that will later be replaced with map data
            MapConsole = new ScrollingConsole(GameLoop.GameWidth, GameLoop.GameHeight);
        }

        // centers the viewport camera on an Actor
        public void CenterOnActor(Actor actor)
        {
            MapConsole.CenterViewPortOnPoint(actor.Position);
        }

        #region Input

        private static readonly Dictionary<Keys, Direction> MovementDirectionMapping = new Dictionary<Keys, Direction>
        {
            { Keys.NumPad7, Direction.UP_LEFT }, { Keys.NumPad8, Direction.UP }, { Keys.NumPad9, Direction.UP_RIGHT },
            { Keys.NumPad4, Direction.LEFT }, { Keys.NumPad6, Direction.RIGHT },
            { Keys.NumPad1, Direction.DOWN_LEFT }, { Keys.NumPad2, Direction.DOWN }, { Keys.NumPad3, Direction.DOWN_RIGHT },
            { Keys.Up, Direction.UP }, { Keys.Down, Direction.DOWN }, { Keys.Left, Direction.LEFT }, { Keys.Right, Direction.RIGHT }
        };

        // Scans the SadConsole's Global KeyboardState and triggers behaviour
        // based on the button pressed.
        public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
        {
            //if (info.IsKeyPressed(Keys.F11))
            //Settings.ToggleFullScreen(); // Too bugged right now to be used

            if (HandleMove(info))
            {
                if (!GetPlayer.Bumped)
                    GameLoop.World.ProcessTurn(TimeHelper.GetWalkTime(GetPlayer), true);
                else
                    GameLoop.World.ProcessTurn(TimeHelper.GetAttackTime(GetPlayer), true);
            }

            if (info.IsKeyPressed(Keys.NumPad5))
                GameLoop.World.ProcessTurn(TimeHelper.Wait, true);

            if (info.IsKeyPressed(Keys.Escape))
                SadConsole.Game.Instance.Exit();

            if (info.IsKeyPressed(Keys.A))
            {
                bool sucess = CommandManager.DirectAttack(GameLoop.World.Player);
                GameLoop.World.ProcessTurn(TimeHelper.GetAttackTime(GameLoop.World.Player), sucess);
            }

            if (info.IsKeyPressed(Keys.G))
            {
                Item item = GameLoop.World.CurrentMap.GetEntityAt<Item>(GameLoop.World.Player.Position);
                bool sucess = CommandManager.PickUp(GameLoop.World.Player, item);
                InventoryScreen.ShowItems(GameLoop.World.Player);
                GameLoop.World.ProcessTurn(TimeHelper.Interact, sucess);
            }
            if (info.IsKeyPressed(Keys.D))
            {
                bool sucess = CommandManager.DropItems(GameLoop.World.Player);
                Item item = GameLoop.World.CurrentMap.GetEntity<Item>(GameLoop.World.Player.Position);
                InventoryScreen.RemoveItemFromConsole(item);
                InventoryScreen.ShowItems(GameLoop.World.Player);
                GameLoop.World.ProcessTurn(TimeHelper.Interact, sucess);
            }
            if (info.IsKeyPressed(Keys.C))
            {
                bool sucess = CommandManager.CloseDoor(GameLoop.World.Player);
                GameLoop.World.ProcessTurn(TimeHelper.Interact, sucess);
            }
            if (info.IsKeyPressed(Keys.I))
            {
                InventoryScreen.Show();
            }

            if (info.IsKeyPressed(Keys.H))
            {
                bool sucess = CommandManager.HurtYourself(GameLoop.World.Player);
                GameLoop.World.ProcessTurn(TimeHelper.MagicalThings, sucess);
            }
#if DEBUG
            if (info.IsKeyPressed(Keys.F10))
            {
                CommandManager.ToggleFOV();
            }

            if (info.IsKeyPressed(Keys.F8))
            {
                GetPlayer.AddComponent(new Components.TestComponent(GetPlayer));
            }
#endif
            return base.ProcessKeyboard(info);
        }

        private bool HandleMove(SadConsole.Input.Keyboard info)
        {
            foreach (Keys key in MovementDirectionMapping.Keys)
            {
                if (info.IsKeyPressed(key))
                {
                    Direction moveDirection = MovementDirectionMapping[key];
                    Coord coorToMove = new Coord(moveDirection.DeltaX, moveDirection.DeltaY);

                    bool sucess = CommandManager.MoveActorBy(GameLoop.World.Player, coorToMove);
                    CenterOnActor(GameLoop.World.Player);
                    return sucess;
                }
            }

            return false;
        }

        #endregion Input

        // Creates a window that encloses a map console
        // of a specified height and width
        // and displays a centered window title
        // make sure it is added as a child of the UIManager
        // so it is updated and drawn
        public void CreateMapWindow(int width, int height, string title)
        {
            MapWindow = new Window(width, height)
            {
                CanDrag = false
            };

            MapWindow.ThemeColors = SadConsole.Themes.Colors.CreateAnsi();

            //make console short enough to show the window title
            //and borders, and position it away from borders
            int mapConsoleWidth = width - 2;
            int mapConsoleHeight = height - 2;

            // Resize the Map Console's ViewPort to fit inside of the window's borders snugly
            MapConsole.ViewPort = new Rectangle(0, 0, mapConsoleWidth, mapConsoleHeight);

            //reposition the MapConsole so it doesnt overlap with the left/top window edges
            MapConsole.Position = new Point(1, 1);

            MapConsole.DefaultBackground = Color.Black;

            //close window button
            /*Button closeButton = new Button(3, 1);
            closeButton.Position = new Point(0, 0);
            closeButton.Text = "[X]";

            //Add the close button to the Window's list of UI elements
            MapWindow.Add(closeButton);*/

            // center the title of the console at the top of the window
            MapWindow.Title = title.Align(HorizontalAlignment.Center, mapConsoleWidth);

            //add the map viewer to the window
            MapWindow.Children.Add(MapConsole);

            // The MapWindow becomes a child console of the UIManager
            Children.Add(MapWindow);

            // Add the player to the MapConsole's render list
            MapConsole.Children.Add(GameLoop.World.Player);

            // Without this, the window will never be visible on screen
            MapWindow.Show();
        }

        // Method helper to print the X and Y of the console.
        // Use only for debugging purposes

        /*private static void PrintHeader()
        {
            int counter = 0;
            var startingColor = Color.Black.GetRandomColor(SadConsole.Global.Random);
            var color = startingColor;
            var UIManagerConsoles = Global.CurrentScreen;
            for (int x = 0; x < UIManagerConsoles.Width; x++)
            {
                UIManagerConsoles[x].Glyph = counter.ToString()[0];
                UIManagerConsoles[x].Foreground = color;

                counter++;

                if (counter == 10)
                {
                    counter = 0;
                    color = color.GetRandomColor(SadConsole.Global.Random);
                }
            }

            counter = 0;
            color = startingColor;
            for (int y = 0; y < UIManagerConsoles.Height; y++)
            {
                UIManagerConsoles[0, y].Glyph = counter.ToString()[0];
                UIManagerConsoles[0, y].Foreground = color;

                counter++;

                if (counter == 10)
                {
                    counter = 0;
                    color = color.GetRandomColor(SadConsole.Global.Random);
                }
            }

            // Display console size
            UIManagerConsoles.Print(4, 2, "Console Size");
            UIManagerConsoles.Print(4, 3, "                         ");
            UIManagerConsoles.Print(4, 3, $"{UIManagerConsoles.Width} {UIManagerConsoles.Height}");
        }*/

        // Adds the entire list of entities found in the
        // World.CurrentMap's Entities SpatialMap to the
        // MapConsole, so they can be seen onscreen
        private void SyncMapEntities(Map map)
        {
            // remove all Entities from the console first
            MapConsole.Children.Clear();

            // Now pull all of the entities into the MapConsole in bulk
            foreach (Entity entity in map.Entities.Items)
            {
                MapConsole.Children.Add(entity);
            }

            // Subscribe to the Entities ItemAdded listener, so we can keep our MapConsole entities in sync
            map.Entities.ItemAdded += OnMapEntityAdded;

            // Subscribe to the Entities ItemRemoved listener, so we can keep our MapConsole entities in sync
            map.Entities.ItemRemoved += OnMapEntityRemoved;
        }

        // Remove an Entity from the MapConsole every time the Map's Entity collection changes
        public void OnMapEntityRemoved(object sender, ItemEventArgs<GoRogue.GameFramework.IGameObject> e)
        {
            MapConsole.Children.Remove(e.Item as Entity);
        }

        // Add an Entity to the MapConsole every time the Map's Entity collection changes
        public void OnMapEntityAdded(object sender, ItemEventArgs<GoRogue.GameFramework.IGameObject> e)
        {
            MapConsole.Children.Add(e.Item as Entity);
        }

        // Loads a Map into the MapConsole
        private void LoadMap(Map map)
        {
            // First load the map's tiles into the console
            MapConsole = new ScrollingConsole(GameLoop.World.CurrentMap.Width, GameLoop.World.CurrentMap.Height, Global.FontDefault,
                new Rectangle(0, 0, GameLoop.GameWidth, GameLoop.GameHeight), map.Tiles);

            // Now Sync all of the map's entities
            SyncMapEntities(map);
        }

        #endregion HelperMethods
    }
}