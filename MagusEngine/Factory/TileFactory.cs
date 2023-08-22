using Arquimedes;
using GoRogue.GameFramework;
using MagusEngine.Core;
using MagusEngine.ECS.Components;
using MagusEngine.Systems;
using SadRogue.Primitives;

namespace MagusEngine.Factory
{
    public static class TileFactory
    {
        public static Tile GenericGrass(Point pos, Map? map = null)
        {
            var tile = DataManager.QueryTileInData("t_soil", pos);
            // TODO: REDO
            //for (int i = 0; i < tile.Vegetations.Length; i++)
            //{
            //    tile.AddVegetation(DataManager.QueryPlantInData("grass"), i, (GameSys.Map?)map);
            //}

            return tile;
        }

        public static Tile GenericDirtRoad(Point pos)
        {
            return DataManager.QueryTileInData("dirt_road", pos);
        }

        public static Tile GenericTree()
        {
            var tile = new Tile(MagiPalette.Wood, Color.Black, 'O', false, false, Point.None)
            {
                Name = "Tree",
            };
            tile.AddComponent<MaterialComponent>(new("wood"));
            return tile;
        }

        internal static Tile GenericTree(Point pos)
        {
            var tile = new Tile(MagiPalette.Wood, Color.Black, 'O', false, false, pos)
            {
                Name = "Tree",
            };
            tile.AddComponent<MaterialComponent>(new("wood"));
            return tile;
        }

        internal static Tile GenericStoneWall()
        {
            return DataManager.QueryTileInData("stone_wall");
        }

        internal static Tile GenericStoneWall(Point pos)
        {
            return DataManager.QueryTileInData("stone_wall", pos);
        }

        internal static Tile GenericStoneFloor()
        {
            return DataManager.QueryTileInData("t_stone");
        }

        internal static Tile GenericStoneFloor(Point pos)
        {
            return DataManager.QueryTileInData("t_stone", pos);
        }

        public static Tile GenericTreeTrunk(Point pos)
        {
            return DataManager.QueryTileInData("tree_trunk", pos);
        }
    }
}