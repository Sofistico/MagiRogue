using Arquimedes.Enumerators;
using MagusEngine.Core.Civ;
using SadRogue.Primitives;

namespace MagusEngine.ECS.Components.TilesComponents
{
    public class WorldTile
    {
        public float HeightValue { get; set; }
        public float HeatValue { get; set; }
        public float MoistureValue { get; set; }
        public float MineralValue { get; set; }
        public int MagicalAuraStrength { get; set; }
        public HeightType HeightType { get; set; }
        public HeatType HeatType { get; set; }
        public MoistureType MoistureType { get; set; }
        public BiomeType BiomeType { get; set; }
        public SpecialLandType SpecialLandType { get; set; }

        public Dictionary<Direction, WorldTile> Directions { get; set; } = new();
        public int Bitmask { get; private set; }

        /// <summary>
        /// The FloodFilled variable will be used to keep track of which tiles have already been
        /// processed by the flood filling algorithm
        /// </summary>
        public bool FloodFilled { get; set; }

        /// <summary>
        /// Anything that isn't water will be collidable, so false means water
        /// </summary>
        public bool Collidable { get; set; }

        public int BiomeBitmask { get; set; }

        //public Road? Road { get; internal set; }

        public bool Visited { get; internal set; }

        /// <summary>
        /// The name of the 9x9 region of the local map Randomly generated
        /// </summary>
        public string? RegionName { get; internal set; }

        public void UpdateBiomeBitmask()
        {
            int count = 0;

            if (Collidable && Top != null && Top.BiomeType == BiomeType)
                count++;
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
                count++;
            if (Right.HeightType == HeightType)
                count += 2;
            if (Left.HeightType == HeightType)
                count += 4;
            if (Bottom.HeightType == HeightType)
                count += 8;

            Bitmask = count;
        }



        public WorldDirection GetLowestNeighbor()
        {
            if (Left.HeightValue < Right.HeightValue
                && Left.HeightValue < Top.HeightValue
                && Left.HeightValue < Bottom.HeightValue)
            {
                return WorldDirection.Left;
            }
            else if (Right.HeightValue < Left.HeightValue
                && Right.HeightValue < Top.HeightValue
                && Right.HeightValue < Bottom.HeightValue)
            {
                return WorldDirection.Right;
            }
            else if (Top.HeightValue < Left.HeightValue
                && Top.HeightValue < Right.HeightValue
                && Top.HeightValue < Bottom.HeightValue)
            {
                return WorldDirection.Right;
            }
            else if (Bottom.HeightValue < Left.HeightValue
                && Bottom.HeightValue < Top.HeightValue
                && Bottom.HeightValue < Right.HeightValue)
            {
                return WorldDirection.Right;
            }
            else
            {
                return WorldDirection.Bottom;
            }
        }

    }
}
