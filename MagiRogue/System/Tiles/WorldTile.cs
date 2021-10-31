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
        Snow = 8,
        MagicLand = 9
    }

    public enum HeatType
    {
        Coldest,
        Colder,
        Cold,
        Warm,
        Warmer,
        Warmest
    }

    public enum MoistureType
    {
        Wettest,
        Wetter,
        Wet,
        Dry,
        Dryer,
        Dryest
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class WorldTile : TileBase
    {
        public float HeightValue { get; set; }
        public float HeatValue { get; set; }
        public float MoistureValue { get; set; }

        public HeightType HeightType { get; set; }
        public HeatType HeatType { get; set; }
        public MoistureType MoistureType { get; set; }

        public WorldTile Left { get; set; }
        public WorldTile Right { get; set; }
        public WorldTile Top { get; set; }
        public WorldTile Bottom { get; set; }
        public int Bitmask { get; private set; }

        /// <summary>
        /// The FloodFilled variable will be used to keep
        /// track of which tiles have already been processed by the flood filling algorithm
        /// </summary>
        public bool FloodFilled { get; set; }

        /// <summary>
        /// Anything that isn't water will be collidable
        /// </summary>
        public bool Collidable { get; set; }

        public WorldTile(Color foregroud, Color background,
            int glyph, Point position,
            bool blocksMove = false, bool isTransparent = true,
            string name = "World Tile")
            : base(foregroud, background, glyph, (int)MapLayer.TERRAIN, position, blocksMove, isTransparent, name)
        {
        }

        /// <summary>
        /// Empty constructor, for use to put new data inside
        /// </summary>
        public WorldTile() : base(Color.Black, Color.Black, '2', 0, Point.None, "null")
        {
        }

        public void UpdateBitmask()
        {
            int count = 0;

            if (Top.HeightType == HeightType)
                count += 1;
            if (Right.HeightType == HeightType)
                count += 2;
            if (Left.HeightType == HeightType)
                count += 4;
            if (Bottom.HeightType == HeightType)
                count += 8;

            Bitmask = count;
        }

        private string GetDebuggerDisplay()
        {
            return HeightType.ToString();
        }
    }
}