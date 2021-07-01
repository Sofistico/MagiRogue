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
using MagiRogue.System.Magic;

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

        public TargetState State { get; set; }

        public Target(Point spawnCoord)
        {
            Color targetColor = new Color(255, 0, 0);

            OriginCoord = spawnCoord;

            Cursor = new Actor("Target Cursor",
                targetColor, Color.Transparent, 'X', spawnCoord, (int)MapLayer.PLAYER)
            {
                IsWalkable = true,
                CanBeKilled = false,
                CanBeAttacked = false,
                CanInteract = false,
                LeavesGhost = false
            };

            SadConsole.Effects.Blink blink = new SadConsole.Effects.Blink()
            {
                BlinkCount = -1,
                BlinkSpeed = 2.0,
                UseCellBackgroundColor = true
            };
            Cursor.Effect = blink;
            blink.Restart();

            State = TargetState.Resting;
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
                State = TargetState.Targeting;
                return true;
            }
            return false;
        }

        public void OnSelectSpell(SpellBase spell, Actor caster)
        {
            var (targetDistance, sucess) = StartTargetting();

            if (sucess)
            {
                var distance = Distance.Chebyshev.Calculate(OriginCoord, targetDistance);

                if (distance <= spell.SpellRange)
                {
                    spell.CastSpell(targetDistance, caster);
                    EndTargetting();
                    return;
                }
            }
            else
                return;
        }

        private (Point targetDistance, bool sucess) StartTargetting()
        {
            GameLoop.World.ChangeControlledEntity(Cursor);
            State = TargetState.Targeting;

            if (EntityInTarget())
            {
                return (TargetList.FirstOrDefault().Position, true);
            }
            return (Point.None, false);
        }

        private void EndTargetting()
        {
            GameLoop.World.ChangeControlledEntity(GameLoop.World.Player);
            GameLoop.World.CurrentMap.Remove(Cursor);
            State = TargetState.Resting;
        }

        public enum TargetState
        {
            Resting,
            Targeting
        }
    }
}