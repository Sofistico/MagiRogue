using GoRogue.Components;
using GoRogue.GameFramework;
using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Magic;
using MagiRogue.Utils;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace MagiRogue.Entities
{
    // Extends the SadConsole.Entities.Entity class
    // by adding an ID to it using GoRogue's ID system
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Entity : SadConsole.Entities.Entity, IGameObject
    {
        #region Fields

        public uint ID => backingField.ID; // stores the entity's unique identification number
        public int Layer { get; set; } // stores and sets the layer that the entity is rendered

        [DataMember]
        public virtual double Weight { get; set; }

        /// <summary>
        /// The size is defined as cm³
        /// </summary>
        [DataMember]
        public int Volume { get; set; }

        /// <summary>
        /// Determines whetever the entity leaves an ghost when it leaves the fov
        /// </summary>
        public bool LeavesGhost { get; set; } = true;

        [DataMember]
        public string Description { get; set; }

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

        #region BackingField fields

        public Map CurrentMap => backingField.CurrentMap;

        public bool IsTransparent { get => backingField.IsTransparent; set => backingField.IsTransparent = value; }
        public bool IsWalkable { get => backingField.IsWalkable; set => backingField.IsWalkable = value; }
        public IComponentCollection GoRogueComponents => backingField.GoRogueComponents;

        #endregion BackingField fields

        private IGameObject backingField;

        #endregion Fields

        #region Constructor

        public Entity(Color foreground, Color background,
            int glyph, Point coord, int layer) : base(foreground, background, glyph, layer)
        {
            InitializeObject(foreground, background, glyph, coord, layer);
        }

        #endregion Constructor

        #region Helper Methods

        private void InitializeObject(
            Color foreground, Color background, int glyph, Point coord, int layer)
        {
            Appearance.Foreground = foreground;
            Appearance.Background = background;
            Appearance.Glyph = glyph;
            Layer = layer;

            backingField = new GameObject(coord, layer);
            Position = backingField.Position;

            PositionChanged += Position_Changed;

            Magic = new MagicManager();

            //IsWalkable = false;
        }

#nullable enable

        private void Position_Changed(object? sender, ValueChangedEventArgs<Point> e)
            => Moved?.Invoke(sender, new GameObjectPropertyChanged<Point>(this, e.OldValue, e.NewValue));

#nullable disable

        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"{nameof(Entity)} : {Name}");
            }
        }

        public bool AlwaySeen { get; internal set; }

        #endregion Helper Methods

        #region Overload Methods

        public virtual Entity Copy()
        {
            var entity = new Entity(this.Appearance.Foreground,
                Appearance.Background, Appearance.Glyph, Position, Layer)
            {
                Magic = this.Magic,
                AlwaySeen = this.AlwaySeen,
                CanBeAttacked = CanBeAttacked,
                CanBeKilled = CanBeKilled,
                CanInteract = CanInteract,
                Description = Description,
                IgnoresWalls = IgnoresWalls,
                LeavesGhost = LeavesGhost,
                Volume = Volume,
                Weight = Weight,
                ZIndex = ZIndex,
                Name = Name,
            };

            foreach (var item in this.GoRogueComponents)
            {
                entity.GoRogueComponents.Add(item.Component, item.Tag);
            }

            return entity;
        }

        #endregion Overload Methods

        #region IGameObject Interface

#nullable enable

        public event EventHandler<GameObjectPropertyChanged<Point>>? Moved;

#nullable disable

        public event EventHandler<GameObjectCurrentMapChanged> AddedToMap
        {
            add
            {
                backingField.AddedToMap += value;
            }

            remove
            {
                backingField.AddedToMap -= value;
            }
        }

        public event EventHandler<GameObjectCurrentMapChanged> RemovedFromMap
        {
            add
            {
                backingField.RemovedFromMap += value;
            }

            remove
            {
                backingField.RemovedFromMap -= value;
            }
        }

        public event EventHandler<GameObjectPropertyChanged<bool>> TransparencyChanging
        {
            add
            {
                backingField.TransparencyChanging += value;
            }

            remove
            {
                backingField.TransparencyChanging -= value;
            }
        }

        public event EventHandler<GameObjectPropertyChanged<bool>> TransparencyChanged
        {
            add
            {
                backingField.TransparencyChanged += value;
            }

            remove
            {
                backingField.TransparencyChanged -= value;
            }
        }

        public event EventHandler<GameObjectPropertyChanged<bool>> WalkabilityChanging
        {
            add
            {
                backingField.WalkabilityChanging += value;
            }

            remove
            {
                backingField.WalkabilityChanging -= value;
            }
        }

        public event EventHandler<GameObjectPropertyChanged<bool>> WalkabilityChanged
        {
            add
            {
                backingField.WalkabilityChanged += value;
            }

            remove
            {
                backingField.WalkabilityChanged -= value;
            }
        }

        public void OnMapChanged(Map newMap)
        {
            backingField.OnMapChanged(newMap);
        }

        public void AddComponent(params object[] components)
        {
            foreach (object component in components)
                backingField.GoRogueComponents.Add(component);
        }

        #endregion IGameObject Interface
    }
}