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

        public IList<Entity> TargetList { get; set; }

        public Coord OriginCoord { get; set; }

        public Target(Coord spawnCoord)
        {
            Color targetColor = new Color(255, 0, 0);

            OriginCoord = spawnCoord;

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

        public IList<T> TargetEntity<T>() where T : Entity
        {
            TargetList = null;

            IList<T> entities = GameLoop.World.CurrentMap.GetEntities<T>(Cursor.Position).ToList();

            entities.RemoveAt(0);

            if (entities.Count != 0)
            {
                TargetList = (IList<Entity>)entities;
                return entities;
            }

            return null;
        }

        public T TargetTile<T>() where T : TileBase
        {
            return GameLoop.World.CurrentMap.GetTileAt<T>(Cursor.Position);
        }

        public bool EntityInTarget()
        {
            if (GameLoop.World.CurrentMap.GetEntities<Entity>(Cursor.Position).Any(e => e.ID != Cursor.ID)
                && GameLoop.World.CurrentMap.GetEntity<Entity>(Cursor.Position) is not Player)
            {
                TargetEntity<Entity>();
                return true;
            }
            return false;
        }

        public Actor TargetRandomActor(int searchRadius, Coord centerRadius)
        {
            RadiusAreaProvider radius = new RadiusAreaProvider(centerRadius, searchRadius, Radius.CIRCLE);

            throw new NotImplementedException();
        }
    }
}