﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace MagiRogue.UI.Windows
{
    /// <summary>
    /// This creates a base pop window, keep in mind to create a console in the right plae the math is
    /// Width - ButtonWidth - 3, Height - 4, for the position = ButtonWidth + 2, 1
    /// </summary>
    public abstract class PopWindow : MagiBaseWindow
    {
        public const int ButtonWidth = 40;

        private readonly Button _cancelButton;

        /// <summary>
        /// This creates a base pop window, keep in mind to create a console in the right plae the math is
        /// Width - ButtonWidth - 3, Height - 4, for the position = ButtonWidth + 2, 1
        /// </summary>
        /// <param name="title"></param>
        public PopWindow(string title) : base(100, 20, title)
        {
            CloseOnEscKey = true;

            Center();
            IsFocused = true;

            const string cancelButtonText = "Cancel";

            int cancelButtonWidth = cancelButtonText.Length + 4;

            _cancelButton = new Button(cancelButtonWidth)
            {
                Text = cancelButtonText,
                Position = new Point(ButtonWidth + 1, Height - 2)
            };

            _cancelButton.Click += (_, __) => Hide();

            Add(_cancelButton);
        }
    }
}