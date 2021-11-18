using MagiRogue.System.WorldGen;
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
        MagicLand = 9,
        River = 10
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

    public enum BiomeType
    {
        Desert,
        Savanna,
        TropicalRainforest,
        Grassland,
        Woodland,
        SeasonalForest,
        TemperateRainforest,
        BorealForest,
        Tundra,
        Ice
    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed class WorldTile : TileBase
    {
        public float HeightValue { get; set; }
        public float HeatValue { get; set; }
        public float MoistureValue { get; set; }
        public float MineralValue { get; set; }

        public HeightType HeightType { get; set; }
        public HeatType HeatType { get; set; }
        public MoistureType MoistureType { get; set; }
        public BiomeType BiomeType { get; set; }

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

        public List<River> Rivers { get; set; } = new();

        public int RiverSize { get; set; }
        public int BiomeBitmask { get; set; }

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

        public void UpdateBiomeBitmask()
        {
            int count = 0;

            if (Collidable && Top != null && Top.BiomeType == BiomeType)
                count += 1;
            if (Collidable && Bottom != null && Bottom.BiomeType == BiomeType)
                count += 4;
            if (Collidable && Left != null && Left.BiomeType == BiomeType)
                count += 8;
            if (Collidable && Right != null && Right.BiomeType == BiomeType)
                count += 2;

            BiomeBitmask = count;
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

        public int GetRiverNeighborCount(River river)
        {
            int count = 0;
            if (Left.Rivers.Count > 0 && Left.Rivers.Contains(river))
                count++;
            if (Right.Rivers.Count > 0 && Right.Rivers.Contains(river))
                count++;
            if (Top.Rivers.Count > 0 && Top.Rivers.Contains(river))
                count++;
            if (Bottom.Rivers.Count > 0 && Bottom.Rivers.Contains(river))
                count++;
            return count;
        }

        public RiverDirection GetLowestNeighbor()
        {
            if (Left.HeightValue < Right.HeightValue
                && Left.HeightValue < Top.HeightValue
                && Left.HeightValue < Bottom.HeightValue)
                return RiverDirection.Left;
            else if (Right.HeightValue < Left.HeightValue
                && Right.HeightValue < Top.HeightValue
                && Right.HeightValue < Bottom.HeightValue)
                return RiverDirection.Right;
            else if (Top.HeightValue < Left.HeightValue
                && Top.HeightValue < Right.HeightValue
                && Top.HeightValue < Bottom.HeightValue)
                return RiverDirection.Right;
            else if (Bottom.HeightValue < Left.HeightValue
                && Bottom.HeightValue < Top.HeightValue
                && Bottom.HeightValue < Right.HeightValue)
                return RiverDirection.Right;
            else
                return RiverDirection.Bottom;
        }

        public void SetRiverPath(River river)
        {
            if (!Collidable)
                return;

            if (!Rivers.Contains(river))
            {
                Rivers.Add(river);
            }
        }

        private void SetRiverTile(River river)
        {
            SetRiverPath(river);
            HeightType = HeightType.River;
            HeightValue = 0;
            Collidable = false;
        }

        public void DigRiver(River river, int size)
        {
            SetRiverTile(river);
            RiverSize = size;

            if (size == 1)
            {
                Bottom.SetRiverTile(river);
                Right.SetRiverTile(river);
                Bottom.Right.SetRiverTile(river);
            }

            if (size == 2)
            {
                Bottom.SetRiverTile(river);
                Right.SetRiverTile(river);
                Bottom.Right.SetRiverTile(river);
                Top.SetRiverTile(river);
                Top.Left.SetRiverTile(river);
                Top.Right.SetRiverTile(river);
                Left.SetRiverTile(river);
                Left.Bottom.SetRiverTile(river);
            }

            if (size == 3)
            {
                Bottom.SetRiverTile(river);
                Right.SetRiverTile(river);
                Bottom.Right.SetRiverTile(river);
                Top.SetRiverTile(river);
                Top.Left.SetRiverTile(river);
                Top.Right.SetRiverTile(river);
                Left.SetRiverTile(river);
                Left.Bottom.SetRiverTile(river);
                Right.Right.SetRiverTile(river);
                Right.Right.Bottom.SetRiverTile(river);
                Bottom.Bottom.SetRiverTile(river);
                Bottom.Bottom.Right.SetRiverTile(river);
            }

            if (size == 4)
            {
                Bottom.SetRiverTile(river);
                Right.SetRiverTile(river);
                Bottom.Right.SetRiverTile(river);
                Top.SetRiverTile(river);
                Top.Right.SetRiverTile(river);
                Left.SetRiverTile(river);
                Left.Bottom.SetRiverTile(river);
                Right.Right.SetRiverTile(river);
                Right.Right.Bottom.SetRiverTile(river);
                Bottom.Bottom.SetRiverTile(river);
                Bottom.Bottom.Right.SetRiverTile(river);
                Left.Bottom.Bottom.SetRiverTile(river);
                Left.Left.Bottom.SetRiverTile(river);
                Left.Left.SetRiverTile(river);
                Left.Left.Top.SetRiverTile(river);
                Left.Top.SetRiverTile(river);
                Left.Top.Top.SetRiverTile(river);
                Top.Top.SetRiverTile(river);
                Top.Top.Right.SetRiverTile(river);
                Top.Right.Right.SetRiverTile(river);
            }
        }
    }
}