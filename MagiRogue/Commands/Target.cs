using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Magic;
using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using GoRogue.Pathing;
using MagiRogue.UI.Windows;

namespace MagiRogue.Commands
{
    /// <summary>
    /// Defines an target system.
    /// </summary>
    public class Target
    {
        private Actor _caster;
        private readonly Dictionary<Point, TileBase> tileDictionary;

        public Entity Cursor { get; set; }

        public IList<Entity> TargetList { get; set; }

        public Point OriginCoord { get; set; }

        public TargetState State { get; set; }

        public Path TravelPath { get; set; }

        public int MaxDistance => SpellSelected is not null ? SpellSelected.SpellRange : 500;

        public SpellBase SpellSelected { get; set; }

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
            tileDictionary = new Dictionary<Point, TileBase>();
        }

        private void TargetEntity<T>() where T : Entity
        {
            TargetList.Clear();

            IList<T> entities = GameLoop.World.CurrentMap.GetEntitiesAt<T>(Cursor.Position).ToList();

            entities.RemoveAt(0);

            if (entities.Count != 0)
            {
                TargetList = (List<Entity>)entities;
            }
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
            SpellSelected = spell;
            _caster = caster;
            if (SpellSelected.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Self))
            {
                TargetList.Add(_caster);
                var (sucess, s) = EndSpellTargetting();
                GameLoop.World.ProcessTurn
                    (System.Time.TimeHelper.GetCastingTime(GameLoop.World.Player, s), sucess);
                return;
            }

            if (SpellSelected.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Beam))
            {
                LineTargetting();
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

            if (SpellSelected.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Beam))
            {
                return AffectPath();
            }

            if (distance <= SpellSelected.SpellRange)
            {
                bool casted = SpellSelected.CastSpell(TargetList[0].Position, _caster);
                var spellCasted = SpellSelected;
                EndTargetting();
                return (casted, spellCasted);
            }
            GameLoop.UIManager.MessageLog.Add("The target is too far!");
            return (false, null);
        }

        public void EndTargetting()
        {
            if (Cursor.CurrentMap is not null)
            {
                State = TargetState.Resting;
                TargetList.Clear();
                SpellSelected = null;
                _caster = null;

                if (TravelPath is not null)
                {
                    // if there is anything in the path, clear it
                    foreach (Point point in TravelPath.Steps)
                    {
                        var tile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(point);
                        tile.CopyAppearanceFrom(tile.LastSeenAppereance);
                    }
                    TravelPath = null;
                }

                GameLoop.World.ChangeControlledEntity(GameLoop.World.Player);
                GameLoop.World.CurrentMap.Remove(Cursor);
            }
        }

        private (bool, SpellBase) AffectPath()
        {
            if (TravelPath.Length >= 1)
            {
                bool sucess = SpellSelected.CastSpell(TravelPath.Steps.ToList(), _caster);

                var casted = SpellSelected;

                EndTargetting();

                return (sucess, casted);
            }

            return (false, null);
        }

        private void LineTargetting()
        {
            Cursor.Moved += (_, __) =>
            {
                TravelPath = GameLoop.World.CurrentMap.AStar.ShortestPath(OriginCoord, __.NewValue);
                foreach (Point pos in TravelPath.Steps)
                {
                    // gets each point in the travel path steps and change the background of the wall
                    var halp = GameLoop.World.CurrentMap.GetTileAt<TileBase>(pos);
                    halp.Background = Color.Yellow;
                    tileDictionary.TryAdd(pos, halp);
                }
                // This loops makes sure that all the pos that aren't in the TravelPath gets it's proper appearence
                foreach (Point item in tileDictionary.Keys)
                {
                    if (!TravelPath.Steps.Contains(item))
                    {
                        TileBase llop = GameLoop.World.CurrentMap.GetTileAt<TileBase>(item);
                        llop.Background = llop.LastSeenAppereance.Background;
                    }
                }
            };
        }

        public void LookTarget()
        {
            LookWindow w = new LookWindow(TargetList[0]);
            w.Show();
        }

        public enum TargetState
        {
            Resting,
            Targeting
        }
    }
}