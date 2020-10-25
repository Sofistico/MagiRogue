using Microsoft.Xna.Framework;

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
        public TileFloor(bool blocksMove = false, bool blocksSight = false) : base(Color.DarkGray, Color.Transparent, '.', (int)MapLayer.TERRAIN, blocksMove, blocksSight)
        {
            Name = "Floor";
            SetMaterial("stone");
        }
    }
}