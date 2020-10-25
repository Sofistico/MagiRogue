using MagiRogue.Entities.Materials;
using MagiRogue.Utils;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MagiRogue.System.Tiles
{
    public class TileBase : Cell
    {
        // Movement and Line of Sight Flags
        public bool IsBlockingMove;
        public bool IsBlockingSight;
        public int Layer;

        // Creates a list of possible materials, and then assings it to the tile, need to move it to a fitting area, like
        // World or GameLoop, because if need to port, every new object will have more than one possible material without
        // any need.
        private static IList<Material> _listOfMaterials;
        public IEnumerable<Material> Material;

        // Tile's name
        public string Name;

        // TileBase is an abstract base class
        // representing the most basic form of of all Tiles used.
        // Every TileBase has a Foreground Colour, Background Colour, and Glyph
        // isBlockingMove and isBlockingSight are optional parameters, set to false by default
        public TileBase(Color foregroud, Color background, int glyph, int layer, bool blockingMove = false,
            bool blockingSight = false, string name = "") : base(foregroud, background, glyph)
        {
            IsBlockingMove = blockingMove;
            IsBlockingSight = blockingSight;
            Name = name;
            Layer = layer;
        }

        public void SetMaterial(string id)
        {
            _listOfMaterials = JsonUtils.JsonDeseralize<List<Material>>(Path.Combine
                (AppDomain.CurrentDomain.BaseDirectory.ToString(), "Entities", "Materials", "MaterialDefinition.json"));
            var foundMaterial = _listOfMaterials.Where(a => a.Id == $"{id}");
            Material = foundMaterial.ToList();
        }
    }
}