using GoRogue;
using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Tiles;
using MagiRogue.UI;
using SadRogue.Primitives;
using SadConsole.Input;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadConsole.Components;
using MagiRogue.Components;

namespace MagiRogue.Commands
{
    /// <summary>
    /// Defines an target system.
    /// </summary>
    public class Target : ITarget
    {
        public Entity Cursor { get; set; }

        public IList<Entity> TargetList { get; set; }

        public Point OriginCoord { get; set; }

        public Target(Point spawnCoord)
        {
            Color targetColor = new Color(255, 0, 0);

            OriginCoord = spawnCoord;

            Cursor = new Actor("Target Cursor", targetColor, Color.Transparent, 'X', spawnCoord, (int)MapLayer.PLAYER)
            {
                IsWalkable = true,
                CanBeKilled = false,
                CanBeAttacked = false,
                CanInteract = false,
                LeavesGhost = false
            };

            /* ColoredGlyph frameCell = Cursor.Appearance;
             frameCell.Foreground = Color.Transparent;
             frameCell.Background = Color.Transparent;
             frameCell.Glyph = 'X';

             Timer timer = new Timer(TimeSpan.FromSeconds(2.0));*/

            SadConsole.Effects.EffectsManager.ColoredGlyphState s = new SadConsole.Effects.EffectsManager.ColoredGlyphState(Cursor.Appearance);

            SadConsole.Effects.Blink blink = new SadConsole.Effects.Blink()
            {
                BlinkCount = -1,
                BlinkSpeed = 2.0,
                //BlinkOutColor = Color.Green,
                UseCellBackgroundColor = true
            };
            Cursor.Effect = blink;
            blink.Restart();
            //Cursor.AddComponent(new AnimationComponent(timer, Cursor.Appearance, frameCell));
        }

        public IList<T> TargetEntity<T>() where T : Entity
        {
            TargetList = null;

            IList<T> entities = GameLoop.World.CurrentMap.GetEntitiesAt<T>(Cursor.Position).ToList();

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
            if (GameLoop.World.CurrentMap.GetEntitiesAt<Entity>(Cursor.Position).Any(e => e.ID != Cursor.ID)
                && GameLoop.World.CurrentMap.GetEntityAt<Entity>(Cursor.Position) is not Player)
            {
                TargetEntity<Entity>();
                return true;
            }
            return false;
        }
    }
}