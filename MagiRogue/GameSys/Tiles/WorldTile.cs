using MagusEngine.Core.Civ;
using MagusEngine.Core.WorldStuff;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MagiRogue.GameSys.Tiles
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed class WorldTile : TileBase
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

        public WorldTile Left { get; set; }
        public WorldTile Right { get; set; }
        public WorldTile Top { get; set; }
        public WorldTile TopRight { get; set; }
        public WorldTile TopLeft { get; set; }
        public WorldTile Bottom { get; set; }
        public WorldTile BottomRight { get; set; }
        public WorldTile BottomLeft { get; set; }
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

        public List<River> Rivers { get; set; } = new();

        public int RiverSize { get; set; }
        public int BiomeBitmask { get; set; }

        public Site? SiteInfluence { get; set; }
        public Road? Road { get; internal set; }
        public bool Visited { get; internal set; }

        /// <summary>
        /// The name of the 9x9 region of the local map Randomly generated
        /// </summary>
        public string RegionName { get; internal set; }

        public WorldTile(Color foregroud, Color background,
            int glyph, Point position,
            bool blocksMove = false, bool isTransparent = true,
            string name = "World Tile")
            : base(foregroud, background, glyph, (int)MapLayer.TERRAIN, position, blocksMove, isTransparent, name)
        {
            TileHealth = 999;
        }

        /// <summary>
        /// Empty constructor, for use to put new data inside
        /// </summary>
        public WorldTile() : base(Color.Black, Color.Black, '2', 0, Point.None, "null", false, true)
        {
        }


        public override TileBase Copy()
        {
            throw new NotImplementedException();
        }
    }
}