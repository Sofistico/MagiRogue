using MagiRogue.GameSys.Tiles;
using MagusEngine.Core.Entities.Base;
using System.Text;
using Console = SadConsole.Console;

namespace Diviner.Windows
{
    public class LookWindow : PopWindow
    {
        private readonly MagiEntity entityLooked;
        private readonly TileBase tileLooked;
        private readonly Console lookConsole;

        public LookWindow(MagiEntity entity) : base(entity.Name)
        {
            entityLooked = entity;

            lookConsole = new Console(Width - ButtonWidth - 4, Height - 4)
            {
                Position = new Point(ButtonWidth + 2, 1)
            };

            lookConsole.Cursor.Position = new Point(1, 1);
            StringBuilder desc = new StringBuilder();
            if (entity.Description is not null)
            {
                desc.Append(entity.GetDescriptor());
                desc.AppendLine();
                desc.Append(entity.GetCurrentStatus());
                lookConsole.Cursor.Print(entity.Description);
            }
            Children.Add(lookConsole);
        }

        public LookWindow(TileBase tile) : base(tile.Name)
        {
            tileLooked = tile;

            lookConsole = new Console(Width - ButtonWidth - 4, Height - 4)
            {
                Position = new Point(ButtonWidth + 2, 1)
            };

            lookConsole.Cursor.Position = new Point(1, 1);
            if (tile.Description is not null)
            {
                lookConsole.Cursor.Print(tile.Description);
            }
            Children.Add(lookConsole);
        }
    }
}