using Arquimedes.Enumerators;
using MagusEngine.ECS.Components.TilesComponents;
using MagusEngine.Serialization.MapConverter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagusEngine.Core.Civ
{
    [JsonConverter(typeof(RoadJsonConverter))]
    public class Road
    {
        public Dictionary<Point, WorldDirection> RoadDirectionInPos { get; set; }
        public List<WorldTile> RoadTiles { get; set; }
        public RoadStatus Status { get; set; }
        public int RoadId { get; set; }

        public Road()
        {
            RoadTiles = new List<WorldTile>();
            RoadDirectionInPos = new();
        }

        public void AddTileToList(WorldTile tile) => RoadTiles.Add(tile);
    }
}