using MagiRogue.GameSys;
using MagiRogue.GameSys.Time;
using MagiRogue.Settings;
using MagiRogue.UI;
using MagiRogue.Utils;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue;

public static class GameLoop
{
    private static GlobalSettings settings;

    public static int GameWidth { get; private set; }
    public static int GameHeight { get; private set; }

    // Managers
    public static UIManager UIManager { get; set; }

    public static Universe Universe { get; set; }

    public static IDGenerator IdGen { get; private set; } = new(1);
    public static ShaiRandom.Generators.IEnhancedRandom GlobalRand { get; } = GoRogue.Random.GlobalRandom.DefaultRNG;

    #region configuration

    private static void Main(string[] args)
    {
        ConfigureBeforeCreateGame(args);

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

    // runs each frame
    private static void ConfigureBeforeCreateGame(string[] args)
    {
        foreach (var _ in args)
        {
            // TODO!
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
        Palette.AddToColorDictionary();
        // Makes so that no excpetion happens for a custom control
        //SadConsole.UI.Themes.Library.Default.SetControlTheme(typeof(UI.Controls.MagiButton),
        //   typeof(SadConsole.UI.Themes.ButtonTheme));

        //Instantiate the UIManager
        UIManager = (UIManager)GameHost.Instance.Screen!;

        // Now let the UIManager create its consoles
        // so they can use the World data
        UIManager.InitMainMenu();
    }

    #endregion configuration

    #region global methods

    /// <summary>
    /// Gets the current map, a shorthand for GameLoop.Universe.CurrentMap
    /// </summary>
    /// <returns></returns>
    public static Map GetCurrentMap()
    {
        return Universe?.CurrentMap;
    }

    public static void SetIdGen(uint lastId) => IdGen = new IDGenerator(lastId + 1);

    public static void AddMessageLog(string message, bool newLine = true)
    {
        if (UIManager is null && UIManager.MessageLog is null)
            return;
        UIManager.MessageLog.PrintMessage(message, newLine);
        UIManager.StatusWindow.ChangePositionToUpMessageLog();
    }

    public static int GetNHoursFromTurn(int hours)
    {
        int turn = Universe.Time.Turns;
        int turnInNHours = turn * TimeDefSpan.SecondsPerHour * hours;
        return turnInNHours;
    }

    #region IdCounters

    public static int GetHfId()
    {
        return SequentialIdGenerator.HistoricalFigureId;
    }

    public static int GetCivId()
        => SequentialIdGenerator.CivId;

    public static int GetMythId()
    {
        return SequentialIdGenerator.MythId;
    }

    public static int GetAbilityId()
    {
        return SequentialIdGenerator.AbilityId;
    }

    #endregion IdCounters

    #region Logs

    public static void WriteToLog(List<string> errors)
    {
        // so that it doesn't block the main thread!
        Task.Run(() =>
        {
            if (errors.Count == 0)
                return;
            var path = new StringBuilder(AppDomain.CurrentDomain.BaseDirectory).Append(@"\log.txt").ToString();
            StringBuilder str = new StringBuilder($"{DateTime.Now:dd/MM/yyyy} ");
            foreach (var item in errors)
            {
                str.AppendLine(item);
                str.AppendLine();
            }
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            File.AppendAllText(path, str.ToString());
#if DEBUG
            AddMessageLog("Logged an error in the logs file!");
#endif
        });
    }

    public static void WriteToLog(string error)
    {
        WriteToLog(new List<string>() { error });
    }

    #endregion Logs

    #endregion global methods
}
