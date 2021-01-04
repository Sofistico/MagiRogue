﻿using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;

namespace MagiRogue.UI
{
    //A scrollable window that displays messages
    //using a FIFO (first-in-first-out) Queue data structure
    public class MessageLogWindow : Window
    {
        //max number of lines to store in message log
        private readonly int maxLines = 100;

        // a Queue works using a FIFO structure, where the first line added
        // is the first line removed when we exceed the max number of lines
        private readonly Queue<string> lines;

        // the messageConsole displays the active messages
        private readonly ScrollingConsole messageConsole;

        //scrollbar for message console
        private readonly SadConsole.Controls.ScrollBar messageScrollBar;

        //Track the current position of the scrollbar
        private int scrollBarCurrentPosition;

        // account for the thickness of the window border to prevent UI element spillover
        private readonly int windowBorderThickness = 2;

        // Create a new window with the title centered
        // the window is draggable by default
        public MessageLogWindow(int width, int height, string title) : base(width, height)
        {
            // Ensure that the window background is the correct colour
            //Theme.FillStyle.Background = DefaultBackground;

            ThemeColors = SadConsole.Themes.Colors.CreateAnsi();

            lines = new Queue<string>();
            CanDrag = false;

            Title = title.Align(HorizontalAlignment.Center, Width);

            // add the message console, reposition, enable the viewport, and add it to the window
            messageConsole = new ScrollingConsole(width - windowBorderThickness, height - windowBorderThickness)
            {
                Position = new Point(1, 1)
            };
            messageConsole.ViewPort = new Rectangle(0, 0, width - 1, height - windowBorderThickness);
            messageConsole.DefaultBackground = Color.Black;

            // create a scrollbar and attach it to an event handler, then add it to the Window
            messageScrollBar = new SadConsole.Controls.ScrollBar
                (Orientation.Vertical, height - windowBorderThickness)
            {
                Position = new Point(messageConsole.Width + 1, messageConsole.Position.X),
                IsEnabled = false
            };
            messageScrollBar.ValueChanged += MessageScrollBarValueChanged;
            Add(messageScrollBar);

            // enable mouse input
            UseMouse = true;

            // Add the child consoles to the window
            Children.Add(messageConsole);
        }

        public override void Draw(TimeSpan drawTime)
        {
            base.Draw(drawTime);
        }

        public void Add(string message)
        {
            lines.Enqueue(message);
            // when exceeding the max number of lines remove the oldest one
            if (lines.Count > maxLines)
            {
                lines.Dequeue();
            }
            // Move the cursor to the last line and print the message.
            messageConsole.Cursor.Position = new Point(1, lines.Count);
            messageConsole.Cursor.Print(message + "\n");
        }

        // Controls the position of the messagelog viewport
        // based on the scrollbar position using an event handler
        private void MessageScrollBarValueChanged(object sender, EventArgs e)
        {
            messageConsole.ViewPort = new Rectangle(0,
                messageScrollBar.Value
                + windowBorderThickness,
                messageConsole.Width,
                messageConsole.ViewPort.Height);
        }

        public override void Update(TimeSpan time)
        {
            // Ensure that the scrollbar tracks the current position of the messageConsole.
            if (messageConsole.TimesShiftedUp != 0 |
                messageConsole.Cursor.Position.Y >= messageConsole.ViewPort.Height + scrollBarCurrentPosition)
            {
                //enable the scrollbar once the messagelog has filled up with enough text to warrant scrolling
                messageScrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (scrollBarCurrentPosition < messageConsole.Height - messageConsole.ViewPort.Height)
                {
                    // Record how much we've scrolled to enable how far back the bar can see
                    scrollBarCurrentPosition += messageConsole.TimesShiftedUp != 0 ? messageConsole.TimesShiftedUp : 1;
                }

                // Determines the scrollbar's max vertical position
                // Thanks @Kaev for simplifying this math!
                messageScrollBar.Maximum = scrollBarCurrentPosition - windowBorderThickness;

                // This will follow the cursor since we move the render area in the event.
                messageScrollBar.Value = scrollBarCurrentPosition;

                // Reset the shift amount.
                //messageConsole.TimesShiftedUp = 0;
            }

            base.Update(time);
        }
    }
}