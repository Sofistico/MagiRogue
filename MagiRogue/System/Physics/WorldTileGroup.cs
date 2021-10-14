using MagiRogue.System.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Used in flood fill algorithm
namespace MagiRogue.System.Physics
{
    public enum TileGroupType
    {
        Water,
        Land
    }

    public class WorldTileGroup
    {
        public TileGroupType Type { get; set; }
        public List<WorldTile> WorldTiles { get; set; }

        public WorldTileGroup()
        {
            WorldTiles = new();
        }
    }
}