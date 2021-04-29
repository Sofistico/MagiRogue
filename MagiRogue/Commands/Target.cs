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
    /// <summary>
    /// Defines an target system.
    /// </summary>
    public class Target : ITarget
    {
        public Entity Cursor { get; set; }

        public Action TargetAction;

        public Coord OriginCoord { get; set; }

        public Target(Coord spawnCoord)
        {
            Color targetColor = new Color(255, 0, 0);

            spawnCoord = OriginCoord;

            Cursor = new Actor(targetColor, Color.Transparent, 'X', (int)MapLayer.PLAYER, spawnCoord)
            {
                Name = "Target Cursor",
                IsWalkable = true,
                CanBeKilled = false,
                CanBeAttacked = false,
                CanInteract = false,
                LeavesGhost = false
            };

            var frameCell = Cursor.Animation.CreateFrame()[0];
            frameCell.Foreground = Color.Transparent;
            frameCell.Background = Color.Transparent;
            frameCell.Glyph = 'X';

            Cursor.Animation.AnimationDuration = 2.0f;
            Cursor.Animation.Repeat = true;

            Cursor.Animation.Start();
        }

        public T TargetEntity<T>() where T : Entity
        {
            return GameLoop.World.CurrentMap.GetEntityAt<T>(Cursor.Position);
        }

        public T TargetTile<T>() where T : TileBase
        {
            return GameLoop.World.CurrentMap.GetTileAt<T>(Cursor.Position);
        }
    }
}