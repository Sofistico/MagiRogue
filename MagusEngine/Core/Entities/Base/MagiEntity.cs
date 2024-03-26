using MagusEngine.Bus.ComponentBus;
using MagusEngine.Core.Magic;
using MagusEngine.Services;
using MagusEngine.Systems.Physics;
using SadConsole.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MagusEngine.Core.Entities.Base
{
    // Extends the SadConsole.Entities.Entity class by adding the IGameObject of SadConsole
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class MagiEntity : MagiGameObject
    {
        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"{nameof(MagiEntity)} : {Name}");
            }
        }

        #region Fields

        public MapStuff.MagiMap? CurrentMagiMap => (MapStuff.MagiMap?)CurrentMap;

        public int HistoryId { get; set; } // stores the history id of the entitiy
        public string? Name { get; set; }

        // I think it's in kilograms
        public virtual double Weight { get; set; }

        public double Mass => Weight / PhysicsConstants.PlanetGravity;

        /// <summary>
        /// The size is defined as cm³
        /// </summary>
        public virtual int Volume { get; set; }

        /// <summary>
        /// The height in cm
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The length in cm
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The broadness in cm
        /// </summary>
        public int Broadness { get; set; }

        /// <summary>
        /// Determines whetever the entity leaves an ghost when it leaves the fov
        /// </summary>
        public bool LeavesGhost { get; set; } = true;

        public string? Description { get; set; }

        public MagicManager Magic { get; set; }

        /// <summary>
        /// Defines if this entity can be killed
        /// </summary>
        public bool CanBeKilled { get; set; } = true;

        /// <summary>
        /// Defines if a entity can target or be attacked by this actor
        /// </summary>
        public bool CanBeAttacked { get; set; } = true;

        /// <summary>
        /// Defines if the entity can interact with it's surrondings
        /// </summary>
        public bool CanInteract { get; set; } = true;

        /// <summary>
        /// Defines if the entity ignores wall and can phase though
        /// </summary>
        public bool IgnoresWalls { get; set; }
        public bool AlwaySeen { get; set; }

        public Entity SadCell { get; set; }

        #endregion Fields

        #region Constructor

        public MagiEntity(Color foreground, Color background,
            int glyph, Point coord, int layer) : base(coord, layer, true, true,
                Locator.GetService<IDGenerator>() is not null ? Locator.GetService<IDGenerator>().UseID : null)
        {
            SadCell = new(foreground, background, glyph, layer)
            {
                Position = coord
            };
            Magic = new MagicManager();

            PositionChanged += MagiEntity_PositionChanged;
        }

        #endregion Constructor

        #region events

        private void MagiEntity_PositionChanged(object? sender, ValueChangedEventArgs<Point> e)
        {
            SadCell.Position = e.NewValue;
        }

        #endregion events

        #region Virtual Methods

        public virtual string? GetDescriptor()
        {
            return Description;
        }

        public virtual string GetCurrentStatus()
        {
            throw new ApplicationException("Must define the get status for the class!");
        }

        public virtual int GetShapingAbility(string shapingAbility)
        {
            return 0;
        }

        public virtual int GetPenetration()
        {
            return 0;
        }

        public virtual int GetMagicResistance()
        {
            return 0;
        }

        public virtual MagiEntity Copy()
        {
            var entity = new MagiEntity(SadCell.AppearanceSingle!.Appearance.Foreground,
                SadCell.AppearanceSingle.Appearance.Background,
                SadCell.AppearanceSingle.Appearance.Glyph,
                Position,
                Layer)
            {
                Magic = Magic,
                AlwaySeen = AlwaySeen,
                CanBeAttacked = CanBeAttacked,
                CanBeKilled = CanBeKilled,
                CanInteract = CanInteract,
                Description = Description,
                IgnoresWalls = IgnoresWalls,
                LeavesGhost = LeavesGhost,
                Volume = Volume,
                Weight = Weight,
                Name = Name,
            };

            foreach (var item in GoRogueComponents)
            {
                entity.GoRogueComponents.Add(item.Component, item.Tag);
            }

            return entity;
        }

        /// <summary>
        /// Moves an actor by a delta position change
        /// </summary>
        /// <param name="deltaPositionChange"></param>
        /// <returns></returns>
        public virtual bool MoveBy(Point deltaPositionChange)
        {
            return CurrentMagiMap.IsTileWalkable(Position + deltaPositionChange, IgnoresWalls);
        }

        /// <summary>
        /// Moves the Actor TO newPosition location returns true if actor was able to move, false if
        /// failed to move
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="ignoreWalkable"></param>
        /// <returns></returns>
        public bool MoveTo(Point newPosition, bool ignoreWalkable = false)
        {
            if (CurrentMagiMap?.IsTileWalkable(newPosition, ignoreWalkable) == true)
            {
                Position = newPosition;
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion Virtual Methods

        #region Components

        public void AddComponents<T>(params T[] components) where T : class
        {
            foreach (var component in components)
            {
                if (component is null)
                    continue;
                GoRogueComponents.Add(component);
                Locator.GetService<MessageBusService>()?.SendMessage<ComponentAddedCommand<T>>(new(ID, component));
            }
        }

        public void AddComponent<T>(T component, string? tag = null) where T : class
        {
            GoRogueComponents.Add(component, tag);
            Locator.GetService<MessageBusService>().SendMessage<ComponentAddedCommand<T>>(new(ID, component));
        }

        public T GetComponent<T>() where T : class
            => GoRogueComponents?.GetFirstOrDefault<T>()!;

        public bool GetComponent<T>(out T component, string? tag = null) where T : class
        {
            component = GoRogueComponents?.GetFirstOrDefault<T>(tag)!;
            return component != null;
        }

        public IEnumerable<T> GetComponents<T>() where T : class
            => GoRogueComponents.GetAll<T>();

        public void RemoveComponent<T>() where T : class
        {
            if (GetComponent(out T comp))
            {
                GoRogueComponents.Remove(comp);
                Locator.GetService<MessageBusService>().SendMessage<ComponentRemovedCommand>(new(ID));
            }
        }

        public void RemoveComponent(string tag)
        {
            try
            {
                GoRogueComponents.Remove(tag);
                Locator.GetService<MessageBusService>().SendMessage<ComponentRemovedCommand>(new(ID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion Components

        #region Deconstructor

        ~MagiEntity()
        {
            PositionChanged -= MagiEntity_PositionChanged;
        }

        #endregion Deconstructor
    }
}
