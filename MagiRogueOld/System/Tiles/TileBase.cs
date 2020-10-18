using System;
using System.Collections.Generic;
using System.Linq;
using SadConsole;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Tiles
{
    public class TileBase : Cell
    {
        // Movement and Line of Sight Flags

        public bool isBlockingMove;
        public bool isBlockingSight;

        // Tile's name
        public string Name;

        // TileBase is an abstract base class
        // representing the most basic form of of all Tiles used.
        // Every TileBase has a Foreground Colour, Background Colour, and Glyph
        // isBlockingMove and isBlockingSight are optional parameters, set to false by default
        public TileBase(Color foregroud, Color background, int glyph, bool blockingMove = false,
            bool blockingSight = false, string name = "") : base(foregroud, background, glyph)
        {
            isBlockingMove = blockingMove;
            isBlockingSight = blockingSight;
            Name = name;
        }
    }
}