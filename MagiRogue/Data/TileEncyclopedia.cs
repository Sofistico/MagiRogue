using MagiRogue.GameSys.Tiles;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.Data
{
    public static class TileEncyclopedia
    {
        public static TileFloor GenericGrass(Point pos)
        {
            int rng = GoRogue.Random.GlobalRandom.DefaultRNG.NextInt(1, 3 + 1);
            return rng switch
            {
                1 => DataManager.QueryTileInData<TileFloor>("t_grass"),
                2 => DataManager.QueryTileInData<TileFloor>("t_grass_long"),
                3 => DataManager.QueryTileInData<TileFloor>("t_grass_short"),
                _ => null,
            };
        }

        public static TileFloor GenericDirtRoad(Point pos)
        {
            return DataManager.QueryTileInData<TileFloor>("dirt_road", pos);
        }

        internal static TileWall GenericTree()
        {
            return new TileWall(Palette.Wood, Color.Black, 'O', "Tree", Point.None, "wood");
        }

        internal static TileWall GenericStoneWall()
        {
            return DataManager.QueryTileInData<TileWall>("stone_wall");
        }

        internal static TileFloor GenericStoneFloor()
        {
            return DataManager.QueryTileInData<TileFloor>("stone_floor");
        }

        public static TileFloor GenericTreeTrunk(Point pos)
        {
            TileFloor tile = DataManager.QueryTileInData<TileFloor>("tree_trunk");
            tile.Position = pos;
            return tile;
        }
    }
}