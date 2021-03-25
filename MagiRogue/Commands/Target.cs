using GoRogue;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using MagiRogue.UI;
using Microsoft.Xna.Framework;
using SadConsole.Input;
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
        public Coord TargetCoord { get; set; }
        public Entity Cursor { get; set; }

        public Target(Coord targetCoord)
        {
            TargetCoord = targetCoord;

            Cursor = new Entity(Color.White, Color.Transparent, 'X', TargetCoord, (int)MapLayer.PLAYER);
            //Cursor.Animation.Frames[1].DefaultForeground = Color.Transparent;
            //Cursor.Animation.AnimationDuration = 1.0f;
        }

        public T TargetEntity<T>(Entity entity) where T : Entity
        {
            throw new NotImplementedException();
        }

        public T TargetTile<T>(TileBase tile) where T : TileBase
        {
            throw new NotImplementedException();
        }

        public void HandleCursorMove(Keyboard info)
        {
            foreach (var key in UIManager.MovementDirectionMapping)
            {
                if (info.IsKeyPressed(key.Key))
                {
                    Cursor.Position += key.Value;
                    return;
                }
            }
        }
    }
}