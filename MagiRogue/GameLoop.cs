﻿global using Point = SadRogue.Primitives.Point;
using GoRogue;
using MagiRogue.GameSys;
using MagiRogue.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MagiRogue
{
    public static class GameLoop
    {
        public const int GameWidth = 120;
        public const int GameHeight = 30;
        private static int hfId = 0;
        private static int civId;
        private static int mythId;

        // Managers
        public static UIManager UIManager { get; set; }

        public static Universe Universe { get; set; }

        public static IDGenerator IdGen { get; private set; } = new(1);
        public static ShaiRandom.Generators.IEnhancedRandom GlobalRand { get; } = GoRogue.Random.GlobalRandom.DefaultRNG;

        private static void Main(string[] args)
        {
            ConfigureBeforeCreateGame();

            // Setup the engine and creat the main window.
            SadConsole.Game.Create(GameWidth, GameHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.Instance.OnStart = Init;

            //Start the game.
            SadConsole.Game.Instance.Run();

            // Code here will not run until the game window closes.
            SadConsole.Game.Instance.Dispose();
        }

        private static void ConfigureBeforeCreateGame()
        {
            // Pre options before creating the game, defines the title and if can resize
            SadConsole.Settings.WindowTitle = "MagiRogue";
            SadConsole.Settings.AllowWindowResize = true;
            // It's ugly, but it's the best
            SadConsole.Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.Stretch;
            // Let's see how this one can be done, will be used in a future serialization work
            SadConsole.Settings.AutomaticAddColorsToMappings = true;
        }

        private static void Init()
        {
            Palette.AddToColorDictionary();
            // Makes so that no excpetion happens for a custom control
            SadConsole.UI.Themes.Library.Default.SetControlTheme(typeof(UI.Controls.MagiButton),
                new SadConsole.UI.Themes.ButtonTheme());

            //Instantiate the UIManager
            UIManager = new UIManager();

            // Now let the UIManager create its consoles
            // so they can use the World data
            UIManager.InitMainMenu();
        }

        /// <summary>
        /// Gets the current map, a shorthand for GameLoop.Universe.CurrentMap
        /// </summary>
        /// <returns></returns>
        public static Map GetCurrentMap()
        {
            return Universe.CurrentMap;
        }

        public static void SetIdGen(uint lastId) => IdGen = new IDGenerator(lastId + 1);

        public static void AddMessageLog(string message)
        {
            if (UIManager is null)
                return;
            UIManager.MessageLog.Add(message);
        }

        #region IdCounters

        public static int GetHfId()
        {
            return hfId++;
        }

        public static int GetCivId()
            => civId++;

        public static int GetMythId()
        {
            return mythId++;
        }

        public static void WriteToLog(List<string> errors)
        {
            try
            {
                var path = new StringBuilder(AppDomain.CurrentDomain.BaseDirectory).Append(@"\log.txt").ToString();
                StringBuilder str = new StringBuilder($"{DateTime.Now:dd/MM/yyyy}");
                foreach (var item in errors)
                {
                    str.AppendLine(item);
                }

                File.AppendAllText(path, str.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void WriteToLog(string error)
        {
            WriteToLog(new List<string>() { error });
        }

        #endregion IdCounters
    }
}