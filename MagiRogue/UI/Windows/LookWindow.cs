﻿using MagiRogue.Entities;
using SadRogue.Primitives;
using Console = SadConsole.Console;

namespace MagiRogue.UI.Windows
{
    public class LookWindow : PopWindow
    {
        private readonly Entity entityLooked;
        private readonly Console lookConsole;

        public LookWindow(Entity entity) : base(entity.Name)
        {
            entityLooked = entity;

            lookConsole = new Console(Width - ButtonWidth - 4, Height - 4)
            {
                Position = new Point(ButtonWidth + 2, 1)
            };

            lookConsole.Cursor.Position = new Point(1, 1);
            if (entity.Description is not null)
            {
                lookConsole.Cursor.Print(entity.Description);
            }
            Children.Add(lookConsole);
        }
    }
}