using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Tiles
{
    public enum HeightType
    {
        DeepWater = 1,
        ShallowWater = 2,
        Shore = 3,
        Sand = 4,
        Grass = 5,
        Forest = 6,
        Rock = 7,
        Snow = 8
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class WorldTile : TileBase
    {
        public float HeightValue { get; set; }

        public HeightType HeightType { get; set; }

        public WorldTile(Color foregroud, Color background,
            int glyph, Point position,
            bool blocksMove = false, bool isTransparent = true,
            string name = "World Tile")
            : base(foregroud, background, glyph, (int)MapLayer.TERRAIN, position, "null", blocksMove, isTransparent, name)
        {
        }

        /// <summary>
        /// Empty constructor, for use to put new data inside
        /// </summary>
        public WorldTile() : base(Color.Black, Color.Black, '2', 0, Point.None, "null")
        {
        }

        private string GetDebuggerDisplay()
        {
            return HeightType.ToString();
        }
    }
}