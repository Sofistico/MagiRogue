﻿using MagiRogue.System.Tiles;
using MagiRogue.System.Planet;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Civ
{
    public enum RoadStatus
    {
        Normal,
        Abandoned
    }

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