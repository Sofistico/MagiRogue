using GoRogue.Messaging;
using MagusEngine;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.MapStuff;
using MagusEngine.Services;
using System.Text;
using Console = SadConsole.Console;

namespace Diviner.Windows
{
    public class LookWindow : PopWindow,
        ISubscriber<LookStuff>
    {
        private readonly MagiEntity? entityLooked;
        private readonly Tile? tileLooked;
        private readonly Console lookConsole;

        public LookWindow(MagiEntity entity) : base(entity.Name)
        {
            entityLooked = entity;

            lookConsole = new Console(Width - ButtonWidth - 4, Height - 4)
            {
                Position = new Point(ButtonWidth + 2, 1)
            };

            lookConsole.Cursor.Position = new Point(1, 1);
            StringBuilder desc = new();
            if (entity.Description is not null)
            {
                desc.Append(entity.GetDescriptor());
                desc.AppendLine();
                desc.Append(entity.GetCurrentStatus());
                lookConsole.Cursor.Print(entity.Description);
            }
            Children.Add(lookConsole);
            RegisterSubs();
        }

        public LookWindow(Tile tile) : base(tile.Name)
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
            RegisterSubs();
        }

        public void Handle(LookStuff message)
        {
            LookWindow look;
            if (message.Tile is not null)
                look = new LookWindow(message.Tile);
            else
                look = new(message.Entitiy);

            look.Show(true);
        }

        private void RegisterSubs() => Locator.GetService<MessageBusService>().RegisterSubscriber(this);

        ~LookWindow()
        {
            Locator.GetService<MessageBusService>().UnRegisterSubscriber(this);
        }
    }
}
