﻿using GoRogue;
using GoRogue.GameFramework;
using MagiRogue.Entities.Materials;
using SadRogue.Primitives;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MagiRogue.System.Magic;
using System.Linq;
using GoRogue.Components;
using GoRogue.SpatialMaps;
using GoRogue.Components.ParentAware;
using SadConsole;
using MagiRogue.System.Tiles;

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
        Point IGameObject.Position { get => Position; set => Position = value; }
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

            Magic = new Magic();
            Material = new Material();

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