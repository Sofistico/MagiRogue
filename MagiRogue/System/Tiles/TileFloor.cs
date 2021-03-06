﻿using SadRogue.Primitives;

namespace MagiRogue.System.Tiles
{
    /// <summary>
    /// TileFloor is based on TileBase
    /// Floor tiles to be used in maps.
    /// </summary>
    public class TileFloor : TileBase
    {
        /// <summary>
        /// Default constructor
        /// \nFloors are set to allow movement and line of sight by default
        /// and have a dark gray foreground and a transparent background
        /// represented by the . symbol
        /// </summary>
        /// <param name="position"></param>
        /// <param name="blocksMove"></param>
        /// <param name="tileIsTransparent"></param>
        public TileFloor(Point position, string idMaterial, bool blocksMove = false, bool tileIsTransparent = true) : base(Color.DarkGray, Color.Transparent, '.', (int)MapLayer.TERRAIN, position, idMaterial, blocksMove, tileIsTransparent)
        {
            Name = "Floor";
        }
    }
}