using Arquimedes.Enumerators;
using GoRogue.Components.ParentAware;
using MagusEngine.Core.MapStuff;
using Newtonsoft.Json;
using SadRogue.Primitives;
using System.Collections.Generic;

namespace MagusEngine.Components.TilesComponents
{
    public class WorldTile : ParentAwareComponentBase<Tile>
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

        [JsonIgnore]
        public Dictionary<Direction, WorldTile> Directions { get; set; } = new();

        [JsonIgnore]
        public WorldTile Top { get => Directions[Direction.Up]; set => Directions[Direction.Up] = value; }

        [JsonIgnore]
        public WorldTile Bottom { get => Directions[Direction.Down]; set => Directions[Direction.Down] = value; }

        [JsonIgnore]
        public WorldTile Left { get => Directions[Direction.Left]; set => Directions[Direction.Left] = value; }

        [JsonIgnore]
        public WorldTile Right { get => Directions[Direction.Right]; set => Directions[Direction.Right] = value; }

        [JsonIgnore]
        public WorldTile TopRight { get => Directions[Direction.UpRight]; set => Directions[Direction.UpRight] = value; }

        [JsonIgnore]
        public WorldTile TopLeft { get => Directions[Direction.UpLeft]; set => Directions[Direction.UpLeft] = value; }

        [JsonIgnore]
        public WorldTile BottomRight { get => Directions[Direction.DownRight]; set => Directions[Direction.DownRight] = value; }

        [JsonIgnore]
        public WorldTile BottomLeft { get => Directions[Direction.DownLeft]; set => Directions[Direction.DownLeft] = value; }

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

        public bool Visited { get; internal set; }

        /// <summary>
        /// The name of the 9x9 region of the local map Randomly generated
        /// </summary>
        public string? RegionName { get; internal set; }

        public Point Position
        {
            get => Parent!.Position;
            set => Parent!.Position = value;
        }

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

        public Direction GetLowestNeighbor()
        {
            Direction lowerDirection = default;
            float lowerValue = float.MaxValue;
            foreach (var (dir, neighboor) in Directions)
            {
                if (!dir.IsCardinal())
                    continue;
                if (neighboor.HeightValue < lowerValue)
                {
                    lowerDirection = dir;
                    lowerValue = neighboor.HeightValue;
                }
                if (neighboor.HeightType is HeightType.ShallowWater or HeightType.DeepWater)
                {
                    lowerDirection = Direction.None;
                    break;
                }
            }

            return lowerDirection;
        }

        public float GetResources()
        {
            // i dunno!
            return MineralValue;
        }
    }
}
