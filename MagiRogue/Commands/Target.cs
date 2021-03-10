using GoRogue;
using MagiRogue.Entities;
using MagiRogue.System.Tiles;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Commands
{
    public class Target : ITarget
    {
        private int _targetGlyph;
        private Coord _targetCoord;

        public Target(int targetGlyph, Coord targetCoord)
        {
            _targetGlyph = targetGlyph;
            _targetCoord = targetCoord;
        }

        public void ControlCursor(ScrollingConsole console)
        {
            throw new NotImplementedException();
        }

        public T TargetEntity<T>(Entity entity) where T : Entity
        {
            throw new NotImplementedException();
        }

        public T TargetTile<T>(TileBase tile) where T : TileBase
        {
            throw new NotImplementedException();
        }
    }
}