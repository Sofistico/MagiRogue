using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;
using System;

namespace MagiRogue.UI
{
    public class MainMenuWindow : Window
    {
        // The control console where all buttons will be inside
        private readonly ControlsConsole controlConsole;

        // The window border, to make the console fit inside the window
        private readonly int windowBorder = 1;

        public MainMenuWindow(int width, int height, string title = "Main Menu") : base(width, height)
        {
            ThemeColors = SadConsole.Themes.Colors.CreateAnsi();

            Title = title.Align(HorizontalAlignment.Left, width);

            controlConsole = new ControlsConsole(width, height - windowBorder)
            {
                Position = new Point(0, 1),
                ThemeColors = Colors.CreateAnsi()
            };

            Button startGame = new Button(12, 1)
            {
                Text = "Start Game",
                ThemeColors = Colors.CreateAnsi()
            };
            Button testMap = new Button(12, 1)
            {
                Text = "Test Map",
                ThemeColors = Colors.CreateAnsi()
            };
            Button quitGame = new Button(11, 1)
            {
                Text = "Quit Game",
                ThemeColors = Colors.CreateAnsi()
            };

            Children.Add(controlConsole);
            startGame.Click += StartGameClick;
            testMap.Click += TestMap_Click;
            quitGame.Click += QuitGameClick;

            controlConsole.Add(startGame);
            controlConsole.Add(testMap);
            controlConsole.Add(quitGame);

            PositionButtons();
        }

        private void TestMap_Click(object sender, EventArgs e) => GameLoop.UIManager.StartTestGame();

        private void QuitGameClick(object sender, EventArgs e)
        {
            SadConsole.Game.Instance.Exit();
        }

        private void PositionButtons()
        {
            int i = 0;
            foreach (ControlBase button in controlConsole)
            {
                button.Position = new Point(55, 2 + (i * 2));
                ++i;
            }
        }

        private void StartGameClick(object sender, EventArgs e) => GameLoop.UIManager.StartGame();
    }
}