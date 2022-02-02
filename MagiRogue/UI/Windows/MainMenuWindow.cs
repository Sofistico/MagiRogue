using MagiRogue.Entities;
using MagiRogue.UI.Controls;
using SadConsole.UI.Controls;
using SadRogue.Primitives;
using System;
using SadConsole;
using Console = SadConsole.Console;
using SadConsole.UI;
using MagiRogue.Data;
using MagiRogue.System;

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

            SetupSelectionButtons(startGame, continueGame, saveGame, testMap, quitGame);

            PositionButtons();

            IsFocused = true;
        }

        private void ContinueGame_Click(object sender, EventArgs e)
        {
            if (GameStarted && GameLoop.UIManager.NoPopWindow)
            {
                Hide();
                GameLoop.UIManager.IsFocused = true;
            }
            else
            {
                if (Utils.SaveUtils.CheckIfThereIsSaveFile())
                {
                    // creates a new pop window
                    PopWindow pop = new PopWindow(30, 15, "Load Game");
                    const string text = "Load!";
                    // The load button
                    MagiButton loadGame = new MagiButton(text.Length + 2)
                    {
                        Text = text,
                        Position = new Point(3, pop.Height - 2)
                    };
                    // the event handler of the click
                    loadGame.Click += LoadGame_Click;
                    // TODO: make this into a method for pop window, since the ratio of the controls
                    // are amazing
                    savesBox = new ListBox(pop.Width / 2 + 5, 10)
                    {
                        Position = new Point((pop.Width / 2) - 10, (pop.Height / 2) - 5)
                    };

                    // populate the list with the save names
                    PopulateListWithSaves();

                    // and the set up with SadConsole
                    pop.Controls.Add(loadGame);
                    pop.Controls.Add(savesBox);
                    Children.Add(pop);
                    pop.Show();
                }
            }
        }

        private void LoadGame_Click(object? sender, EventArgs e)
        {
            if (savesBox.SelectedItem != null)
            {
                if (SaveAndLoad.CheckIfStringIsValidSave(savesBox.SelectedItem.ToString()))
                {
                    Universe uni = SaveAndLoad.LoadGame(savesBox.SelectedItem.ToString());
                    GameLoop.UIManager.StartGame(uni.Player, uni: uni);
                }
            }
        }

        /// <summary>
        /// This takes all save file on the Saves folder and shows them
        /// </summary>
        private void PopulateListWithSaves()
        {
            string[] saves = Utils.SaveUtils.ReturnAllSaveFiles();
            for (int i = 0; i < saves.Length; i++)
            {
                string save = Utils.SaveUtils.GetSaveName(saves[i]);
                savesBox.Items.Add(save);
            }
        }

        private void TestMap_Click(object sender, EventArgs e)
        {
            if (!GameStarted && GameLoop.UIManager.NoPopWindow)
            {
                GameLoop.UIManager.StartGame(Player.TestPlayer(), testGame: true);
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
            if (!GameStarted && GameLoop.UIManager.NoPopWindow)
            {
                GameLoop.UIManager.CharCreationScreen();
            }
        }

        private void SaveGameClick(object sender, EventArgs e)
        {
            if (GameStarted && GameLoop.UIManager.NoPopWindow)
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