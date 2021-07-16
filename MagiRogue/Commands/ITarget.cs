using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using System.Collections.Generic;

namespace MagiRogue.Commands
{
    public interface ITarget
    {
        IList<T> TargetEntity<T>() where T : Entity;

        T TargetTile<T>() where T : TileBase;
    }
}