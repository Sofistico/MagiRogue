using Arquimedes.Enumerators;
using GoRogue.GameFramework;
using MagusEngine.Bus.ComponentBus;
using MagusEngine.ECS.Components;
using MagusEngine.Serialization;
using MagusEngine.Services;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

namespace MagusEngine.Core.MapStuff
{
    public partial class Tile : IGameObject
    {
        public ColoredGlyph Appearence { get; set; }
        public ColoredGlyph? LastSeenAppereance { get; set; }
        public int MoveTimeCost { get; set; } = 100;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Trait> Traits { get; set; } = new();

        public Tile() : this(Color.BlueViolet, Color.Wheat, '@', true, true, Point.None)
        {
            LastSeenAppereance = (ColoredGlyph)Appearence.Clone();
        }

        public Tile(Color foreground,
            Color background,
            char glyph,
            bool isWalkable,
            bool isTransparent,
            Point pos)
        {
            Appearence = new ColoredGlyph(foreground, background, glyph);
            _gameObject = new((int)MapLayer.TERRAIN,
                isWalkable,
                isTransparent,
                Locator.GetService<IDGenerator>().UseID);
            Position = pos;
            LastSeenAppereance = (ColoredGlyph)Appearence.Clone();
        }

        public Tile(Color foreground,
            Color background,
            char glyph,
            bool isWalkable,
            bool isTransparent,
            Point pos,
            string? name,
            string idMaterial,
            int moveTimeCost = 100) : this(foreground, background, glyph, isWalkable, isTransparent, pos)
        {
            MoveTimeCost = moveTimeCost;
            Name = name;
            AddComponent<MaterialComponent>(new(idMaterial));
        }

        public T? GetComponent<T>(string? tag = null) where T : class => GoRogueComponents.GetFirst<T>(tag);

        public bool GetComponent<T>(out T? comp, string? tag = null) where T : class
        {
            comp = GoRogueComponents.GetFirstOrDefault<T>(tag)!;
            return comp != null;
        }

        public bool HasComponent<TFind>(string? tag = null) where TFind : class
        {
            return GoRogueComponents.Contains<TFind>(tag);
        }

        public void RemoveComponent(object comp)
        {
            GoRogueComponents.Remove(comp);
        }

        public void RemoveComponent(string tag)
        {
            GoRogueComponents.Remove(tag);
        }

        public Tile Copy()
        {
            throw new NotImplementedException();
        }

        public Tile Copy(Point pos)
        {
            throw new NotImplementedException();
        }

        public void AddComponent<T>(T value, string? tag = null) where T : class
        {
            GoRogueComponents.Add(value, tag);
            Locator.GetService<MessageBusService>()
                .SendMessage<ComponentAddedCommand<T>>(new(ID, value));
        }

        public MaterialTemplate? GetMaterial()
        {
            return GetComponent<MaterialComponent>()?.Material;
        }
    }
}
