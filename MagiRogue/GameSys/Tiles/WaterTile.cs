using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.GameSys.Tiles
{
    public class WaterTile : TileBase
    {
        public bool IsSea { get; set; }
        public int SwinDifficulty { get; private set; } = 2;

        public WaterTile(Color foregroud, Color background, int glyph, Point position,
            bool isSea, int layer = (int)MapLayer.TERRAIN,
            string idOfMaterial = "h20", bool blocksMove = false, bool isTransparent = true,
            string name = "Water")
            : base(foregroud, background, glyph, layer, position, idOfMaterial,
                  blocksMove, isTransparent, name)
        {
            IsSea = isSea;
            if (IsSea)
                SwinDifficulty += 2;
        }

        public bool CanSwinThere(Actor actor)
        {
            if (actor.Abilities[((int)AbilityName.Swin)].Score > SwinDifficulty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static WaterTile NormalRiverWater(Point pos)
        {
            return new WaterTile(Palette.DeepWaterColor, Color.Transparent, '~', pos, false);
        }

        public static WaterTile NormalSeaWater(Point pos)
        {
            return new WaterTile(Palette.DeepWaterColor, Color.Transparent, '~', pos, true);
        }
    }
}