using GoRogue.Components;
using GoRogue.Components.ParentAware;
using GoRogue.GameFramework;
using SadRogue.Primitives;
using System;

namespace MagusEngine.Core.MapStuff
{
    public partial class Tile : IGameObject
    {
        private readonly GameObject _gameObject;

        public GoRogue.GameFramework.Map? CurrentMap => ((IGameObject)_gameObject).CurrentMap;

        public bool IsTransparent { get => ((IGameObject)_gameObject).IsTransparent; set => ((IGameObject)_gameObject).IsTransparent = value; }
        public bool IsWalkable { get => ((IGameObject)_gameObject).IsWalkable; set => ((IGameObject)_gameObject).IsWalkable = value; }

        #region IGameObj

        public uint ID => ((IHasID)_gameObject).ID;

        public int Layer => ((IHasLayer)_gameObject).Layer;

        public IComponentCollection GoRogueComponents => ((IObjectWithComponents)_gameObject).GoRogueComponents;

        public Point Position { get => ((IPositionable)_gameObject).Position; set => ((IPositionable)_gameObject).Position = value; }

        public event EventHandler<GameObjectCurrentMapChanged>? AddedToMap
        {
            add
            {
                ((IGameObject)_gameObject).AddedToMap += value;
            }

            remove
            {
                ((IGameObject)_gameObject).AddedToMap -= value;
            }
        }

        public event EventHandler<GameObjectCurrentMapChanged>? RemovedFromMap
        {
            add
            {
                ((IGameObject)_gameObject).RemovedFromMap += value;
            }

            remove
            {
                ((IGameObject)_gameObject).RemovedFromMap -= value;
            }
        }

        public event EventHandler<ValueChangedEventArgs<bool>>? TransparencyChanging
        {
            add
            {
                ((IGameObject)_gameObject).TransparencyChanging += value;
            }

            remove
            {
                ((IGameObject)_gameObject).TransparencyChanging -= value;
            }
        }

        public event EventHandler<ValueChangedEventArgs<bool>>? TransparencyChanged
        {
            add
            {
                ((IGameObject)_gameObject).TransparencyChanged += value;
            }

            remove
            {
                ((IGameObject)_gameObject).TransparencyChanged -= value;
            }
        }

        public event EventHandler<ValueChangedEventArgs<bool>>? WalkabilityChanging
        {
            add
            {
                ((IGameObject)_gameObject).WalkabilityChanging += value;
            }

            remove
            {
                ((IGameObject)_gameObject).WalkabilityChanging -= value;
            }
        }

        public event EventHandler<ValueChangedEventArgs<bool>>? WalkabilityChanged
        {
            add
            {
                ((IGameObject)_gameObject).WalkabilityChanged += value;
            }

            remove
            {
                ((IGameObject)_gameObject).WalkabilityChanged -= value;
            }
        }

        public event EventHandler<ValueChangedEventArgs<Point>>? PositionChanging
        {
            add
            {
                ((IPositionable)_gameObject).PositionChanging += value;
            }

            remove
            {
                ((IPositionable)_gameObject).PositionChanging -= value;
            }
        }

        public event EventHandler<ValueChangedEventArgs<Point>>? PositionChanged
        {
            add
            {
                ((IPositionable)_gameObject).PositionChanged += value;
            }

            remove
            {
                ((IPositionable)_gameObject).PositionChanged -= value;
            }
        }

        public void OnMapChanged(GoRogue.GameFramework.Map? newMap)
        {
            ((IGameObject)_gameObject).OnMapChanged(newMap);
        }

        #endregion IGameObj
    }
}
