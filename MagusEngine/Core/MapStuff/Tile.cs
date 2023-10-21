using Arquimedes.Enumerators;
using GoRogue.Components;
using MagusEngine.Bus.ComponentBus;
using MagusEngine.ECS.Components;
using MagusEngine.Serialization;
using MagusEngine.Services;
using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagusEngine.Core.MapStuff
{
    public class Tile : MagiGameObject
    {
        public ColoredGlyph Appearence { get; set; }
        public ColoredGlyph? LastSeenAppereance { get; set; }
        public int MoveTimeCost { get; set; } = 100;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Trait> Traits { get; set; } = new();

        public Tile() : this(Color.BlueViolet, Color.Wheat, '@', true, true, Point.None)
        {
        }

        public Tile(Color foreground,
            Color background,
            char glyph,
            bool isWalkable,
            bool isTransparent,
            Point pos,
            IComponentCollection collection = null) : base(pos,
                (int)MapLayer.TERRAIN,
                isWalkable, isTransparent,
                Locator.GetService<IDGenerator>() is not null ? Locator.GetService<IDGenerator>().UseID : null,
                collection)
        {
            Appearence = new ColoredGlyph(foreground, background, glyph);
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

        private Tile(Tile tile)
            : this(tile.Appearence.Foreground,
                tile.Appearence.Background,
                tile.Appearence.GlyphCharacter,
                tile.IsWalkable,
                tile.IsTransparent,
                tile.Position,
                tile.GoRogueComponents)
        {
            Traits = tile.Traits;
            Name = tile.Name;
            MoveTimeCost = tile.MoveTimeCost;
        }

        public T? GetComponent<T>(string? tag = null) where T : class
            => GoRogueComponents.GetFirst<T>(tag);

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
            return new Tile(this);
        }

        public Tile Copy(Point pos)
        {
            var tile = Copy();
            tile.Position = pos;

            return tile;
        }

        public void AddComponent<T>(T value, string? tag = null) where T : class
        {
            GoRogueComponents.Add(value, tag);
            Locator.GetService<MessageBusService>()
                ?.SendMessage<ComponentAddedCommand<T>>(new(ID, value));
        }

        public MaterialTemplate? GetMaterial()
        {
            return GetComponent<MaterialComponent>()?.Material;
        }
    }
}
