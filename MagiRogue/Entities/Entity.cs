﻿using Microsoft.Xna.Framework;
using SadConsole.Components;
using GoRogue.GameFramework;
using GoRogue;
using System;
using System.Collections.Generic;

namespace MagiRogue.Entities
{
    // Extends the SadConsole.Entities.Entity class
    // by adding an ID to it using GoRogue's ID system
    public abstract class Entity : SadConsole.Entities.Entity, GoRogue.IHasID, IGameObject
    {
        public uint ID { get; private set; } // stores the entity's unique identification number
        public int Layer { get; set; } // stores and sets the layer that the entity is rendered

        public Map CurrentMap => backingField.CurrentMap;

        public bool IsStatic => backingField.IsStatic;

        public bool IsTransparent { get => backingField.IsTransparent; set => backingField.IsTransparent = value; }
        public bool IsWalkable { get => backingField.IsWalkable; set => backingField.IsWalkable = value; }
        Coord IGameObject.Position { get => backingField.Position; set => backingField.Position = value; }

        private IGameObject backingField;

        protected Entity(Color foreground, Color background, int glyph, int layer, int width = 1, int height = 1) : base(width, height)
        {
            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;

            Layer = layer;

            // Create a new unique identifier for this entity
            ID = System.Map.IDGenerator.UseID();

            // Ensure that the entity position/offset is tracked by scrollingconsoles
            Components.Add(new EntityViewSyncComponent());

            backingField = new GameObject(Position, layer, this);
        }

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