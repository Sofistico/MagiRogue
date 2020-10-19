using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Themes;

namespace MagiRogue.UI
{
    public class MainMenuWindow : Window
    {
        public MainMenuWindow(int width, int height, string title = "Main Menu") : base(width, height)
        {
            Title = title.Align(HorizontalAlignment.Center, Width);

            Button startGame = new Button(12, 1)
            {
                Position = new Point(width / 2, height / 2),
                Text = "Start Game",
                ThemeColors = Colors.CreateAnsi()
            };
            startGame.Click += StartGame_Click;
            Add(startGame);
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            GameLoop.UIManager.StartGameMainMenu();
        }
    }
}