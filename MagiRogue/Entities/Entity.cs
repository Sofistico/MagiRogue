using GoRogue;
using GoRogue.GameFramework;
using MagiRogue.Entities.Materials;
using SadRogue.Primitives;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MagiRogue.System.Magic;
using GoRogue.Components;
using GoRogue.SpatialMaps;

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

        /// <summary>
        /// The weight of the entity in kg
        /// </summary>
        public float Weight { get; set; }

        [DataMember]
        public Material Material { get; set; }

        [DataMember]

        /// <summary>
        /// The size of the entity in meters
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Determines whetever the entity leaves an ghost when it leaves the fov
        /// </summary>
        public bool LeavesGhost { get; set; } = true;

        [DataMember]
        public string Description { get; set; }

        public Magic Magic { get; set; }

        #region BackingField fields

        public Map CurrentMap => backingField.CurrentMap;

        public bool IsTransparent { get => backingField.IsTransparent; set => backingField.IsTransparent = value; }
        public bool IsWalkable { get => backingField.IsWalkable; set => backingField.IsWalkable = value; }
        Point IGameObject.Position { get => backingField.Position; set => backingField.Position = value; }

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
            base.Position = backingField.Position;

            this.Moved += SadMoved;
            backingField.Moved += GoRogueMoved;

            Magic = new Magic();
            Material = new Material();
        }

        private void GoRogueMoved(object sender, GameObjectPropertyChanged<Point> e)
        {
            if (backingField.Position != base.Position)
            {
                base.Position = backingField.Position;
            }
        }

        private void SadMoved(object sender, GameObjectPropertyChanged<Point> e)
        {
            if (base.Position != backingField.Position)
            {
                backingField.Position = base.Position;

                // In this case, GoRogue wouldn't allow the position set, so set SadConsole's position back to the way it was
                // to keep them in sync.  Since GoRogue's position never changed, this won't infinite loop.
                if (backingField.Position != base.Position)
                {
                    base.Position = backingField.Position;
                }
            }
        }

        private string DebuggerDisplay
        {
            get
            {
                return string.Format($"{nameof(Entity)} : {Name}");
            }
        }

        public ITaggableComponentCollection GoRogueComponents => backingField.GoRogueComponents;

        /// <summary>
        /// Returns a GoRogue component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetGoRogueComponent<T>() where T : GoRogue.GameFramework.Components.IGameObjectComponent
        {
            return backingField.GoRogueComponents.GetFirst<T>();
        }

        #endregion Helper Methods

        #region IGameObject Interface

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

        public event EventHandler<GameObjectPropertyChanged<Point>> Moved
        {
            add
            {
                backingField.Moved += value;
            }

            remove
            {
                backingField.Moved -= value;
            }
        }

        public void OnMapChanged(Map newMap)
        {
            backingField.OnMapChanged(newMap);
        }

        public void AddComponent(params object[] components)
        {
            foreach (object component in components)
                AddComponent(component);
        }

        #endregion IGameObject Interface
    }
}