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
        private SpellBase _spellSelected;
        private Actor _caster;

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
                BlinkSpeed = 1.3,
                UseCellBackgroundColor = true
            };
            Cursor.Effect = blink;
            blink.Restart();

            State = TargetState.Resting;

            TargetList = new List<Entity>();
        }

        public IList<T> TargetEntity<T>() where T : Entity
        {
            TargetList.Clear();

            IList<T> entities = GameLoop.World.CurrentMap.GetEntitiesAt<T>(Cursor.Position).ToList();

            entities.RemoveAt(0);

            if (entities.Count != 0)
            {
                TargetList = (List<Entity>)entities;
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
            _spellSelected = spell;
            _caster = caster;
            if (_spellSelected.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Self))
            {
                TargetList.Add(_caster);
                var (sucess, s) = EndSpellTargetting();
                GameLoop.World.ProcessTurn
                    (System.Time.TimeHelper.GetCastingTime(GameLoop.World.Player, s), sucess);
                return;
            }
            StartTargetting();
        }

        private void StartTargetting()
        {
            GameLoop.World.ChangeControlledEntity(Cursor);
            GameLoop.World.CurrentMap.Add(Cursor);
            State = TargetState.Targeting;
        }

        public (bool, SpellBase) EndSpellTargetting()
        {
            int distance = (int)Distance.Chebyshev.Calculate(OriginCoord, Cursor.Position);

            if (distance <= _spellSelected.SpellRange)
            {
                _spellSelected.CastSpell(TargetList[0].Position, _caster);
                var spellCasted = _spellSelected;
                _spellSelected = null;
                _caster = null;
                EndTargetting();
                return (true, spellCasted);
            }
            GameLoop.UIManager.MessageLog.Add("The target is too far!");
            return (false, null);
        }

        public void EndTargetting()
        {
            if (Cursor.CurrentMap is null)
            {
                State = TargetState.Resting;
                TargetList.Clear();
                return;
            }
            GameLoop.World.ChangeControlledEntity(GameLoop.World.Player);
            GameLoop.World.CurrentMap.Remove(Cursor);
            State = TargetState.Resting;
            TargetList.Clear();
        }

        public enum TargetState
        {
            Resting,
            Targeting
        }
    }
}