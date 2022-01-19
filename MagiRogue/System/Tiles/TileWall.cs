using SadRogue.Primitives;

namespace MagiRogue.System.Tiles
{
    // TileWall is based on TileBase
    public sealed class TileWall : TileBase
    {
        // Default constructor
        // Walls are set to block movement and line of sight by default
        // and have a light gray foreground and a transparent background
        // represented by the # symbol
        /// <summary>
        /// Default stone wall constructor.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="idMaterial"></param>
        /// <param name="blocksMovement"></param>
        /// <param name="tileIsTransparent"></param>
        public TileWall(Point position, string idMaterial = "stone", bool blocksMovement = true, bool tileIsTransparent = false) :
            base(Color.LightGray, Color.Transparent, '#', (int)MapLayer.TERRAIN, position, idMaterial, blocksMovement, tileIsTransparent)
        {
            Name = "Stone Wall";
        }

        /// <summary>
        /// This constructor makes it possible to create any kind of wall.
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="glyph"></param>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="idMaterial"></param>
        /// <param name="blocksMove"></param>
        /// <param name="tileIsTransparent"></param>
        public TileWall(Color foreground, Color background, int glyph, string name, Point position, string idMaterial,
            bool blocksMove = true, bool tileIsTransparent = false)
            : base(foreground, background, glyph, (int)MapLayer.TERRAIN, position, idMaterial, blocksMove,
                  tileIsTransparent, name)
        {
        }
    }
}