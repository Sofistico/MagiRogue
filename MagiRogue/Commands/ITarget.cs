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
        IList<T> TargetEntity<T>() where T : Entity;

        T TargetTile<T>() where T : TileBase;
    }
}