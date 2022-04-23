using MagiRogue.Data.Serialization;
using MagiRogue.GameSys.Tiles;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MagiRogue.GameSys.Planet
{
    [JsonConverter(typeof(RiverJsonConverter))]
    public class River
    {
        public int RiverCount { get; }
        public int Length { get; set; }
        public int Id { get; }
        public List<WorldTile> Tiles { get; set; }
        public int Intersections { get; set; }
        public float TurnCount { get; set; }
        public WorldDirection CurrentDirection { get; set; }

        public River(int id)
        {
            Id = id;
            Tiles = new();
        }

        public void AddTile(WorldTile tile)
        {
            tile.SetRiverPath(this);
            Tiles.Add(tile);
        }
    }

    public class RiverGroup
    {
        public List<River> Rivers = new();
    }
}