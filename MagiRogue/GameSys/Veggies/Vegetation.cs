using GoRogue;
using GoRogue.Components;
using GoRogue.Components.ParentAware;
using GoRogue.GameFramework;
using MagiRogue.Data.Enumerators;
using MagiRogue.Utils.Extensions;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Veggies
{
    public class Vegetation : IGameObject
    {
        private readonly GameObject backiendField = new GameObject((int)MapLayer.VEGETATION);
        private ColoredGlyph sadGlyph;

        #region Properties

        #region PropBackiendField

        public GoRogue.GameFramework.Map? CurrentMap => ((IGameObject)backiendField).CurrentMap;

        public bool IsTransparent { get => ((IGameObject)backiendField).IsTransparent; set => ((IGameObject)backiendField).IsTransparent = value; }
        public bool IsWalkable { get => ((IGameObject)backiendField).IsWalkable; set => ((IGameObject)backiendField).IsWalkable = value; }
        public Point Position { get => ((IGameObject)backiendField).Position; set => ((IGameObject)backiendField).Position = value; }

        public uint ID => ((IHasID)backiendField).ID;

        public int Layer => ((IHasLayer)backiendField).Layer;

        public IComponentCollection GoRogueComponents => ((IObjectWithComponents)backiendField).GoRogueComponents;

        public event EventHandler<GameObjectCurrentMapChanged>? AddedToMap
        {
            add
            {
                ((IGameObject)backiendField).AddedToMap += value;
            }

            remove
            {
                ((IGameObject)backiendField).AddedToMap -= value;
            }
        }

        public event EventHandler<GameObjectCurrentMapChanged>? RemovedFromMap
        {
            add
            {
                ((IGameObject)backiendField).RemovedFromMap += value;
            }

            remove
            {
                ((IGameObject)backiendField).RemovedFromMap -= value;
            }
        }

        public event EventHandler<GameObjectPropertyChanged<bool>>? TransparencyChanging
        {
            add
            {
                ((IGameObject)backiendField).TransparencyChanging += value;
            }

            remove
            {
                ((IGameObject)backiendField).TransparencyChanging -= value;
            }
        }

        public event EventHandler<GameObjectPropertyChanged<bool>>? TransparencyChanged
        {
            add
            {
                ((IGameObject)backiendField).TransparencyChanged += value;
            }

            remove
            {
                ((IGameObject)backiendField).TransparencyChanged -= value;
            }
        }

        public event EventHandler<GameObjectPropertyChanged<bool>>? WalkabilityChanging
        {
            add
            {
                ((IGameObject)backiendField).WalkabilityChanging += value;
            }

            remove
            {
                ((IGameObject)backiendField).WalkabilityChanging -= value;
            }
        }

        public event EventHandler<GameObjectPropertyChanged<bool>>? WalkabilityChanged
        {
            add
            {
                ((IGameObject)backiendField).WalkabilityChanged += value;
            }

            remove
            {
                ((IGameObject)backiendField).WalkabilityChanged -= value;
            }
        }

        public event EventHandler<GameObjectPropertyChanged<Point>>? Moved
        {
            add
            {
                ((IGameObject)backiendField).Moved += value;
            }

            remove
            {
                ((IGameObject)backiendField).Moved -= value;
            }
        }

        #endregion PropBackiendField

        #endregion Properties

        [JsonRequired]
        public string Id { get; set; }

        [JsonRequired]
        public Color Foreground { get; set; }

        [JsonRequired]
        public Color Background { get; set; }

        [JsonRequired]
        public int[] Glyphs { get; set; }

        public ColoredGlyph SadGlyph => sadGlyph ??= new ColoredGlyph(Foreground, Background, Glyphs.GetRandomItemFromList());

        [JsonConstructor]
        public Vegetation()
        {
        }

        public Vegetation(ColoredGlyph glyph)
        {
            Foreground = glyph.Foreground;
            Background = glyph.Background;
            Glyphs = new int[] { glyph.Glyph };
        }

        public Vegetation(Color foreground, Color background, int[] glyphs)
        {
            Foreground = foreground;
            Background = background;
            Glyphs = glyphs;
        }

        public void OnMapChanged(GoRogue.GameFramework.Map? newMap)
        {
            ((IGameObject)backiendField).OnMapChanged(newMap);
        }
    }
}
