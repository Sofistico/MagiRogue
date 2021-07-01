using MagiRogue.UI.Controls;
using SadRogue.Primitives;
using SadConsole;
using SadConsole.UI.Controls;
using SadConsole.UI.Themes;
using System;
using System.Collections.Generic;

namespace MagiRogue.UI.Windows
{
    public class MainMenuWindow : MagiBaseWindow
    {
        private bool _gameStarted;

        private MagiButton startGame;
        private MagiButton testMap;
        private MagiButton continueGame;

        public MainMenuWindow(int width, int height, string title = "Main Menu") : base(width, height, title)
        {
            continueGame = new MagiButton(15, 1)
            {
                Text = "Continue Game"
            };
            startGame = new MagiButton(12, 1)
            {
                Text = "Start Game"
            };
            testMap = new MagiButton(12, 1)
            {
                Text = "Test Map"
            };
            MagiButton quitGame = new MagiButton(11, 1)
            {
                Text = "Quit Game"
            };

            continueGame.Click += ContinueGame_Click;
            startGame.Click += StartGameClick;
            testMap.Click += TestMap_Click;
            quitGame.Click += QuitGameClick;

            SetupSelectionButtons(startGame, continueGame, testMap, quitGame);

            PositionButtons();

            IsFocused = true;
        }

        private void ContinueGame_Click(object sender, EventArgs e)
        {
            if (_gameStarted)
            {
                Hide();
                GameLoop.UIManager.IsFocused = true;
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
            SadConsole.GameHost.Instance.Dispose();
        }

        private void PositionButtons()
        {
            int i = 0;
            foreach (ControlBase button in Controls)
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