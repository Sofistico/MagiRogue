using System.Diagnostics.CodeAnalysis;
using Arquimedes.Enumerators;
using Arquimedes.Settings;
using Diviner.Exceptions;
using Diviner.Interfaces;
using Diviner.Windows;
using GoRogue.Messaging;
using MagusEngine;
using MagusEngine.Exceptions;
using MagusEngine.Bus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Utils;
using SadConsole;
using SadConsole.Input;
using Color = SadConsole.UI.AdjustableColor;

namespace Diviner
{
    // Creates/Holds/Destroys all consoles used in the game and makes consoles easily addressable
    // from a central place.
    public sealed class UIManager : ScreenObject,
        ISubscriber<FocusUiManagerMessage>,
        ISubscriber<TogglePopWindowMessage>,
        ISubscriber<StartGameMessage>,
        ISubscriber<RestartGame>,
        ISubscriber<LookStuff>,
        ISubscriber<ShowGlyphOnConsole>
    {
        private readonly Dictionary<WindowTag, IWindowTagContract> _windows = [];
        private Universe? _universe;

        #region Managers

        // Here are the managers, in the future will be a dictionary
        public MapWindow MapWindow { get; set; } = null!;

        public MessageLogWindow MessageLog { get; set; } = null!;
        public InventoryWindow InventoryScreen { get; set; } = null!;
        public BottomStatusWindow StatusWindow { get; set; } = null!;
        public MainMenuWindow MainMenu { get; set; } = null!;
        public CharacterCreationWindow CharCreationWindow { get; set; } = null!;

        public bool NoPopWindow { get; set; } = true;

        #endregion Managers

        #region Field

        public SadConsole.UI.Colors CustomColors { get; private set; } = null!;

        #endregion Field

        #region ConstructorAndInitCode

        public UIManager()
        {
            UseMouse = false;
            Locator.GetService<MessageBusService>().RegisterAllSubscriber(this);
        }

        // Initiates the game by means of going to the menu first
        public void InitMainMenu(int gameHeight, int gameWidth, bool beginOnTestMap = false)
        {
            //SetUpCustomColors();

            MainMenu = new MainMenuWindow(gameWidth, gameHeight)
            {
                IsFocused = true
            };
            Children.Add(MainMenu);
            MainMenu.Show();
            MainMenu.Position = new Point(0, 0);
            if (beginOnTestMap)
                StartGame(Player.TestPlayer(), gameHeight, gameWidth, null, true);
        }

        public void StartGame(Player player, int height, int width, Universe? uni = null, bool testGame = false)
        {
            IsFocused = true;
            MainMenu.GameStarted = true;
            MainMenu.Hide();

            if (!testGame && uni is null)
            {
                CharCreationWindow.Hide();
            }

            if (uni is not null)
            {
                if (uni.CurrentMap!.LastPlayerPosition == Point.None)
                    throw new Exception("The player position was invalid, an error occured!");
                uni.PlacePlayerOnLoad();
            }
            else
            {
                uni = new Universe(player, testGame);
            }
            Find.Universe = uni;
            ConfigureWindowsAndConsoles(height, width, uni);
        }

        private void ConfigureWindowsAndConsoles(int height, int width, Universe uni)
        {
            //Message Log initializationz
            var messageLogWidth = (int)MathMagi.GetSimplePercentage(width, 0.25) - 2;
            MessageLog = new MessageLogWindow(messageLogWidth, height, "Message Log")
            {
                Position = new Point(width - messageLogWidth, 0),
            };
#if DEBUG
            Locator.GetService<MessageBusService>()?.SendMessage<AddMessageLog>(new("Test message log works"));
#endif
            // Inventory initialization
            InventoryScreen = new InventoryWindow(width / 2, height / 2);
            Children.Add(InventoryScreen);
            InventoryScreen.Hide();
            const int statusWindowHeight = 4;
            StatusWindow = new BottomStatusWindow(width - MessageLog.Width, statusWindowHeight, "Status Window");
            StatusWindow.ChangePositionToBottomPage();
            // Build the Window
            CreateMapWindow(width - MessageLog.Width, height - StatusWindow.Height, "Game Map");

            // Then load the map into the MapConsole
            MapWindow.LoadMap(uni.CurrentMap);
            // Start the game with the camera focused on the player
            MapWindow.CenterOnActor(uni.Player);
            AddWindowToList(MapWindow);
            Children.Add(StatusWindow);
            Children.Add(MessageLog);
            StatusWindow.Show();
            MessageLog.Show();

            FocusedMode = FocusBehavior.Set;
            IsFocused = true;
        }

        /// <summary>
        /// The char creation screen, before the initialization of the game.
        /// </summary>
        public void CharCreationScreen(int width, int height)
        {
            CharCreationWindow ??= new CharacterCreationWindow(width, height);
            CharCreationWindow.Position = new Point(0, 0);
            CharCreationWindow.Show();
            Children.Add(CharCreationWindow);
            // Hides the main menu to make it possible to interact with the screen.
            MainMenu.Hide();
        }

        #endregion ConstructorAndInitCode

        #region Overrides

        #region Input

        /// <summary>
        /// Scans the SadConsole's Global KeyboardState and triggers behaviour based on the button pressed.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override bool ProcessKeyboard(Keyboard info)
        {
            _universe ??= Find.Universe;
            if (_universe?.CurrentMap is not null
                && (_universe.CurrentMap.ControlledEntitiy is not null
                || _universe.WorldMap.AssocietatedMap.Equals(_universe.CurrentMap)))
            {
                if (KeyboardHandle.HandleMapKeys(info, this, _universe))
                {
                    return true;
                }
                if (KeyboardHandle.HandleUiKeys(info, this))
                {
                    return true;
                }
            }
            return base.ProcessKeyboard(info);
        }

        #endregion Input

        #endregion Overrides

        #region HelperMethods

        // Creates a window that encloses a map console of a specified height and width and displays
        // a centered window title make sure it is added as a child of the UIManager so it is
        // updated and drawn
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

        // Build a new coloured theme based on SC's default theme and then set it as the program's
        // default theme.
        private void SetUpCustomColors()
        {
            // Create a set of default colours that we will modify
            CustomColors = new SadConsole.UI.Colors();

            // Pick a couple of background colours that we will apply to all consoles.
            Color backgroundColor = new(CustomColors.Black, "Black");

            // Set background colour for controls consoles and their controls
            CustomColors.ControlHostBackground = backgroundColor;
            CustomColors.ControlBackgroundNormal = backgroundColor;

            // Generate background colours for dark and light themes based on
            // the default background colour.
            //CustomColors.ControlH = (backgroundColor * 1.3f).FillAlpha();
            //CustomColors.ControlBackDark = (backgroundColor * 0.7f).FillAlpha();

            // Set a color for currently selected controls. This should always be different from the
            // background colour.
            CustomColors.ControlBackgroundSelected = new Color(CustomColors.GrayDark, "Grey");

            // Rebuild all objects' themes with the custom colours we picked above.
            CustomColors.RebuildAppearances();

            // Now set all of these colours as default for SC's default theme.
            //SadConsole.UI.Themes.Library.Default.Colors = CustomColors;
        }

        public T? GetWindow<T>(WindowTag tag) where T : IWindowTagContract
        {
            _windows.TryGetValue(tag, out var window);
            return (T?)window;
        }

        public void AddWindowToList(IWindowTagContract window)
        {
            if (window.Tag == WindowTag.Undefined)
            {
                throw new UndefinedWindowTagException($"The tag for the window was undefined! Window: {window}");
            }
            _windows.Add(window.Tag, window);
        }

        #endregion HelperMethods

        public void Handle(FocusUiManagerMessage message)
        {
            IsFocused = true;
        }

        public void Handle(TogglePopWindowMessage message)
        {
            NoPopWindow = message.NoPopWindow;
        }

        public void Handle(StartGameMessage message)
        {
            var settings = Locator.GetService<GlobalSettings>();
            StartGame(message.Player, settings.ScreenHeight, settings.ScreenWidth, message.Universe, message.TestGame);
        }

        public void Handle(RestartGame message)
        {
            MainMenu.Show();
        }

        public void Handle(LookStuff message)
        {
            LookWindow look;
            if (message.Tile is not null)
                look = new LookWindow(message.Tile);
            else
                look = new LookWindow(message.Entitiy!);

            look.Show(true);
        }

        public void Handle([NotNull] ShowGlyphOnConsole message)
        {
            MagiBaseWindow window = GetWindow<MagiBaseWindow>(message.WindowTag) ?? throw new NullValueException(nameof(window));
            var originalGlyph = window.GetGlyph(message.Position.X, message.Position.Y);
            window.SetGlyph(message.Position.X, message.Position.Y, message.Glyph);
            // How will i recover the last used glyph if the message will be destroyed?
            message.LastUsedGlyph = originalGlyph;
        }

        ~UIManager()
        {
            Locator.GetService<MessageBusService>().UnRegisterAllSubscriber(this);
        }
    }
}
