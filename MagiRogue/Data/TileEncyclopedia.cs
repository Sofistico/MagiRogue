using MagiRogue.System.Tiles;
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

#pragma warning disable CS8603 // Possível retorno de referência nula.
            return rng switch
            {
                1 => new TileFloor("Long grass", pos, "grass", '\'', Palette.GrassColor, Color.Black),
                2 => new TileFloor("Short grass", pos, "grass", '.', Palette.GrassColor, Color.Black),
                3 => new TileFloor("Grass", pos, "grass", ',', Palette.GrassColor, Color.Black),
                _ => null,
            };
#pragma warning restore CS8603 // Possível retorno de referência nula.
        }

        public static TileFloor GenericDirt(Point pos)
        {
            return new TileFloor("Dirt", pos, "dirt", '.', Palette.Dirt, Color.Transparent);
        }

        public static TileFloor GenericDirtRoad(Point pos)
        {
            return new TileFloor("Dirt road", pos, "dirt", '.', Palette.DirtRoad, Color.Transparent);
        }

        internal static TileWall GenericTree()
        {
            return new TileWall(Palette.Wood, Color.Black, 'O', "Tree", Point.None, "wood");
        }

        internal static TileWall GenericStoneWall()
        {
            return new TileWall(Point.None);
        }

        internal static TileFloor GenericStoneFloor()
        {
            return new TileFloor(Point.None);
        }
    }
}