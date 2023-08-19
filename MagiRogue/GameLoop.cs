using Arquimedes;
using Arquimedes.Settings;
using Arquimedes.Utils;
using Diviner;
using MagusEngine;
using MagusEngine.Systems;
using SadConsole;
using System;
using System.IO;

namespace MagiRogue;

public static class GameLoop
{
    private static GlobalSettings settings;
    private static bool beginTest;

    public static int GameWidth { get; private set; }
    public static int GameHeight { get; private set; }

    // Managers
    public static UIManager UIManager { get; set; }

    public static Universe Universe { get; set; }

    #region configuration

    private static void Main(string[] args)
    {
        ConfigureBeforeCreateGame(args);

        ConfigureServices();

        // Setup the engine and create the main window.
        Game.Configuration gameStartup = new Game.Configuration()
            .SetScreenSize(GameWidth, GameHeight)
            .OnStart(Init) // Hook the start event so we can add consoles to the system.
            .SetStartingScreen<UIManager>()
            .IsStartingScreenFocused(true)
            .ConfigureFonts((f) => f.UseBuiltinFontExtended());

        Game.Create(gameStartup);
        //Start the game.
        Game.Instance.Run();
        // Code here will not run until the game window closes.
        Game.Instance.Dispose();
    }

    private static void ConfigureServices()
    {
        Locator.InitializeSingletonServices();
    }

    // runs each frame
    private static void ConfigureBeforeCreateGame(string[] args)
    {
        foreach (var arg in args)
        {
            // TODO!
            if (arg.Equals("test"))
                beginTest = true;
        }
        // Pre options before creating the game, defines the title and if can resize
        SadConsole.Settings.WindowTitle = "MagiRogue";
        SadConsole.Settings.AllowWindowResize = true;
        // It's ugly, but it's the best
        SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.Stretch;
        // Let's see how this one can be done, will be used in a future serialization work
        SadConsole.Settings.AutomaticAddColorsToMappings = true;

        settings = JsonUtils.JsonDeseralize<GlobalSettings>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "Settings", "global_setting.json"));
        GameHeight = settings.ScreenHeight;
        GameWidth = settings.ScreenWidth;
    }

    private static void Init()
    {
        MagiPalette.AddToColorDictionary();
        // Makes so that no excpetion happens for a custom control
        //SadConsole.UI.Themes.Library.Default.SetControlTheme(typeof(UI.Controls.MagiButton),
        //   typeof(SadConsole.UI.Themes.ButtonTheme));

        //Instantiate the UIManager
        UIManager = (UIManager)GameHost.Instance.Screen!;

        // Now let the UIManager create its consoles so they can use the World data
        UIManager.InitMainMenu(beginTest);
    }

    #endregion configuration
}
