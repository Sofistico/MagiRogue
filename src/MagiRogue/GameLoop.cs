using Arquimedes;
using Arquimedes.Settings;
using Arquimedes.Utils;
using Diviner;
using MagusEngine;
using MagusEngine.Services;
using SadConsole;
using SadConsole.Configuration;
using System;
using System.IO;

namespace MagiRogue
{
    public static class GameLoop
    {
        private static GlobalSettings settings;
        private static bool beginTest;
        private static UIManager ui;

        public static int GameWidth => settings.ScreenWidth;
        public static int GameHeight => settings.ScreenHeight;

        #region configuration

        private static void Main(string[] args)
        {
            ConfigureBeforeCreateGame(args);

            // Setup the engine and create the main window.
            var config = new Builder()
                .SetScreenSize(GameWidth, GameHeight)
                .OnStart(Init) // Hook the start event so we can add consoles to the system.
                .SetStartingScreen<UIManager>()
                .IsStartingScreenFocused(true)
                .ConfigureFonts(static (f, _) => f.UseBuiltinFontExtended());
            Game.Create(config);
            try
            {
                Game.Instance.Run();
            }
            catch (Exception ex)
            {
                string message = $"An error occurred during the game loop:\n- Ex: {ex.Message} \n- StackTrace - {ex.StackTrace}";
                Locator.GetService<MagiLog>().Log(message, "crash");
                System.Console.WriteLine(message);
                throw;
            }
            finally
            {
                // Code here will not run until the game window closes.
                Game.Instance.Dispose();
            }
        }

        // runs each frame
        private static void ConfigureBeforeCreateGame(string[] args)
        {
            foreach (var arg in args)
            {
                // TODO!
                if (arg.Equals("test", StringComparison.Ordinal))
                    beginTest = true;
            }
            // Pre options before creating the game, defines the title and if can resize
            Settings.WindowTitle = "MagiRogue";
            Settings.AllowWindowResize = true;
            // It's ugly, but it's the best
            Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.Stretch;
            // Let's see how this one can be done, will be used in a future serialization work
            Settings.AutomaticAddColorsToMappings = true;

            settings = JsonUtils.JsonDeseralize<GlobalSettings>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", "global_setting.json"))!;
            Locator.AddService(settings);
        }

        private static void Init(object? sender, GameHost e)
        {
            MagiPalette.AddToColorDictionary();
            // Makes so that no excpetion happens for a custom control
            //SadConsole.UI.Themes.Library.Default.SetControlTheme(typeof(UI.Controls.MagiButton),
            //   typeof(SadConsole.UI.Themes.ButtonTheme));

            //Instantiate the UIManager
            ui = (UIManager)GameHost.Instance.Screen!;

            // Now let the UIManager create its consoles so they can use the World data
            ui.InitMainMenu(GameHeight, GameWidth, beginTest);
            Locator.AddService(ui);
        }

        #endregion configuration
    }
}
