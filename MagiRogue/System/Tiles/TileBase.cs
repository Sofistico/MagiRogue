using MagiRogue.Entities.Materials;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using GoRogue.GameFramework;
using GoRogue;

namespace MagiRogue.System.Tiles
{
    public abstract class TileBase : Cell, GoRogue.GameFramework.IGameObject
    {
        private int _tileHealth;
        private bool _destroyed;

        // Movement and Line of Sight Flags
        /// <summary>
        /// It's really complicated the relation between IsBlockingMove and the IsWalkable field from the backing field, but
        /// it can be said that IsWalkable = !IsBlockingMove.
        /// </summary>
        public bool IsBlockingMove;
        public bool TileIsTransparent;
        public int Layer;

        private readonly IGameObject backingField;

        // Creates a list of possible materials, and then assings it to the tile, need to move it to a fitting area, like
        // World or GameLoop, because if need to port, every new object will have more than one possible material without
        // any need.
        public Material MaterialOfTile { get; set; }

        // TOOD: Add a way to mine terrain, to make the tile health drop to zero and give some item.
        /// <summary>
        /// The health of the tile, if it gets to zero, the tiles gets destroyed and becomes a floor of the same material
        /// , a way to abstract the debris
        /// </summary>
        public int TileHealth
        {
            get => _tileHealth;
            set
            {
                if (value <= 0)
                {
                    _tileHealth = 0;
                    _destroyed = true;
                }
                _tileHealth = value;
            }
        }

        // Tile's name
        public string Name;

        #region backingField Data

        public GoRogue.GameFramework.Map CurrentMap => backingField.CurrentMap;

        public bool IsStatic => backingField.IsStatic;

        public bool IsTransparent { get => backingField.IsTransparent; set => backingField.IsTransparent = value; }
        public bool IsWalkable { get => !IsBlockingMove; set => backingField.IsWalkable = !IsBlockingMove; }
        public Coord Position { get => backingField.Position; set => backingField.Position = value; }

        public uint ID => backingField.ID;

        int IHasLayer.Layer => backingField.Layer;

        #endregion backingField Data

        // TileBase is an abstract base class
        // representing the most basic form of of all Tiles used.
        // Every TileBase has a Foreground Colour, Background Colour, and Glyph
        // isBlockingMove and isBlockingSight are optional parameters, set to false by default
        public TileBase(Color foregroud, Color background, int glyph, int layer, Coord position, string idOfMaterial, bool blocksMove = true,
            bool isTransparent = true, string name = "ForgotToChangeName") : base(foregroud, background, glyph)
        {
            IsBlockingMove = blocksMove;
            TileIsTransparent = isTransparent;
            Name = name;
            Layer = layer;
            backingField = new GameObject(position, layer, parentObject: this, isStatic: true, !blocksMove, isTransparent);
            MaterialOfTile = GameLoop.PhysicsManager.SetMaterial(idOfMaterial, MaterialOfTile);
        }

        protected void CalculateTileHealth() => _tileHealth = (int)MaterialOfTile.Density * MaterialOfTile.Hardness;

        #region IGameObject Interface

        public event EventHandler<ItemMovedEventArgs<IGameObject>> Moved
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

        public void OnMapChanged(GoRogue.GameFramework.Map newMap)
        {
            backingField.OnMapChanged(newMap);
        }

        public void AddComponent(object component)
        {
            backingField.AddComponent(component);
        }

        public T GetComponent<T>()
        {
            return backingField.GetComponent<T>();
        }

        public IEnumerable<T> GetComponents<T>()
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