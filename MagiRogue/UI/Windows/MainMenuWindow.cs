using MagiRogue.Entities;
using MagiRogue.UI.Controls;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using SadConsole;
using Console = SadConsole.Console;
using SadConsole.UI;
using MagiRogue.Data;

namespace MagiRogue.UI.Windows
{
    public class MainMenuWindow : MagiBaseWindow
    {
        public bool GameStarted { get; set; }

        private readonly MagiButton startGame;
        private readonly MagiButton testMap;
        private readonly MagiButton continueGame;
        private readonly MagiButton saveGame;
        private TextBox saveName;
        private PopWindow savePop;
        private PopWindow loadWindow;
        private ListBox savesBox;

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
            saveGame = new MagiButton(11, 1)
            {
                Text = "Save Game"
            };

            continueGame.Click += ContinueGame_Click;
            startGame.Click += StartGameClick;
            testMap.Click += TestMap_Click;
            quitGame.Click += QuitGameClick;
            saveGame.Click += SaveGameClick;

            SetupSelectionButtons(startGame, saveGame, continueGame, testMap, quitGame);

            PositionButtons();

            IsFocused = true;

            loadWindow = new PopWindow("Load Game");
            savesBox = new ListBox(loadWindow.Width, loadWindow.Height);
        }

        private void ContinueGame_Click(object sender, EventArgs e)
        {
            if (GameStarted)
            {
                Hide();
                GameLoop.UIManager.IsFocused = true;
            }
            else
            {
                if (SaveAndLoad.CheckIfThereIsSaveFile())
                {
                }
            }
        }

        private void TestMap_Click(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                GameLoop.UIManager.StartGame(Player.TestPlayer(), true);
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

        private void SaveGameClick(object sender, EventArgs e)
        {
            if (GameStarted)
            {
                OpenSavePop();
            }
        }

        private void OpenSavePop()
        {
            savePop = new PopWindow(30, 15, "Save test");
            saveName = new TextBox(10)
            {
                Position = new SadRogue.Primitives.Point(5, savePop.Height / 2)
            };
            string save = "Save";
            MagiButton saveAndClose = new MagiButton(save.Length + 2)
            {
                Text = save,
                Position = new Point(2, 13)
            };
            saveAndClose.Click += SaveAndClose_Click;
            savePop.Controls.Add(saveName);
            savePop.Controls.Add(saveAndClose);
            savePop.Surface.Print(saveName.Position.X - 2, saveName.Position.Y - 2, "Save name here:");
            Children.Add(savePop);
            savePop.Show();
        }

        private void SaveAndClose_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(saveName.Text) && GameStarted)
            {
                GameLoop.Universe.SaveGame(saveName.Text);
                savePop.Hide();
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
            GameLoop.Universe = null;
            RefreshButtons();

            /*foreach (Console item in Children)
            {
                Children.Remove(item);
                item.Dispose();
            }*/

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