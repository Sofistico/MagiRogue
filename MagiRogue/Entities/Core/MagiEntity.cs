using GoRogue.Components;
using GoRogue.GameFramework;
using MagiRogue.GameSys.Magic;
using SadRogue.Primitives;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace MagiRogue.Entities.Core
{
    // Extends the SadConsole.Entities.Entity class
    // by adding the IGameObject of SadConsole
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class MagiEntity : SadConsole.Entities.Entity, IGameObject
    {
        #region Fields

        public uint ID => backingField.ID; // stores the entity's unique identification number
        public int Layer { get; set; } // stores and sets the layer that the entity is rendered

        public int HistoryId { get; set; } // stores the history id of the entitiy

        [DataMember]
        public virtual double Weight { get; set; }

        /// <summary>
        /// The size is defined as cm³
        /// </summary>
        [DataMember]
        public int Volume { get; set; }

        public int Height { get; set; }
        public int Length { get; set; }
        public int Broadness { get; set; }

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
        public bool AlwaySeen { get; set; }

        #region BackingField fields

        public Map CurrentMap => backingField.CurrentMap;

        public bool IsTransparent { get => backingField.IsTransparent; set => backingField.IsTransparent = value; }
        public bool IsWalkable { get => backingField.IsWalkable; set => backingField.IsWalkable = value; }
        public IComponentCollection GoRogueComponents => backingField.GoRogueComponents;

        #endregion BackingField fields

        private IGameObject backingField;

        #endregion Fields

        #region Constructor

        public MagiEntity(Color foreground, Color background,
            int glyph, Point coord, int layer) : base(foreground, background, glyph, layer)
        {
            InitializeObject(foreground, background, glyph, coord, layer);
        }

        #endregion Constructor

        #region Helper Methods

        private void InitializeObject(
            Color foreground, Color background, int glyph, Point coord, int layer)
        {
            AppearanceSingle.Appearance.Foreground = foreground;
            AppearanceSingle.Appearance.Background = background;
            AppearanceSingle.Appearance.Glyph = glyph;
            Layer = layer;

            backingField = new GameObject(coord, layer, idGenerator: Locator.GetService<IDGenerator>().UseID);
            Position = backingField.Position;

            //PositionChanged += Position_Changed;

            Magic = new MagicManager();
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"{nameof(MagiEntity)} : {Name}");
            }
        }

        #region temporary

        protected override void OnPositionChanged(Point oldPosition, Point newPosition)
        {
            var args = new ValueChangedEventArgs<Point>(oldPosition, newPosition);

            PositionablePositionChanging?.Invoke(this, args);
            base.OnPositionChanged(oldPosition, newPosition);
            PositionablePositionChanged?.Invoke(this, args);
        }

        #endregion temporary

        #endregion Helper Methods

        #region Virtual Methods

        public virtual string GetDescriptor()
        {
            return Description;
        }

        public virtual string GetCurrentStatus()
        {
            throw new ApplicationException("Must define the get status for the class!");
        }

        #endregion Virtual Methods

        #region Overload Methods

        public virtual MagiEntity Copy()
        {
            var entity = new MagiEntity(AppearanceSingle.Appearance.Foreground,
                AppearanceSingle.Appearance.Background, AppearanceSingle.Appearance.Glyph, Position, Layer)
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
                ZIndex = ZIndex,
                Name = Name,
            };

            foreach (var item in GoRogueComponents)
            {
                entity.GoRogueComponents.Add(item.Component, item.Tag);
            }

            return entity;
        }

        #endregion Overload Methods

        #region IGameObject Interface

        #region temporary

        private event EventHandler<ValueChangedEventArgs<Point>>? PositionablePositionChanging;

        event EventHandler<ValueChangedEventArgs<Point>>? IPositionable.PositionChanging
        {
            add => PositionablePositionChanging += value;
            remove => PositionablePositionChanging -= value;
        }

        private event EventHandler<ValueChangedEventArgs<Point>>? PositionablePositionChanged;

        /// <inheritdoc />
        event EventHandler<ValueChangedEventArgs<Point>>? IPositionable.PositionChanged
        {
            add => PositionablePositionChanged += value;
            remove => PositionablePositionChanged -= value;
        }

        #endregion temporary

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

        public event EventHandler<ValueChangedEventArgs<bool>> TransparencyChanging
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

        public event EventHandler<ValueChangedEventArgs<bool>> TransparencyChanged
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

        public event EventHandler<ValueChangedEventArgs<bool>> WalkabilityChanging
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

        public event EventHandler<ValueChangedEventArgs<bool>> WalkabilityChanged
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
            {
                backingField.GoRogueComponents.Add(component);
                //Locator.GetService<MessageBusService>().SendMessage(component);
            }
        }

        public T GetComponent<T>() where T : class
            => backingField.GoRogueComponents.GetFirstOrDefault<T>();

        public T[] GetComponents<T>() where T : class
            => backingField.GoRogueComponents.OfType<T>().ToArray();

        #endregion IGameObject Interface
    }
}