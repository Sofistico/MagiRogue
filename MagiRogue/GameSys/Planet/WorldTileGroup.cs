﻿using MagiRogue.Data.Enumerators;
using MagiRogue.GameSys.Tiles;
using System.Collections.Generic;

// Used in flood fill algorithm
namespace MagiRogue.GameSys.Planet
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