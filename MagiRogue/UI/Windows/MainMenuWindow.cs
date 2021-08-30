using MagiRogue.UI.Controls;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;

namespace MagiRogue.UI.Windows
{
    public class MainMenuWindow : MagiBaseWindow
    {
        public bool GameStarted { get; set; }

        private readonly MagiButton startGame;
        private readonly MagiButton testMap;
        private readonly MagiButton continueGame;

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
            if (GameStarted)
            {
                Hide();
                GameLoop.UIManager.IsFocused = true;
            }
            else
                continueGame.IsEnabled = false;
        }

        private void TestMap_Click(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                GameLoop.UIManager.StartGame(new Entities.Player(Color.Black, Color.White, Point.None), true);
                GameStarted = true;
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
            if (!GameStarted)
            {
                GameLoop.UIManager.CharCreationScreen();
            }
            else
            {
                startGame.IsEnabled = false;
            }
        }

        private void RefreshButtons()
        {
            foreach (var control in Controls)
            {
                control.IsEnabled = true;
            }
        }

        public void RestartGame()
        {
            GameStarted = false;
            GameLoop.World = null;
            RefreshButtons();

            foreach (SadConsole.Console item in GameLoop.UIManager.Children)
            {
                if (!item.Equals(this))
                    item.Dispose();
            }

            GameLoop.UIManager.MessageLog = null;
            GameLoop.UIManager.MapWindow = null;
            GameLoop.UIManager.StatusConsole = null;
            GameLoop.UIManager.InventoryScreen = null;
            GameLoop.UIManager.Children.Clear();
            GameLoop.UIManager.Children.Add(this);

            Show();
        }
    }
}