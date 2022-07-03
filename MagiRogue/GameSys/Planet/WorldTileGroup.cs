using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Used in flood fill algorithm
namespace MagiRogue.GameSys.Planet
{
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