using MagusEngine.Bus.ComponentBus;
using MagusEngine.Core.Magic;
using MagusEngine.Services;
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

        public MapStuff.MagiMap? MagiMap => (MapStuff.MagiMap?)CurrentMap;

        public int HistoryId { get; set; } // stores the history id of the entitiy
        public string? Name { get; set; }

        public virtual double Weight { get; set; }

        /// <summary>
        /// The size is defined as cm³
        /// </summary>
        public int Volume { get; set; }

        public int Height { get; set; }
        public int Length { get; set; }
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

        #endregion Components

        #region Deconstructor

        ~MagiEntity()
        {
            PositionChanged -= MagiEntity_PositionChanged;
        }

        #endregion Deconstructor
    }
}
