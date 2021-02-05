using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagiRogue.System.Tiles;
using Microsoft.Xna.Framework;
using GoRogue;

namespace MagiRogue.System.Physics
{
    // TODO: To be done
    public class PlanetTileGenerator : TileBase
    {
        public PlanetMapTileType TileType { get; set; }

        public PlanetTileGenerator(PlanetMapTileType tileType, Color foreground, Color background, int glyph, int layer,
            Coord position, string idOfMaterial, bool blocksMove, bool isTransparent, string nameOfTile)
            : base(foreground, background, glyph, layer, position, idOfMaterial, blocksMove, isTransparent, nameOfTile)
        {
            TileType = tileType;
        }
    }

    public enum PlanetMapTileType
    {
        Water,
        Land,
        Forest,
        Mountain,
        Wall,
        Road
    }
}