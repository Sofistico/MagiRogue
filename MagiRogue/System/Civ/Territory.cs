using MagiRogue.System.Tiles;
using MagiRogue.System.Planet;

namespace MagiRogue.System.Civ
{
    public class Territory
    {
        public WorldTileGroup OwnedLand { get; set; }
        public WorldTileGroup OwnedWater { get; set; }

        public Territory()
        {
            OwnedLand = new();
            OwnedWater = new();
            OwnedLand.Type = TileGroupType.Land;
            OwnedWater.Type = TileGroupType.Water;
        }

        public void AddLand(WorldTile tile) => OwnedLand.WorldTiles.Add(tile);

        public void AddWaterLand(WorldTile tile) => OwnedWater.WorldTiles.Add(tile);

        public int Count() => OwnedLand.WorldTiles.Count + OwnedWater.WorldTiles.Count;
    }
}