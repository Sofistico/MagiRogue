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
        public static TileFloor ShortGrass(Point point)
        {
            return new TileFloor(
                           "Grass",
                            point,
                            "grass",
                            (int)',',
                            Color.Green,
                            Color.Black);
        }
    }
}