using System.Text;
using MagusEngine.Components.TilesComponents;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using Console = SadConsole.Console;

namespace Diviner.Windows
{
    public class LookWindow : PopWindow
    {
        private readonly MagiEntity? entityLooked;
        private readonly Tile? tileLooked;
        private readonly Console lookConsole;

        public LookWindow(MagiEntity entity) : base(entity.Name)
        {
            entityLooked = entity;
            lookConsole = CreateLookConsole();
            StringBuilder desc = new();
            if (entity.Description is not null)
            {
                desc.Append(entity.GetDescriptor());
                desc.AppendLine();
                desc.Append(entity.GetCurrentStatus());
                lookConsole.Cursor.Print(entity.Description);
            }
#if DEBUG
            desc.Append("ID: ").Append(entity.ID).AppendLine();
#endif
            desc.Append("Position: ").AppendLine(entity.Position.ToString());
            Children.Add(lookConsole);
        }

        public LookWindow(Tile tile) : base(tile.Name)
        {
            tileLooked = tile;

            lookConsole = CreateLookConsole();

            StringBuilder desc = new();
            if (tile.Description is not null)
            {
                desc.Append(tile.Description).AppendLine();
            }
            desc.Append("This is made of: ").Append(tile.Material.Name).AppendLine();
#if DEBUG
            desc.Append("ID: ").Append(tile.ID).AppendLine();
#endif
            if (tile.IsTransparent)
                desc.Append("This tile is transparent").AppendLine();
            else
                desc.Append("This tile doesn't lets light though").AppendLine();
            if (tile.IsWalkable)
                desc.Append("This tile is walkable").AppendLine();
            else
                desc.Append("This tile isn't walkable").AppendLine();
            if (tile?.Traits?.Count > 0)
            {
                desc.AppendLine("This tile has the following properties:");
                foreach (var trait in tile.Traits)
                {
                    desc.Append(trait.ToString()).AppendLine();
                }
            }
            if (tile!.GetComponent<PlantComponent>(out var vegetation))
            {
                desc.Append("Tile has the following vegetation: ").Append(vegetation!.Plant.Name).AppendLine();
            }
            if (tile.GetComponent<WaterTile>(out var water))
            {
                desc.Append("This looks to have ").Append(water?.Depth).Append(" depth").AppendLine();
            }
            desc.Append("Position: ").AppendLine(tile.Position.ToString());
            lookConsole.Cursor.Print(desc.ToString());
            Children.Add(lookConsole);
        }

        private Console CreateLookConsole()
        {
            var lookConsole = new Console(Width - 2, Height - 3)
            {
                Position = new Point(1, 1),
            };

            lookConsole.Cursor.Position = new Point(0, 0);
            return lookConsole;
        }
    }
}
