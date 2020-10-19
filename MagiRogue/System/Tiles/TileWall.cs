using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MagiRogue.System.Tiles
{
    // TileWall is based on TileBase
    public class TileWall : TileBase
    {
        // Default constructor
        // Walls are set to block movement and line of sight by default
        // and have a light gray foreground and a transparent background
        // represented by the # symbol
        public TileWall(bool blocksMovement = true, bool blocksSight = true) :
            base(Color.LightGray, Color.Transparent, '#', (int)MapLayer.TERRAIN, blocksMovement, blocksSight)
        {
            Name = "Wall";
        }
    }
}