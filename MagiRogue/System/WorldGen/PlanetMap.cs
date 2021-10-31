using MagiRogue.System.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.WorldGen
{
    // made with http://www.jgallant.com/procedurally-generating-wrapping-world-maps-in-unity-csharp-part-1/#intro
    public class PlanetMap
    {
        private readonly int _height;
        private readonly int _width;

        public float[,] HeightData { get; }
        public float[,] HeatData { get; }
        public float[,] MoistureData { get; }
        public float Min { get; set; }
        public float Max { get; set; }
        public Map AssocietatedMap { get; }

        public PlanetMap(int width, int height)
        {
            _height = height;
            _width = width;
            HeightData = new float[width, height];
            HeatData = new float[width, height];
            MoistureData = new float[width, height];
            Min = float.MinValue;
            Max = float.MaxValue;
            AssocietatedMap = new("Planet", width, height);
        }

        public void SetWorldTiles(WorldTile[,] tiles)
        {
            AssocietatedMap.GoRogueComponents.GetFirstOrDefault<FOVHandler>().Disable();

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    AssocietatedMap.SetTerrain(tiles[x, y]);
                }
            }
        }
    }
}