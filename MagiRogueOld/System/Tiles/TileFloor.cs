using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace MagiRogue.System.Tiles
{
    // TileFloor is based on TileBase
    // Floor tiles to be used in maps.
    public class TileFloor : TileBase
    {
        // Default constructor
        // Floors are set to allow movement and line of sight by default
        // and have a dark gray foreground and a transparent background
        // represented by the . symbol
        public TileFloor(bool blocksMove = false, bool blocksSight = false) : base(Color.DarkGray, Color.Transparent, '.', blocksMove, blocksSight)
        {
            Name = "Floor";
        }
    }
}