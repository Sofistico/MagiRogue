using MagiRogue.Entities;
using MagiRogue.System;
using MagiRogue.System.Magic;
using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;
using GoRogue.Pathing;
using MagiRogue.UI.Windows;
using System;
using MagiRogue.Utils;

namespace MagiRogue.Commands
{
    /// <summary>
    /// Defines an target system.
    /// </summary>
    public class Target
    {
        private Actor _caster;
        private readonly Dictionary<Point, TileBase> tileDictionary;
        private static readonly Radius radius = Radius.Circle;

        public Entity Cursor { get; set; }

        public IList<Entity> TargetList { get; set; }

        public Point OriginCoord { get; set; }

        public TargetState State { get; set; }

        public Path TravelPath { get; set; }

        public int MaxDistance => SpellSelected is not null ? SpellSelected.SpellRange : 999;

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
                LeavesGhost = false,
                AlwaySeen = true
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

        public bool TileInTarget()
        {
            TileBase tile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(Cursor.Position);

            if (tile is not null)
            {
                return true;
            }

            return false;
        }

        public bool EntityInTarget()
        {
            if (GameLoop.World.CurrentMap.GetEntitiesAt<Entity>(Cursor.Position).Any(e => e.ID != Cursor.ID))
            //&& GameLoop.World.CurrentMap.GetEntityAt<Entity>(Cursor.Position) is not Player)
            {
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

            StartTargetting();
        }

        public void StartTargetting()
        {
            GameLoop.World.ChangeControlledEntity(Cursor);
            GameLoop.World.CurrentMap.Add(Cursor);
            Cursor.Moved += Cursor_Moved;

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

            if (SpellSelected.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Ball)
                || SpellSelected.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Cone))
            {
                return AffectArea();
            }

            if (distance <= SpellSelected.SpellRange)
            {
                return AffectTarget();
            }
            GameLoop.UIManager.MessageLog.Add("The target is too far!");
            return (false, null);
        }

        private (bool, SpellBase) AffectTarget()
        {
            bool casted;
            if (TargetList.Count > 0)
                casted = SpellSelected.CastSpell(TargetList[0].Position, _caster);
            else if (TravelPath is not null)
            {
                casted = SpellSelected.CastSpell(TravelPath.End, _caster);
            }
            else
            {
                casted = false;
                GameLoop.UIManager.MessageLog.Add("An error ocurred, cound't find a target!");
            }
            var spellCasted = SpellSelected;
            EndTargetting();
            return (casted, spellCasted);
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
                    foreach (Point point in tileDictionary.Keys)
                    {
                        TileBase tile = GameLoop.World.CurrentMap.GetTileAt<TileBase>(point);
                        if (tile is not null)
                            tile.Background = tile.LastSeenAppereance.Background;
                    }
                    Cursor.Moved -= Cursor_Moved;
                    TravelPath = null;
                }

                GameLoop.World.ChangeControlledEntity(GameLoop.World.Player);
                GameLoop.World.CurrentMap.Remove(Cursor);
            }
        }

        private (bool, SpellBase) AffectPath()
        {
            if (TravelPath is not null && TravelPath.Length >= 1)
            {
                bool sucess = SpellSelected.CastSpell(TravelPath.Steps.ToList(), _caster);

                var casted = SpellSelected;

                EndTargetting();

                return (sucess, casted);
            }

            return (false, null);
        }

        private (bool, SpellBase) AffectArea()
        {
            if (SpellSelected.Effects.Any(e => e.Radius > 0))
            {
                var _spellCasted = SpellSelected;
                var allPosEntities = new List<Point>();

                foreach (var entity in TargetList)
                {
                    if (entity.CanBeAttacked)
                        allPosEntities.Add(entity.Position);
                }

                var sucess = SpellSelected.CastSpell(allPosEntities, _caster);

                EndTargetting();

                return (sucess, _spellCasted);
            }
            return (false, null);
        }

        /// <summary>
        /// Makes the render and the path to the target
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cursor_Moved(object sender, GoRogue.GameFramework.GameObjectPropertyChanged<Point> e)
        {
            TargetList.Clear();
            TravelPath = GameLoop.World.CurrentMap.AStar.ShortestPath(OriginCoord, e.NewValue);
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
                    if (llop is not null)
                    {
                        llop.Background = llop.LastSeenAppereance.Background;
                    }
                }
            }

            SpellAreaHelper();
        }

        private void SpellAreaHelper()
        {
            if (SpellSelected is not null)
            {
                if (SpellSelected.Effects.Any(a => a.AreaOfEffect is SpellAreaEffect.Ball))
                {
                    RadiusLocationContext radiusLocation =
                       new RadiusLocationContext(Cursor.Position, SpellSelected.Effects.FirstOrDefault().Radius);

                    foreach (Point point in radius.PositionsInRadius(radiusLocation))
                    {
                        AddTileToDictionary(point);
                        AddEntityToList(point);
                    }
                }

                if (SpellSelected.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Cone))
                {
                    ISpellEffect effect = GetSpellAreaEffect(SpellAreaEffect.Cone);
                    foreach (Point point in
                        GeometryUtils.Cone(OriginCoord, effect.Radius, this).Points)
                    {
                        AddTileToDictionary(point);
                        AddEntityToList(point);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the tile to the <see cref="tileDictionary"/>.
        /// </summary>
        /// <param name="point"></param>
        private void AddTileToDictionary(Point point)
        {
            var halp = GameLoop.World.CurrentMap.GetTileAt<TileBase>(point);
            if (halp is not null)
            {
                halp.Background = Color.Yellow;
                tileDictionary.TryAdd(point, halp);
            }

            // sanity check
        }

        public bool SpellTargetsTile()
        {
            if (SpellSelected is not null && (SpellSelected.Effects.Any(a => a.TargetsTile)
                || SpellSelected.Effects.Any(a => a.AreaOfEffect is not SpellAreaEffect.Target)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an entity to the <see cref="TargetList"/>.
        /// </summary>
        /// <param name="point"></param>
        private void AddEntityToList(Point point)
        {
            var entity = GameLoop.World.CurrentMap.GetEntityAt<Entity>(point);
            if (entity is not null && !TargetList.Contains(entity))
                TargetList.Add(GameLoop.World.CurrentMap.GetEntityAt<Entity>(point));
        }

        private ISpellEffect GetSpellAreaEffect(SpellAreaEffect areaEffect) =>
            SpellSelected.Effects.Find(e => e.AreaOfEffect == areaEffect);

        public void LookTarget()
        {
            LookWindow w = new(TargetEntity());
            w.Show();
        }

        private Entity TargetEntity() => GameLoop.World.CurrentMap.GetEntityAt<Entity>(Cursor.Position);

        public enum TargetState
        {
            Resting,
            Targeting
        }
    }
}