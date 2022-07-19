using MagiRogue.Data.Enumerators;
using MagiRogue.Data.Serialization.MapSerialization;
using MagiRogue.GameSys.Tiles;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Civ
{
    [JsonConverter(typeof(RoadJsonConverter))]
    public class Road
    {
        public Dictionary<Point, WorldDirection> RoadDirectionInPos { get; set; }

        public List<WorldTile> RoadTiles { get; set; }
        public RoadStatus Status { get; set; }

        public Road()
        {
            RoadTiles = new List<WorldTile>();
            RoadDirectionInPos = new();
        }

        public void AddTileToList(WorldTile tile) => RoadTiles.Add(tile);
    }
}