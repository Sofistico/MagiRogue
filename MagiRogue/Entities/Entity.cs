using Microsoft.Xna.Framework;
using SadConsole.Components;
using GoRogue.GameFramework;
using GoRogue;
using System;
using System.Collections.Generic;

namespace MagiRogue.Entities
{
    // Extends the SadConsole.Entities.Entity class
    // by adding an ID to it using GoRogue's ID system
    public class Entity : SadConsole.Entities.Entity, IGameObject
    {
        #region Fields

        public uint ID { get; private set; } // stores the entity's unique identification number
        public int Layer { get; set; } // stores and sets the layer that the entity is rendered

        #region BackingField fields

        public Map CurrentMap => backingField.CurrentMap;

        public bool IsStatic => backingField.IsStatic;

        public bool IsTransparent { get => backingField.IsTransparent; set => backingField.IsTransparent = value; }
        public bool IsWalkable { get => backingField.IsWalkable; set => backingField.IsWalkable = value; }
        Coord IGameObject.Position { get => backingField.Position; set => backingField.Position = value; }

        #endregion BackingField fields

        private IGameObject backingField;
        private EntityViewSyncComponent entityViewSync;

        #endregion Fields

        #region Constructor

        public Entity(Color foreground, Color background, int glyph, Coord coord, int layer, int width = 1, int height = 1) : base(width, height)
        {
            InitializeObject(foreground, background, glyph, coord, layer);
        }

        #endregion Constructor

        #region Helper Methods

        private void InitializeObject(Color foreground, Color background, int glyph, Coord coord, int layer)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            // Create a new unique identifier for this entity
            //ID = System.Map.IDGenerator.UseID();

            Layer = layer;

            // Ensure that the entity position/offset is tracked by scrollingconsoles
            entityViewSync = new EntityViewSyncComponent();
            Components.Add(entityViewSync);
            entityViewSync.HandleIsVisible = false;

            backingField = new GameObject(coord, layer, this);
            base.Position = backingField.Position;

            base.Moved += SadMoved;
            backingField.Moved += GoRogueMoved;
        }

        private void GoRogueMoved(object sender, ItemMovedEventArgs<IGameObject> e)
        {
            if (backingField.Position != base.Position)
            {
                base.Position = backingField.Position;
            }
        }

        private void SadMoved(object sender, EntityMovedEventArgs e)
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

        #endregion Helper Methods

        #region IGameObject Interface

        event EventHandler<ItemMovedEventArgs<IGameObject>> IGameObject.Moved
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

        public bool MoveIn(Direction direction)
        {
            return backingField.MoveIn(direction);
        }

        public void OnMapChanged(Map newMap)
        {
            backingField.OnMapChanged(newMap);
        }

        public void AddComponent(object component)
        {
            backingField.AddComponent(component);
        }

        T IHasComponents.GetComponent<T>()
        {
            return backingField.GetComponent<T>();
        }

        IEnumerable<T> IHasComponents.GetComponents<T>()
        {
            return backingField.GetComponents<T>();
        }

        public bool HasComponent(Type componentType)
        {
            return backingField.HasComponent(componentType);
        }

        public bool HasComponent<T>()
        {
            return backingField.HasComponent<T>();
        }

        public bool HasComponents(params Type[] componentTypes)
        {
            return backingField.HasComponents(componentTypes);
        }

        public void RemoveComponent(object component)
        {
            backingField.RemoveComponent(component);
        }

        public void RemoveComponents(params object[] components)
        {
            backingField.RemoveComponents(components);
        }

        #endregion IGameObject Interface
    }
}