using GoRogue;
using GoRogue.GameFramework;
using MagiRogue.Entities.Materials;
using SadRogue.Primitives;
using SadConsole;
using System;
using System.Collections.Generic;
using GoRogue.Components;
using GoRogue.SpatialMaps;
using GoRogue.Components.ParentAware;

namespace MagiRogue.System.Tiles
{
    public abstract class TileBase : ColoredGlyph, IGameObject
    {
        private int _tileHealth;
        private bool _destroyed;
        private readonly IGameObject backingField;
        private int _infusedMp;

        // Movement and Line of Sight Flags
        /// <summary>
        /// It's really complicated the relation between IsBlockingMove and the IsWalkable field from the backing field, but
        /// it can be said that IsWalkable = !IsBlockingMove.
        /// </summary>
        public bool IsBlockingMove { get; set; }
        public int Layer { get; set; }

        public ColoredGlyph LastSeenAppereance { get; set; }

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
        public string Name { get; set; }

        public int InfusedMp
        {
            get
            {
                return _infusedMp;
            }

            set
            {
                if (MaterialOfTile.MPInfusionLimit is object && MaterialOfTile.MPInfusionLimit > 0)
                {
                    _infusedMp = value;
                }
                else
                {
                    _infusedMp = 0;
                }
            }
        }

        #region backingField Data

        public GoRogue.GameFramework.Map CurrentMap => backingField.CurrentMap;

        public bool IsTransparent { get => backingField.IsTransparent; set => backingField.IsTransparent = value; }
        public bool IsWalkable { get => !IsBlockingMove; set => backingField.IsWalkable = !IsBlockingMove; }
        public Point Position { get => backingField.Position; set => backingField.Position = value; }

        public uint ID => backingField.ID;

        int IHasLayer.Layer => backingField.Layer;

        //public ITaggableComponentCollection GoRogueComponents => backingField.GoRogueComponents;

        public IComponentCollection GoRogueComponents => backingField.GoRogueComponents;

        #endregion backingField Data

        // TileBase is an abstract base class
        // representing the most basic form of of all Tiles used.
        // Every TileBase has a Foreground Colour, Background Colour, and Glyph
        // isBlockingMove and isBlockingSight are optional parameters, set to false by default
        protected TileBase(Color foregroud, Color background, int glyph, int layer,
            Point position, string idOfMaterial, bool blocksMove = true,
            bool isTransparent = true, string name = "ForgotToChangeName") : base(foregroud, background, glyph)
        {
            IsBlockingMove = blocksMove;
            Name = name;
            Layer = layer;
            backingField = new GameObject(position, layer, !blocksMove, isTransparent);
            MaterialOfTile = System.Physics.PhysicsManager.SetMaterial(idOfMaterial);
            LastSeenAppereance = new ColoredGlyph(Foreground, Background, Glyph)
            {
                IsVisible = false
            };
        }

        protected void CalculateTileHealth() => _tileHealth = (int)MaterialOfTile.Density * MaterialOfTile.Hardness;

        public virtual void DestroyTile(TileBase changeTile, Entities.Item itemDropped)
        {
            if (_destroyed)
            {
                GameLoop.World.CurrentMap.SetTerrain(changeTile);
                GameLoop.World.CurrentMap.Add(itemDropped);
            }
        }

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

        event EventHandler<GameObjectPropertyChanged<Point>> IGameObject.Moved
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

        public void OnMapChanged(GoRogue.GameFramework.Map newMap)
        {
            backingField.OnMapChanged(newMap);
        }

        #endregion IGameObject Interface
    }
}