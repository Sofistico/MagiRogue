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
        public TileWall(Point position, bool blocksMovement = true, bool tileIsTransparent = false) :
            base(Color.LightGray, Color.Transparent, '#', (int)MapLayer.TERRAIN, position, blocksMovement, tileIsTransparent)
        {
            Name = "Wall";
            SetMaterial("stone");
        }
    }
}