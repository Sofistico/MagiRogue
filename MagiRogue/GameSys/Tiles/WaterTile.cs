using MagiRogue.Data.Enumerators;
using MagiRogue.Entities;
using SadRogue.Primitives;

namespace MagiRogue.GameSys.Tiles
{
    public sealed class WaterTile : TileBase
    {
        public bool IsSea { get; set; }
        public int SwinDifficulty { get; } = 2;

        public WaterTile(Color foregroud, Color background, int glyph, Point position,
            bool isSea, int layer = (int)MapLayer.TERRAIN,
            string idOfMaterial = "h2o", bool blocksMove = false, bool isTransparent = true,
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
            if (actor.Mind.Abilities[((int)AbilityName.Swin)].Score > SwinDifficulty)
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

        public override TileBase Copy()
        {
            throw new System.NotImplementedException();
        }
    }
}