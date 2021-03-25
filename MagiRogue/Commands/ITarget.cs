using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using SadConsole;
using SadConsole.Input;

namespace MagiRogue.Commands
{
    public interface ITarget
    {
        T TargetEntity<T>(Entity entity) where T : Entity;

        T TargetTile<T>(TileBase tile) where T : TileBase;

        bool ControlCursor(Keyboard keys);
    }
}