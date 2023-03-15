using GoRogue.GameFramework;
using MagiRogue.GameSys.Tiles;
using SadRogue.Primitives;

namespace MagiRogue.Data
{
    public static class TileEncyclopedia
    {
        public static TileFloor GenericGrass(Point pos, Map? map = null)
        {
            var tile = DataManager.QueryTileInData<TileFloor>("t_soil", pos);

            for (int i = 0; i < tile.Vegetations.Length; i++)
            {
                tile.AddVegetation(DataManager.QueryPlantInData("grass"), i, (GameSys.Map?)map);
            }

            return tile;
        }

        public static TileFloor GenericDirtRoad(Point pos)
        {
            return DataManager.QueryTileInData<TileFloor>("dirt_road", pos);
        }

        internal static TileWall GenericTree()
        {
            return new TileWall(Palette.Wood, Color.Black, 'O', "Tree", Point.None, "wood");
        }

        internal static TileWall GenericTree(Point pos)
        {
            return new TileWall(Palette.Wood, Color.Black, 'O', "Tree", pos, "wood");
        }

        internal static TileWall GenericStoneWall()
        {
            return DataManager.QueryTileInData<TileWall>("stone_wall");
        }

        internal static TileWall GenericStoneWall(Point pos)
        {
            return DataManager.QueryTileInData<TileWall>("stone_wall", pos);
        }

        internal static TileFloor GenericStoneFloor()
        {
            return DataManager.QueryTileInData<TileFloor>("stone_floor");
        }

        internal static TileFloor GenericStoneFloor(Point pos)
        {
            return DataManager.QueryTileInData<TileFloor>("stone_floor", pos);
        }

        public static TileFloor GenericTreeTrunk(Point pos)
        {
            TileFloor tile = DataManager.QueryTileInData<TileFloor>("tree_trunk");
            tile.Position = pos;
            return tile;
        }
    }
}