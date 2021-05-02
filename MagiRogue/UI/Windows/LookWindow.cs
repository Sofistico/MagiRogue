using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;

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
            lookConsole.Cursor.Print(entity.Description);

            Children.Add(lookConsole);
        }
    }
}