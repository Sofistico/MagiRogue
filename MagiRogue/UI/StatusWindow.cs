﻿using MagiRogue.Entities;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Collections.Generic;

namespace MagiRogue.UI
{
    public class StatusWindow : Window
    {
        private Player player;
        private ScrollingConsole statsConsole;
        private ScrollBar statusScroll;

        private const int windowBorderThickness = 2;

        public StatusWindow(int width, int heigth, string title) : base(width, heigth)
        {
            // Ensure that the window background is the correct colour
            ThemeColors = SadConsole.Themes.Colors.CreateAnsi();

            CanDrag = false;

            player = GameLoop.World.Player;

            Title = title.Align(HorizontalAlignment.Center, width);

            statsConsole = new ScrollingConsole(width - windowBorderThickness, heigth - windowBorderThickness)
            {
                Position = new Point(1, 1),
                ViewPort = new Rectangle(0, 0, width - 1, heigth - windowBorderThickness),
                DefaultBackground = Color.Black
            };

            statusScroll = new ScrollBar
                (Orientation.Vertical, heigth - windowBorderThickness)
            {
                Position = new Point(statsConsole.Width + 1, statsConsole.Position.X)
                //IsEnabled = false
            };
            statusScroll.ValueChanged += StatusScroll_ValueChanged; ;
            Add(statusScroll);

            // enable mouse input
            UseMouse = true;

            Children.Add(statsConsole);
            PrintStats();
        }

        private void StatusScroll_ValueChanged(object sender, EventArgs e)
        {
            statsConsole.ViewPort = new Rectangle(0, statusScroll.Value + windowBorderThickness,
                statsConsole.Width, statsConsole.ViewPort.Height);
        }

        public void PrintStats()
        {
            statsConsole.Print(0, 0, player.Name);
            statsConsole.Print(0, 2, player.Health.ToString(), Color.Red);
            statsConsole.Print(2, 2, player.MaxHealth.ToString(), Color.DarkRed);
        }
    }
}