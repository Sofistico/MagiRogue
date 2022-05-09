using SadRogue.Primitives;

namespace MagiRogue.GameSys.Tiles
{
    /// <summary>
    /// TileFloor is based on TileBase
    /// Floor tiles to be used in maps.
    /// </summary>
    public sealed class TileFloor : TileBase
    {
        /// <summary>
        /// Constructor for any kind of floor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="position"></param>
        /// <param name="idMaterial"></param>
        /// <param name="glyph"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <param name="blocksMove"></param>
        /// <param name="tileIsTransparent"></param>
        public TileFloor(string name, Point position, string idMaterial, int glyph, 
            Color foreground, Color background,
            bool blocksMove = false, bool tileIsTransparent = true)
            : base(foreground, background, glyph, (int)MapLayer.TERRAIN, position, idMaterial, blocksMove,
                  tileIsTransparent, name)
        {
        }

        /// <summary>
        /// Default constructor or a stone tile.
        /// \nFloors are set to allow movement and line of sight by default
        /// and have a dark gray foreground and a transparent background
        /// represented by the . symbol
        /// </summary>
        /// <param name="position"></param>
        /// <param name="blocksMove"></param>
        /// <param name="tileIsTransparent"></param>
        public TileFloor(Point position, string idMaterial = "stone", bool blocksMove = false, bool tileIsTransparent = true)
            : base(Color.DarkGray, Color.Transparent, '.', (int)MapLayer.TERRAIN, position, idMaterial, blocksMove, tileIsTransparent)
        {
            Name = "Stone Floor";
        }

        public override TileFloor Copy()
        {
            TileFloor copy = new TileFloor(Name, Position, MaterialOfTile.Id, Glyph, Foreground,
                Background, IsBlockingMove, IsTransparent)
            {
                MoveTimeCost = MoveTimeCost,
                LastSeenAppereance = LastSeenAppereance,
                BitMask = BitMask,
                Description = Description,
                Decorators = Decorators,
                IsDirty = IsDirty,
                InfusedMp = InfusedMp,
                IsVisible = IsVisible,
                TileHealth = TileHealth
            };
            return copy;
        }

    }
}