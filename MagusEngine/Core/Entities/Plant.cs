using Arquimedes.Enumerators;
using GoRogue.Components;
using GoRogue.Components.ParentAware;
using GoRogue.GameFramework;
using MagusEngine.Serialization;
using MagusEngine.Utils.Extensions;
using Newtonsoft.Json;
using SadConsole;
using SadRogue.Primitives;
using System;

namespace MagusEngine.Core.Entities
{
    public class Plant : IGameObject
    {
        private readonly GameObject backiendField = new GameObject((int)MapLayer.VEGETATION);
        private ColoredGlyph sadGlyph;
        private MagiColorSerialization fore;
        private MagiColorSerialization back;
        private MaterialTemplate material;

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

        public event EventHandler<ValueChangedEventArgs<bool>>? TransparencyChanging
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

        public event EventHandler<ValueChangedEventArgs<bool>>? TransparencyChanged
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

        public event EventHandler<ValueChangedEventArgs<bool>>? WalkabilityChanging
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

        public event EventHandler<ValueChangedEventArgs<bool>>? WalkabilityChanged
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

        /*public event EventHandler<SadRogue.Primitives.ValueChangedEventArgs<Point>>? Moved
        {
            add
            {
                ((IGameObject)backiendField).PositionChanged += value;
            }

            remove
            {
                ((IGameObject)backiendField).PositionChanged -= value;
            }
        }*/

        public event EventHandler<ValueChangedEventArgs<Point>>? PositionChanging
        {
            add
            {
                ((IPositionable)backiendField).PositionChanging += value;
            }

            remove
            {
                ((IPositionable)backiendField).PositionChanging -= value;
            }
        }

        public event EventHandler<ValueChangedEventArgs<Point>>? PositionChanged
        {
            add
            {
                ((IPositionable)backiendField).PositionChanged += value;
            }

            remove
            {
                ((IPositionable)backiendField).PositionChanged -= value;
            }
        }

        #endregion PropBackiendField

        #endregion Properties

        [JsonRequired]
        public string Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public string Fore
        {
            get
            {
                return fore.ColorName;
            }

            set
            {
                fore = new MagiColorSerialization(value);
            }
        }

        public string Back
        {
            get
            {
                return back.ColorName;
            }

            set
            {
                back = new MagiColorSerialization(value);
            }
        }

        [JsonRequired]
        public char[] Glyphs { get; set; }

        public ColoredGlyph SadGlyph => sadGlyph ??= new ColoredGlyph(Foreground, Background, Glyphs.GetRandomItemFromList());
        public Color Foreground => fore.Color;
        public Color Background => back.Color;

        public string MaterialId { get; set; }

        public MaterialTemplate Material => material;

        [JsonConstructor]
        public Plant()
        {
        }

        public Plant(ColoredGlyph glyph)
        {
            fore = new(glyph.Foreground);
            back = new(glyph.Background);
            Glyphs = new char[] { (char)glyph.Glyph };
        }

        public Plant(Color foreground, Color background, char[] glyphs)
        {
            fore = new(foreground);
            back = new(background);
            Glyphs = glyphs;
        }

        public void OnMapChanged(GoRogue.GameFramework.Map? newMap)
        {
            ((IGameObject)backiendField).OnMapChanged(newMap);
        }

        public Plant Clone()
        {
            var plant = new Plant(Foreground, Background, Glyphs)
            {
                Id = Id,
                Name = Name,
                Position = new Point(Position.X, Position.Y),
                IsWalkable = IsWalkable,
                IsTransparent = IsTransparent,
            };
            foreach (var item in GoRogueComponents)
            {
                plant.GoRogueComponents.Add(item.Component);
            }

            return plant;
        }
    }
}
