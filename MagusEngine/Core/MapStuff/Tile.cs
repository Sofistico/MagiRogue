using Arquimedes.Enumerators;
using GoRogue.GameFramework;
using SadConsole;
using SadRogue.Primitives;

namespace MagusEngine.Core
{
    public partial class Tile : IGameObject
    {
        public ColoredGlyph Appearence { get; set; }
        public ColoredGlyph? LastSeenAppereance { get; internal set; }
        public int MoveTimeCost { get; set; } = 100;
        public string? Name { get; set; }
        public string? Description { get; set; }

        public Tile(Color foreground,
            Color background,
            char glyph,
            bool isWalkable,
            bool isTransparent)
        {
            Appearence = new(foreground, background, glyph);
            _gameObject = new((int)MapLayer.TERRAIN,
                isWalkable,
                isTransparent,
                Locator.GetService<IDGenerator>().UseID);
        }

        public T? GetComponent<T>() where T : class => GoRogueComponents.GetFirst<T>();

        public bool HasComponent<TFind>(string? tag = null) where TFind : class
        {
            return GoRogueComponents.Contains<TFind>(tag);
        }
    }
}