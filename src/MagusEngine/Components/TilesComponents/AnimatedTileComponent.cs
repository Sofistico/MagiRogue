using GoRogue.Components.ParentAware;
using MagusEngine.Core.MapStuff;

namespace MagusEngine.Components.TilesComponents
{
    public class AnimatedTileComponent : ParentAwareComponentBase<Tile>
    {
        public int Moves { get; set; }
        public char[] Glyphs { get; set; }
        public string Name { get; set; }

        public AnimatedTileComponent(int moves, char[] glyphs, string name)
        {
            Moves = moves;
            Glyphs = glyphs;
            Name = name;
        }
    }
}
