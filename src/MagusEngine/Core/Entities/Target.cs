using Arquimedes.Enumerators;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using MagusEngine.Actions;
using MagusEngine.Bus.MapBus;
using MagusEngine.Bus.UiBus;
using MagusEngine.Core.Entities.Base;
using MagusEngine.Core.Magic;
using MagusEngine.Core.MapStuff;
using MagusEngine.Services;
using MagusEngine.Systems;
using MagusEngine.Systems.Time;
using MagusEngine.Utils;
using SadConsole.Effects;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagusEngine.Core.Entities
{
    /// <summary>
    /// Defines an target system.
    /// </summary>
    public class Target
    {
        private Actor? _caster;
        private MagiEntity? _lastControlledEntity;
        private readonly Dictionary<Point, Tile> _tileDictionary;
        private Spell? _selectedSpell;
        private Item? _selectedItem;

        public MagiEntity Cursor { get; set; }

        public Point OriginCoord { get; set; }

        public TargetState State { get; set; }

        public Path? TravelPath { get; set; }

        public int MaxDistance { get; private set; } = 999;

        public Point Position => Cursor.Position;

        public Target(Point spawnCoord)
        {
            Color targetColor = new(255, 0, 0);

            OriginCoord = spawnCoord;

            Cursor = new Actor("Target Cursor",
                targetColor,
                Color.AnsiYellow,
                'X',
                spawnCoord,
                (int)MapLayer.SPECIAL)
            {
                IsWalkable = true,
                CanBeKilled = false,
                CanBeAttacked = false,
                CanInteract = false,
                LeavesGhost = false,
                AlwaySeen = true
            };

            Blink blink = new()
            {
                BlinkCount = -1,
                BlinkSpeed = TimeSpan.FromSeconds(0.5),
                BlinkOutColor = Color.Aquamarine,
                UseCellBackgroundColor = false,
            };
            Cursor.SadCell.AppearanceSingle!.Effect = blink;
            blink.Restart();

            _tileDictionary = [];
        }

        public bool EntityInTarget(bool ignorePlayer = true)
        {
            var entity = Cursor?.CurrentMagiMap?.GetEntitiesAt<MagiEntity>(Cursor.Position, Cursor.CurrentMagiMap.LayerMasker.MaskAllBelow((int)MapLayer.SPECIAL)).FirstOrDefault();
            if (entity is null)
                return false;
            if (ignorePlayer && entity is Player)
                return false;
            return true;
        }

        public bool AnyTargeted() => EntityInTarget() || TileInTarget();

        public void OnSelectSpell(Spell spell, Actor caster)
        {
            _selectedSpell = spell;
            _caster = caster;
            if (_selectedSpell.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Self))
            {
                var (sucess, s) = EndSpellTargetting();
                Locator.GetService<MessageBusService>().SendMessage<ProcessTurnEvent>(new(TimeHelper.GetCastingTime(Find.Universe.Player, s), sucess));
                return;
            }

            StartTargetting();
            State = TargetState.TargetingSpell;
            MaxDistance = _selectedSpell?.SpellRange ?? 999;
        }

        public void OnSelectItem(Item item, Actor entity)
        {
            _selectedItem = item;
            _caster = entity;
            StartTargetting();
            State = TargetState.TargetingItem;
            // minimum is always at least one tile
            MaxDistance = (int)MathMagi.FastRound((entity.Body.Strength + 1 + Mrn.Exploding2D6Dice) / item.Weight);
        }

        public void StartTargetting()
        {
            if (State is TargetState.TargetingSpell || State is TargetState.TargetingItem)
                return;
            OriginCoord = new Point(Find.CurrentMap!.ControlledEntitiy!.Position.X, Find.CurrentMap.ControlledEntitiy.Position.Y);
            Cursor.Position = new Point(Find.CurrentMap.ControlledEntitiy.Position.X, Find.CurrentMap.ControlledEntitiy.Position.Y);
            _lastControlledEntity = Find.CurrentMap.ControlledEntitiy;
            Locator.GetService<MessageBusService>()?.SendMessage<ChangeControlledEntitiy>(new(Cursor));
            Locator.GetService<MessageBusService>()?.SendMessage<AddEntitiyCurrentMap>(new(Cursor));

            Cursor.PositionChanged += Cursor_Moved;
            State = TargetState.LookMode;
            Cursor.IgnoresWalls = true;
        }

        // TODO: Customize who should you target
        public (bool, Spell?) EndSpellTargetting()
        {
            bool spellInRange = (int)Distance.Chebyshev.Calculate(OriginCoord, Cursor.Position) <= _selectedSpell?.SpellRange;
            if (_selectedSpell?.Manifestation == SpellManifestation.Instant)
            {
                //if (_selectedSpell?.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Beam) == true)
                //{
                //    return AffectPath();
                //}
                //if (_selectedSpell?.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Cone) == true)
                //{
                //    return AffectArea();
                //}
                //if (_selectedSpell?.Effects.Any(e => e.AreaOfEffect is SpellAreaEffect.Ball) == true)
                //{
                //    return AffectArea();
                //}
                if (spellInRange)
                {
                    return AffectTarget();
                }
            }
            if (spellInRange)
            {
                ActionManager.CastManifestedSpell(_selectedSpell!, _caster, OriginCoord, Cursor.Position);
                var spellCasted = _selectedSpell;
                EndTargetting();
                return (true, spellCasted);
            }
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("The target is too far!"));
            return (false, null);
        }

        public (bool, Item) EndItemTargetting()
        {
            int distance = (int)Distance.Chebyshev.Calculate(OriginCoord, Cursor.Position);

            if (distance <= MaxDistance && _selectedItem is not null)
            {
                ActionManager.ShootProjectileAction(_caster.Position, Cursor.Position, _selectedItem, _caster);
                var item = _selectedItem;
                EndTargetting();
                return (true, item);
            }
            Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("The target is too far!"));
            return (false, null);
        }

        private (bool, Spell) AffectTarget()
        {
            bool casted;
            if (TravelPath is not null)
            {
                casted = _selectedSpell?.CastSpell(TravelPath.End, _caster!) ?? false;
            }
            else
            {
                casted = false;
                Locator.GetService<MessageBusService>().SendMessage<AddMessageLog>(new("An error ocurred, cound't find a target!"));
            }
            Spell spellCasted = _selectedSpell!.Copy();
            EndTargetting();
            return (casted, spellCasted);
        }

        public void EndTargetting()
        {
            if (Cursor.CurrentMap is not null)
            {
                _selectedSpell = null;
                _selectedItem = null;
                _caster = null;

                State = TargetState.LookMode;

                if (TravelPath is not null)
                {
                    ClearTileDictionary();
                    Cursor.PositionChanged -= Cursor_Moved;
                    TravelPath = null;
                }

                Locator.GetService<MessageBusService>().SendMessage<ChangeControlledEntitiy>(new(_lastControlledEntity));
                Locator.GetService<MessageBusService>().SendMessage<RemoveEntitiyCurrentMap>(new(Cursor));
            }
        }

        private void ClearTileDictionary()
        {
            if (_tileDictionary.Count == 0) return;
            // if there is anything in the path, clear it
            foreach (Point point in _tileDictionary.Keys)
            {
                Tile tile = Cursor.CurrentMagiMap.GetTileAt(point);
                if (tile is not null)
                    tile.Appearence.Background = tile.LastSeenAppereance.Background;
            }
            _tileDictionary.Clear();
        }

        //private (bool, Spell?) AffectArea()
        //{
        //    if (_selectedSpell.Effects.Any(e => e.Radius > 0))
        //    {
        //        var allPos = new List<Point>();

        //        foreach (var entity in TargetList)
        //        {
        //            if (entity.CanBeAttacked)
        //                allPos.Add(entity.Position);
        //        }

        //        if (_selectedSpell.AffectsTile)
        //        {
        //            foreach (var pos in _tileDictionary.Keys)
        //            {
        //                if (!allPos.Contains(pos))
        //                    allPos.Add(pos);
        //            }
        //        }

        //        var sucess = _selectedSpell.CastSpell(allPos, _caster);
        //        var spell = _selectedSpell;

        //        EndTargetting();

        //        return (sucess, spell);
        //    }
        //    return (false, null);
        //}

        /// <summary>
        /// Makes the render and the path to the target
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cursor_Moved(object? sender, ValueChangedEventArgs<Point> e)
        {
            if (Cursor is null)
                return;
            TravelPath = Cursor.CurrentMagiMap.AStar.ShortestPath(OriginCoord, e.NewValue)!;
            if (State is TargetState.LookMode || TravelPath is null)
            {
                TravelPath = Cursor.CurrentMagiMap.AStarWithAllWalkable().ShortestPath(OriginCoord, e.NewValue)!;
                ClearTileDictionary();
                Cursor.IgnoresWalls = true;
                return;
            }
            try
            {
                foreach (Point pos in TravelPath.Steps)
                {
                    // gets each point in the travel path steps and change the background of the wall
                    var halp = Cursor.CurrentMagiMap.GetTileAt(pos);
                    halp.Appearence.Background = Color.Yellow;
                    _tileDictionary.TryAdd(pos, halp);
                }

                // This loops makes sure that all the pos that aren't in the TravelPath gets it's
                // proper appearence
                foreach (Point item in _tileDictionary.Keys)
                {
                    if (!TravelPath.Steps.Contains(item))
                    {
                        Tile llop = Cursor.CurrentMagiMap.GetTileAt(item);
                        if (llop is not null)
                        {
                            llop.Appearence.Background = llop.LastSeenAppereance.Background;
                        }
                    }
                }
                if (_selectedSpell is not null)
                {
                    Cursor.IgnoresWalls = _selectedSpell.IgnoresWall;
                    foreach (var point in TargetHelper.SpellAreaHelper(_selectedSpell, Position, e.NewValue)!)
                    {
                        AddTileToDictionary(point, _selectedSpell.IgnoresWall);
                    }
                }
            }
            catch (Exception ex)
            {
                MagiLog.Log(ex, "An error occured in the Cursor targetting!");
                throw;
            }
        }

        public bool TileInTarget()
        {
            if (!EntityInTarget() && Cursor?.CurrentMagiMap?.GetTileAt(Cursor.Position) != null)
            {
                //if (!lookMode)
                //    State = TargetState.TargetingSpell;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the tile to the <see cref="_tileDictionary"/>.
        /// </summary>
        /// <param name="point"></param>
        private void AddTileToDictionary(Point point, bool ignoresWall = false)
        {
            var halp = Cursor.CurrentMagiMap.GetTileAt(point);
            if (halp is not null && (halp.IsTransparent || ignoresWall))
            {
                halp.Appearence.Background = Color.Yellow;
                _tileDictionary.TryAdd(point, halp);
            }
        }

        public void LookTarget()
        {
            if (DetermineWhatToLook() is MagiEntity entity)
            {
                Locator.GetService<MessageBusService>().SendMessage<LookStuff>(new(entity));
            }
        }

        public MagiEntity? TargetEntity() => Cursor.CurrentMagiMap.GetEntityAt<MagiEntity>(Cursor.Position);

        private Tile? TargetAtTile() => Cursor.CurrentMagiMap.GetTileAt(Cursor.Position);

        private IGameObject? DetermineWhatToLook()
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
                throw new InvalidOperationException("Cound't find what to target!");
            }
        }
    }
}
