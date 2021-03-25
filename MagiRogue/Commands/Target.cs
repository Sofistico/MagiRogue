using GoRogue;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
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
        private Entity cursor;

        public Target(int targetGlyph, Coord targetCoord)
        {
            _targetGlyph = targetGlyph;
            _targetCoord = targetCoord;

            cursor = new Entity(Color.Black, Color.Transparent, _targetGlyph, _targetCoord, (int)MapLayer.PLAYER);
        }

        public bool ControlCursor(SadConsole.Input.Keyboard keys)
        {
            return GameLoop.UIManager.HandleMove(keys);
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