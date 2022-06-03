﻿using MagiRogue.GameSys.Tiles;
using MagiRogue.GameSys.Planet;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MagiRogue.Data.Serialization;
using MagiRogue.Data.Serialization.MapSerialization;

namespace MagiRogue.GameSys.Civ
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RoadStatus
    {
        Normal,
        Abandoned
    }

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