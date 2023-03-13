using GoRogue.Components;
using GoRogue.GameFramework;
using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Veggies;
using MagiRogue.Utils.Extensions;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.GameSys.Tiles
{
    // need to separate the TileBase from the ColoredGlyph object
    public abstract class TileBase : ColoredGlyph, IGameObject
    {
        private int _tileHealth;
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
        public MaterialTemplate MaterialOfTile { get; set; }

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
                    //_destroyed = true;
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
                if (MaterialOfTile?.MPInfusionLimit is not null
                    && MaterialOfTile.MPInfusionLimit > 0)
                {
                    _infusedMp = value;
                }
                else
                {
                    _infusedMp = 0;
                }
            }
        }

        // TODO: For some future fun stuff!
        // like continious wall and determining what there is next to it
        public int BitMask { get; set; }

        public List<Trait> Traits { get; set; }

        public Plant[] Vegetations { get; set; } = new Plant[4];

        public bool HoldsVegetation { get; set; }

        #region backingField Data

        public GoRogue.GameFramework.Map CurrentMap => backingField.CurrentMap;

        public bool IsTransparent { get => backingField.IsTransparent; set => backingField.IsTransparent = value; }
        public bool IsWalkable { get => !IsBlockingMove; set => IsBlockingMove = !value; }
        public Point Position { get => backingField.Position; set => backingField.Position = value; }

        public uint ID => backingField.ID;

        public IComponentCollection GoRogueComponents => backingField.GoRogueComponents;

        /// <summary>
        /// How much Move it costs to transverse this tile.\n
        /// 100 is the norm, it means it takes exactly 100 ticks to go to this tile.\n
        /// Note that the calculation on how many time it takes is usually MoveTime / Speed.
        /// </summary>
        public int MoveTimeCost { get; set; } = 100;
        public string Description { get; internal set; }

        #endregion backingField Data

        /// <summary>
        /// TileBase is an abstract base class
        /// representing the most basic form of of all Tiles used.
        /// Every TileBase has a Foreground Colour, Background Colour, and Glyph
        /// isBlockingMove and isBlockingSight are optional parameters, set to false by default
        /// </summary>
        /// <param name="foregroud"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="layer"></param>
        /// <param name="position"></param>
        /// <param name="idOfMaterial"></param>
        /// <param name="blocksMove"></param>
        /// <param name="isTransparent"></param>
        /// <param name="name"></param>
        protected TileBase(Color foregroud, Color background, int glyph, int layer,
            Point position, string idOfMaterial, bool blocksMove = true,
            bool isTransparent = true, string name = "ForgotToChangeName") :
            base(foregroud, background, glyph)
        {
            IsBlockingMove = blocksMove;
            Name = name;
            Layer = layer;
            backingField = new GameObject(position, layer, !blocksMove, isTransparent);
            MaterialOfTile = GameSys.Physics.PhysicsManager.SetMaterial(idOfMaterial);
            LastSeenAppereance = new ColoredGlyph(Foreground, Background, Glyph)
            {
                IsVisible = false
            };
            //Traits = new List<Trait>();
            if (MaterialOfTile is not null)
                CalculateTileHealth();
        }

        /// <summary>
        /// World tile const
        /// </summary>
        /// <param name="foregroud"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="layer"></param>
        /// <param name="position"></param>
        /// <param name="blocksMove"></param>
        /// <param name="isTransparent"></param>
        /// <param name="name"></param>
        protected TileBase(Color foregroud, Color background, int glyph, int layer,
            Point position, bool blocksMove = true,
            bool isTransparent = true, string name = "ForgotToChangeName")
            : base(foregroud, background, glyph)
        {
            IsBlockingMove = blocksMove;
            Name = name;
            Layer = layer;
            backingField = new GameObject(position, layer, !blocksMove, isTransparent);
            LastSeenAppereance = new ColoredGlyph(Foreground, Background, Glyph)
            {
                IsVisible = false
            };
        }

        protected void CalculateTileHealth() => _tileHealth = (int)MaterialOfTile.Density * MaterialOfTile.Hardness <= 0 ? 10 : _tileHealth = (int)MaterialOfTile.Density * MaterialOfTile.Hardness;

        public virtual void DestroyTile(TileBase changeTile, Entities.Item? itemDropped = null)
        {
            GameLoop.GetCurrentMap().SetTerrain(changeTile);
            LastSeenAppereance = changeTile;
            if (itemDropped is not null)
            {
                GameLoop.GetCurrentMap().AddMagiEntity(itemDropped);
            }
        }

        public abstract TileBase Copy();

        public List<Trait> GetMaterialTraits()
        {
            return MaterialOfTile.ConfersTraits;
        }

        public void AddVegetation(Plant plant, int index)
        {
            if (index < Vegetations.Length && HoldsVegetation)
            {
                Vegetations[index] = plant;
                CopyAppearanceFrom(plant.SadGlyph);
                LastSeenAppereance.CopyAppearanceFrom(plant.SadGlyph);
                IsDirty = true;
                plant.Position = Position;
            }
        }

        #region IGameObject Interface

        /// <summary>
        /// Fired when <see cref="P:GoRogue.GameFramework.IGameObject.IsTransparent" /> is changed.
        /// </summary>
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

        /// <summary>
        /// Fired when <see cref="P:GoRogue.GameFramework.IGameObject.IsWalkable" /> is changed.
        /// </summary>
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

        /// <summary>
        /// Event fired whenever this object's grid position is successfully changed.  Fired regardless of whether
        /// the object is part of a <see cref="GoRogue.GameFramework.Map" />.
        /// </summary>
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

        /// <summary>
        /// Fired when the object is added to a map.
        /// </summary>
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

        /// <summary>
        /// Fired when the object is removed from a map.
        /// </summary>
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

        /// <summary>
        /// Fired when <see cref="P:GoRogue.GameFramework.IGameObject.IsTransparent" /> is about to be changed.
        /// </summary>
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

        /// <summary>
        /// Fired when <see cref="P:GoRogue.GameFramework.IGameObject.IsWalkable" /> is about to changed.
        /// </summary>
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

        /// <summary>
        /// Internal use only, do not call manually!  Must, at minimum, call <see cref="M:GoRogue.GameFramework.GameObjectExtensions.SafelySetCurrentMap(GoRogue.GameFramework.IGameObject,GoRogue.GameFramework.Map@,GoRogue.GameFramework.Map,System.EventHandler{GoRogue.GameFramework.GameObjectCurrentMapChanged},System.EventHandler{GoRogue.GameFramework.GameObjectCurrentMapChanged})" />
        /// which will update the <see cref="P:GoRogue.GameFramework.IGameObject.CurrentMap" /> field of the IGameObject to reflect the change and fire map
        /// added/removed events as appropriate (or provide equivalent functionality).
        /// </summary>
        /// <param name="newMap">New map to which the IGameObject has been added.</param>
        public void OnMapChanged(GoRogue.GameFramework.Map newMap) => backingField.OnMapChanged(newMap);

        #endregion IGameObject Interface
    }
}