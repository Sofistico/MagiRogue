using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;
using System;

namespace MagiRogue.UI
{
    public class MainMenuWindow : MagiBaseWindow
    {
        // The control console where all buttons will be inside
        private readonly ControlsConsole controlConsole;

        // The window border, to make the console fit inside the window
        private readonly int windowBorder = 1;

        private bool _gameStarted;

        private Button startGame;
        private Button testMap;
        private Button continueGame;

        public MainMenuWindow(int width, int height, string title = "Main Menu") : base(width, height, title)
        {
            controlConsole = new ControlsConsole(width, height - windowBorder)
            {
                Position = new Point(0, 1),
            };

            continueGame = new Button(15, 1)
            {
                Text = "Continue Game"
            };
            startGame = new Button(12, 1)
            {
                Text = "Start Game"
            };
            testMap = new Button(12, 1)
            {
                Text = "Test Map"
            };
            Button quitGame = new Button(11, 1)
            {
                Text = "Quit Game"
            };

            Children.Add(controlConsole);
            continueGame.Click += ContinueGame_Click;
            startGame.Click += StartGameClick;
            testMap.Click += TestMap_Click;
            quitGame.Click += QuitGameClick;

            controlConsole.Add(continueGame);
            controlConsole.Add(startGame);
            controlConsole.Add(testMap);
            controlConsole.Add(quitGame);

            PositionButtons();
        }

        private void ContinueGame_Click(object sender, EventArgs e)
        {
            if (_gameStarted)
            {
                Hide();
            }
        }

        private void TestMap_Click(object sender, EventArgs e)
        {
            if (!_gameStarted)
            {
                GameLoop.UIManager.StartGame(true);
                _gameStarted = true;
            }
            else
            {
                testMap.IsEnabled = false;
            }
        }

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

        private void StartGameClick(object sender, EventArgs e)
        {
            if (!_gameStarted)
            {
                GameLoop.UIManager.StartGame();
                _gameStarted = true;
            }
            else
            {
                startGame.IsEnabled = false;
            }
        }
    }
}