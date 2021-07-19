using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Magic;
using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using GoRogue.Pathing;
using SadConsole;

namespace MagiRogue.Commands
{
    /// <summary>
    /// Defines an target system.
    /// </summary>
    public class Target
    {
        private SpellBase _spellSelected;
        private Actor _caster;

        public Entity Cursor { get; set; }

        public IList<Entity> TargetList { get; set; }

        public Point OriginCoord { get; set; }

        public TargetState State { get; set; }

        public Dictionary<Point, TileBase> TravelPath { get; set; }

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
            TravelPath = new Dictionary<Point, TileBase>();
        }

        private IList<T> TargetEntity<T>() where T : Entity
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

        private T TargetTile<T>() where T : TileBase
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

            if (_spellSelected.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Beam))
            {
                Cursor.Moved += (_, __) =>
                {
                    var tile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(__.NewValue);

                    if (!TravelPath.Keys.Contains(__.NewValue))
                    {
                        TravelPath.Add(__.NewValue, tile);
                        tile.Background = Color.Yellow;
                    }
                    else
                    {
                        TravelPath.Remove(__.NewValue);
                        tile.Background = tile.LastSeenAppereance.Background;
                    }
                };
            }

            StartTargetting();
        }

        public void StartTargetting()
        {
            GameLoop.World.ChangeControlledEntity(Cursor);
            GameLoop.World.CurrentMap.Add(Cursor);
            State = TargetState.Targeting;
        }

        // TODO: Customize who should you target
        public (bool, SpellBase) EndSpellTargetting()
        {
            int distance = (int)Distance.Chebyshev.Calculate(OriginCoord, Cursor.Position);

            if (_spellSelected.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Beam))
            {
                return AffectPath();
            }

            if (distance <= _spellSelected.SpellRange)
            {
                bool casted = _spellSelected.CastSpell(TargetList[0].Position, _caster);
                var spellCasted = _spellSelected;
                EndTargetting();
                return (casted, spellCasted);
            }
            GameLoop.UIManager.MessageLog.Add("The target is too far!");
            return (false, null);
        }

        public void EndTargetting()
        {
            State = TargetState.Resting;
            TargetList.Clear();
            _spellSelected = null;
            _caster = null;

            // if there is anything in the path, clear it
            foreach (TileBase tile in TravelPath.Values)
            {
                tile.CopyAppearanceFrom(tile.LastSeenAppereance);
            }
            TravelPath.Clear();

            GameLoop.World.ChangeControlledEntity(GameLoop.World.Player);
            GameLoop.World.CurrentMap.Remove(Cursor);
        }

        private (bool, SpellBase) AffectPath()
        {
            if (TravelPath.Count >= 1)
            {
                foreach (Point target in TravelPath.Keys)
                {
                    _spellSelected.CastSpell(target, _caster);
                }
                var casted = _spellSelected;

                EndTargetting();

                return (true, casted);
            }

            return (false, null);
        }

        public enum TargetState
        {
            Resting,
            Targeting
        }
    }
}