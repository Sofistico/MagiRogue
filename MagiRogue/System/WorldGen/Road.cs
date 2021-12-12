using MagiRogue.System.Tiles;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.WorldGen
{
    public enum RoadStatus
    {
        Normal,
        Abandoned
    }
    public class Road
    {
        public WorldDirection RoadDirection { get; set; }
        public List<WorldTile> RoadTiles { get; set; }
        public RoadStatus Status { get; set; }

        public Road()
        {
            RoadTiles = new List<WorldTile>();
        }

        public void AddTileToList(WorldTile tile) => RoadTiles.Add(tile);
    }
}