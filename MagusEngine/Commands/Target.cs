﻿using Arquimedes.Enumerators;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core;
using MagusEngine.Core.Entities;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic;
using MagusEngine.Services;
using MagusEngine.Systems.Time;
using MagusEngine.Utils;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Commands
{
    /// <summary>
    /// Defines an target system.
    /// </summary>
    public class Target
    {
        private Actor _caster;
        private readonly Dictionary<Point, Tile> tileDictionary;
        private static readonly Radius radius = Radius.Circle;

        public MagiEntity Cursor { get; set; }

        public IList<MagiEntity> TargetList { get; set; }

        public Point OriginCoord { get; set; }

        public TargetState State { get; set; }

        public Path TravelPath { get; set; }

        public int MaxDistance => SpellSelected is not null ? SpellSelected.SpellRange : 999;

        public SpellBase SpellSelected { get; set; }

        public bool LookMode { get; set; }

        public Point Position => Cursor.Position;

        public Target(Point spawnCoord)
        {
            Color targetColor = new Color(255, 0, 0);

            OriginCoord = spawnCoord;

            Cursor = new Actor("Target Cursor",
                targetColor, Color.AnsiYellow, 'X', spawnCoord, (int)MapLayer.SPECIAL)
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
                BlinkSpeed = TimeSpan.FromSeconds(0.5),
                BlinkOutColor = Color.Aquamarine,
                UseCellBackgroundColor = false,
            };
            Cursor.AppearanceSingle.Effect = blink;
            blink.Restart();

            State = TargetState.Resting;

            TargetList = new List<MagiEntity>();
            tileDictionary = new Dictionary<Point, Tile>();
        }

        public bool EntityInTarget()
        {
            if (Cursor.MagiMap.GetEntitiesAt
                <MagiEntity>(Cursor.Position).Any(e => e.ID != Cursor.ID))
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
                Locator.GetService<MessageBusService>()
                    .SendMessage<ProcessTurnEvent>(new(TimeHelper.GetCastingTime(GameLoop.Universe.Player, s)), sucess);
                return;
            }

            StartTargetting();
        }

        public void StartTargetting()
        {
            if (State is TargetState.Targeting)
                return;
            OriginCoord = new Point(
                Cursor.MagiMap.ControlledEntitiy.Position.X,
                Cursor.MagiMap.ControlledEntitiy.Position.Y);
            Cursor.Position = new Point(
                 Cursor.MagiMap.ControlledEntitiy.Position.X,
                 Cursor.MagiMap.ControlledEntitiy.Position.Y);
            Locator.GetService<MessageBusService>().SendMessage<ChangeControlledEntitiy>(new(Cursor));
            Cursor.MagiMap.AddMagiEntity(Cursor);
            Cursor.PositionChanged += Cursor_Moved;

            State = TargetState.Targeting;
            LookMode = true;
            Cursor.IgnoresWalls = true;
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
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("The target is too far!"));
            return (false, null);
        }

        private (bool, SpellBase) AffectTarget()
        {
            bool casted;
            if (TargetList.Count > 0)
            {
                casted = SpellSelected.CastSpell(TargetList[0].Position, _caster);
            }
            else if (TravelPath is not null)
            {
                casted = SpellSelected.CastSpell(TravelPath.End, _caster);
            }
            else
            {
                casted = false;
                Locator.GetService<MessageBusService>()
                    .SendMessage<AddMessageLog>(new("An error ocurred, cound't find a target!"));
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
                    ClearTileDictionary();
                    Cursor.PositionChanged -= Cursor_Moved;
                    TravelPath = null;
                }

                Locator.GetService<MessageBusService>().SendMessage<ChangeControlledEntitiy>(new(GameLoop.Universe.Player));
                Locator.GetService<MessageBusService>().SendMessage<RemoveEntitiyCurrentMap>(new(Cursor));
            }
        }

        private void ClearTileDictionary()
        {
            if (tileDictionary.Count == 0) return;
            // if there is anything in the path, clear it
            foreach (Point point in tileDictionary.Keys)
            {
                Tile tile = Cursor.MagiMap.GetTileAt(point);
                if (tile is not null)
                    tile.Appearence.Background = tile.LastSeenAppereance.Background;
            }
            tileDictionary.Clear();
        }

        private (bool, SpellBase?) AffectPath()
        {
            if (TravelPath?.Length >= 1)
            {
                bool sucess = SpellSelected.CastSpell(TravelPath.Steps.ToList(), _caster);

                var casted = SpellSelected;

                EndTargetting();

                return (sucess, casted);
            }

            return (false, null);
        }

        private (bool, SpellBase?) AffectArea()
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
        private void Cursor_Moved(object sender, ValueChangedEventArgs<Point> e)
        {
            TargetList.Clear();
            TravelPath = Cursor.MagiMap.AStar.ShortestPath(OriginCoord, e.NewValue)!;
            if (LookMode || TravelPath is null)
            {
                TravelPath = Cursor.MagiMap
                    .AStarWithAllWalkable().ShortestPath(OriginCoord, e.NewValue)!;
                ClearTileDictionary();
                return;
            }
            try
            {
                foreach (Point pos in TravelPath.Steps)
                {
                    // gets each point in the travel path steps and change the background of the wall
                    var halp = Cursor.MagiMap.GetTileAt(pos);
                    halp.Appearence.Background = Color.Yellow;
                    tileDictionary.TryAdd(pos, halp);
                }

                // This loops makes sure that all the pos that aren't in the TravelPath gets it's
                // proper appearence
                foreach (Point item in tileDictionary.Keys)
                {
                    if (!TravelPath.Steps.Contains(item))
                    {
                        Tile llop = Cursor.MagiMap.GetTileAt(item);
                        if (llop is not null)
                        {
                            llop.Appearence.Background = llop.LastSeenAppereance.Background;
                        }
                    }
                }

                SpellAreaHelper();
            }
            catch (Exception)
            {
                throw new Exception("An error occured in the Cursor targetting!");
            }
        }

        private void SpellAreaHelper()
        {
            if (SpellSelected is not null)
            {
                if (SpellSelected.Effects.Any(a => a.AreaOfEffect is SpellAreaEffect.Ball))
                {
                    var eff = GetSpellAreaEffect(SpellAreaEffect.Ball);
                    if (eff is null) { return; }
                    RadiusLocationContext radiusLocation =
                       new RadiusLocationContext(Cursor.Position, eff.Radius);

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
                        OriginCoord.Cone(effect.Radius, this, effect.ConeCircleSpan).Points)
                    {
                        AddTileToDictionary(point);
                        AddEntityToList(point);
                    }
                }
            }
        }

        public bool TileInTarget()
        {
            if (!EntityInTarget()
                && Cursor.MagiMap.GetTileAt(Cursor.Position) != null)
            {
                State = TargetState.Targeting;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the tile to the <see cref="tileDictionary"/>.
        /// </summary>
        /// <param name="point"></param>
        private void AddTileToDictionary(Point point)
        {
            var halp = Cursor.MagiMap.GetTileAt(point);
            if (halp is not null)
            {
                halp.Appearence.Background = Color.Yellow;
                tileDictionary.TryAdd(point, halp);
            }

            // sanity check
        }

        public bool SpellTargetsTile()
        {
            return SpellSelected is not null && (SpellSelected.Effects.Any(a => a.TargetsTile)
                || SpellSelected.Effects.Any(a => a.AreaOfEffect is not SpellAreaEffect.Target));
        }

        /// <summary>
        /// Adds an entity to the <see cref="TargetList"/>.
        /// </summary>
        /// <param name="point"></param>
        private void AddEntityToList(Point point)
        {
            var entity = Cursor.MagiMap.GetEntityAt<MagiEntity>(point);
            if (entity is not null && !TargetList.Contains(entity))
                TargetList.Add(Cursor.MagiMap.GetEntityAt<MagiEntity>(point));
        }

        private ISpellEffect GetSpellAreaEffect(SpellAreaEffect areaEffect) =>
            SpellSelected.Effects.Find(e => e.AreaOfEffect == areaEffect);

        public void LookTarget()
        {
            if (DetermineWhatToLook() is MagiEntity entity)
            {
                Locator.GetService<MessageBusService>().SendMessage<LookStuff>(new(entity));
            }
        }

        public MagiEntity? TargetEntity() => Cursor.MagiMap.GetEntityAt<MagiEntity>(Cursor.Position);

        private Tile? TargetAtTile() => Cursor.MagiMap.GetTileAt(Cursor.Position);

        public IGameObject DetermineWhatToLook()
        {
            if (EntityInTarget())
            {
                return TargetEntity();
            }
            else if (TileInTarget())
            {
                return TargetAtTile();
            }
            else
            {
                throw new("Cound't find what to target!");
            }
        }
    }
}