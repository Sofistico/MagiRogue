using Diviner.Controls;
using SadConsole;
using SadConsole.UI.Controls;
using System;
using System.Linq;
using Arquimedes.Data;
using MagusEngine.Core.Entities;
using MagusEngine.Systems;

namespace Diviner.Windows
{
    public class MainMenuWindow : MagiBaseWindow
    {
        public bool GameStarted { get; set; }

        private readonly MagiButton startGame;
        private readonly MagiButton testMap;
        private readonly MagiButton continueGame; // this doesn't work
        private readonly MagiButton saveGame; // neither does this
        private ListBox savesBox;
        private PopWindow loadPop;

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

        private void ContinueGame_Click(object? sender, EventArgs e)
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
                    loadPop = new PopWindow(30, 15, "Load Game");
                    const string text = "Load!";
                    // The load button
                    MagiButton loadGame = new MagiButton(text.Length + 2)
                    {
                        Text = text,
                        Position = new Point(3, loadPop.Height - 2)
                    };
                    // the event handler of the click
                    loadGame.Click += LoadGame_Click;
                    // TODO: make this into a method for pop window, since the ratio of the controls
                    // are amazing
                    savesBox = new ListBox(loadPop.Width / 2 + 5, 10)
                    {
                        Position = new Point((loadPop.Width / 2) - 10, (loadPop.Height / 2) - 5)
                    };
                    string delete = "Delete";
                    MagiButton deleteSaveBtn = new MagiButton(delete.Length + 2)
                    {
                        Text = delete,
                        Position = new Point(loadPop.Width / 2 - 5, loadPop.Height - 3)
                    };

                    deleteSaveBtn.Click += DeleteSaveBtn_Click;

                    // populate the list with the save names
                    PopulateListWithSaves();

                    // and the set up with SadConsole
                    loadPop.Controls.Add(loadGame);
                    loadPop.Controls.Add(deleteSaveBtn);
                    loadPop.Controls.Add(savesBox);
                    Children.Add(loadPop);
                    loadPop.Show();
                }
            }
        }

        private void DeleteSaveBtn_Click(object? sender, EventArgs e)
        {
            if (savesBox.SelectedItem != null)
            {
                SaveAndLoad.DeleteSave(savesBox.SelectedItem.ToString());
                savesBox.Items.Remove(savesBox.SelectedItem);
                savesBox.IsDirty = true;
            }
        }

        private void LoadGame_Click(object? sender, EventArgs e)
        {
            if (savesBox.SelectedItem != null)
            {
                if (SaveAndLoad.CheckIfStringIsValidSave(savesBox.SelectedItem.ToString()))
                {
                    loadPop.Hide();
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

        private void TestMap_Click(object? sender, EventArgs e)
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

        private void QuitGameClick(object? sender, EventArgs e)
        {
            Game.Instance.MonoGameInstance.Exit();
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

        private void StartGameClick(object? sender, EventArgs e)
        {
            if (!GameStarted && GameLoop.UIManager.NoPopWindow)
            {
                GameLoop.UIManager.CharCreationScreen();
            }
        }

        private void SaveGameClick(object? sender, EventArgs e)
        {
            if (GameStarted)
            {
                // makes so that there can only be one save per player name!
                GameLoop.Universe.SaveGame(GameLoop.Universe.Player.Name);
                PopWindow alert = new PopWindow(30, 10, "Save Done!");
                // makes it a variable so that it can be properly accounted for in the print command
                string text = "Save Sucessful!";
                alert.Print((alert.Width - text.Length) / 2, 3, text);
                Children.Add(alert);
                alert.Show();
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
            GameLoop.Universe = null!;
            RefreshButtons();

            foreach (SadConsole.Console item in GameLoop.UIManager.Children.Cast<SadConsole.Console>())
            {
                if (!item.Equals(this))
                    item.Dispose();
            }

            GameLoop.UIManager.MessageLog = null!;
            GameLoop.UIManager.MapWindow = null!;
            GameLoop.UIManager.StatusWindow = null!;
            GameLoop.UIManager.InventoryScreen = null!;
            GameLoop.UIManager.CharCreationWindow = null!;
            GameLoop.UIManager.Children.Clear();
            GameLoop.UIManager.Children.Add(this);

            Show();
        }
    }
}