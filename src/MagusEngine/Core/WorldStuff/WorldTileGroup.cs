using Arquimedes.Enumerators;
using MagusEngine.Components.TilesComponents;
using System.Collections.Generic;

// Used in flood fill algorithm
namespace MagusEngine.Core.WorldStuff
{
    public sealed class WorldTileGroup
    {
        public TileGroupType Type { get; set; }
        public List<WorldTile> WorldTiles { get; set; }

        public WorldTileGroup()
        {
            WorldTiles = new();
        }
    }
}
