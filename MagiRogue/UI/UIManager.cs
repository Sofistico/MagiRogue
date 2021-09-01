using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.UI.Windows;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using Color = SadConsole.UI.AdjustableColor;

namespace MagiRogue.UI
{
    // Creates/Holds/Destroys all consoles used in the game
    // and makes consoles easily addressable from a central place.
    public class UIManager : ScreenObject
    {
        #region Managers

        // Here are the managers
        public MapWindow MapWindow { get; set; }

        public MessageLogWindow MessageLog { get; set; }
        public InventoryWindow InventoryScreen { get; set; }
        public StatusWindow StatusConsole { get; set; }
        public MainMenuWindow MainMenu { get; set; }
        public CharacterCreationWindow CharCreationWindow { get; set; }

        public bool NoPopWindow { get; set; } = true;

        #endregion Managers

        #region Field

        public SadConsole.UI.Colors CustomColors { get; private set; }

        #endregion Field

        #region ConstructorAndInitCode

        public UIManager()
        {
            // must be set to true
            // or will not call each child's Draw method
            IsVisible = true;
            IsFocused = true;

            UseMouse = false;

            // The UIManager becomes the only
            // screen that SadConsole processes
            Parent = GameHost.Instance.Screen;
        }

        // Initiates the game by means of going to the menu first
        public void InitMainMenu()
        {
            SetUpCustomColors();

            MainMenu = new MainMenuWindow(GameLoop.GameWidth, GameLoop.GameHeight)
            {
                IsFocused = true
            };
            Children.Add(MainMenu);
            MainMenu.Show();
            MainMenu.Position = new Point(0, 0);
        }

        public void StartGame(Player player, bool testGame = false)
        {
            IsFocused = true;
            MainMenu.GameStarted = true;

            if (testGame)
            {
                player = new Player(SadRogue.Primitives.Color.White, SadRogue.Primitives.Color.Black, Point.None);
            }
            else
                CharCreationWindow.Hide();

            GameLoop.World = new World(player, testGame);

            //Message Log initialization
            MessageLog = new MessageLogWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Message Log");
            Children.Add(MessageLog);
            MessageLog.Show();
            MessageLog.Position = new Point(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2);
#if DEBUG
            MessageLog.Add("Test message log works");
#endif
            // Inventory initialization
            InventoryScreen = new InventoryWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2);
            Children.Add(InventoryScreen);
            InventoryScreen.Hide();

            StatusConsole = new StatusWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight / 2, "Status Window");
            Children.Add(StatusConsole);
            StatusConsole.Position = new Point(GameLoop.GameWidth / 2, 0);
            StatusConsole.Show();

            // Build the Window
            CreateMapWindow(GameLoop.GameWidth / 2, GameLoop.GameHeight, "Game Map");

            // Then load the map into the MapConsole
            MapWindow.LoadMap(GameLoop.World.CurrentMap);

            // Start the game with the camera focused on the player
            MapWindow.CenterOnActor(GameLoop.World.Player);
        }

        /// <summary>
        /// The char creation screen, before the initialization of the game.
        /// </summary>
        public void CharCreationScreen()
        {
            if (CharCreationWindow is null)
                CharCreationWindow = new CharacterCreationWindow(GameLoop.GameWidth, GameLoop.GameHeight);
            CharCreationWindow.Position = new Point(0, 0);
            CharCreationWindow.Show();
            Children.Add(CharCreationWindow);
            // Hides the main menu to make it possible to interact with the screen.
            MainMenu.Hide();
        }

        #endregion ConstructorAndInitCode

        #region Input

        /// <summary>
        /// Scans the SadConsole's Global KeyboardState and triggers behaviour
        /// based on the button pressed.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override bool ProcessKeyboard(Keyboard info)
        {
            if (GameLoop.World != null)
            {
                if (MapWindow.HandleMapInteraction(info, this, GameLoop.World))
                {
                    return true;
                }
                if (HandleUiKeys(info))
                {
                    return true;
                }
            }

            return base.ProcessKeyboard(info);
        }

        private bool HandleUiKeys(Keyboard info)
        {
            if (info.IsKeyPressed(Keys.I))
            {
                InventoryScreen.Show();
                return true;
            }

            if (info.IsKeyPressed(Keys.Escape) && NoPopWindow)
            {
                MainMenu.Show();
                MainMenu.IsFocused = true;
                return true;
            }

            return false;
        }

        #endregion Input

        #region HelperMethods

        // Creates a window that encloses a map console
        // of a specified height and width
        // and displays a centered window title
        // make sure it is added as a child of the UIManager
        // so it is updated and drawn
        public void CreateMapWindow(int width, int height, string title)
        {
            MapWindow = new MapWindow(width, height, title);

            // The MapWindow becomes a child console of the UIManager
            Children.Add(MapWindow);

            // Add the map console to it
            MapWindow.CreateMapConsole();

            // Without this, the window will never be visible on screen
            MapWindow.Show();
        }

        // Build a new coloured theme based on SC's default theme
        // and then set it as the program's default theme.
        private void SetUpCustomColors()
        {
            // Create a set of default colours that we will modify
            CustomColors = SadConsole.UI.Themes.Library.Default.Colors.Clone();

            // Pick a couple of background colours that we will apply to all consoles.
            Color backgroundColor = new Color(CustomColors.Black, "Black");

            // Set background colour for controls consoles and their controls
            CustomColors.ControlHostBackground = backgroundColor;
            CustomColors.ControlBackgroundNormal = backgroundColor;

            // Generate background colours for dark and light themes based on
            // the default background colour.
            //CustomColors.ControlH = (backgroundColor * 1.3f).FillAlpha();
            //CustomColors.ControlBackDark = (backgroundColor * 0.7f).FillAlpha();

            // Set a color for currently selected controls. This should always
            // be different from the background colour.
            CustomColors.ControlBackgroundSelected = new Color(CustomColors.GrayDark, "Grey");

            // Rebuild all objects' themes with the custom colours we picked above.
            CustomColors.RebuildAppearances();

            // Now set all of these colours as default for SC's default theme.
            SadConsole.UI.Themes.Library.Default.Colors = CustomColors;
        }

        #endregion HelperMethods
    }
}