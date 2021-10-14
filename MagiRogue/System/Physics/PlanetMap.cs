using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Physics
{
    // made with http://www.jgallant.com/procedurally-generating-wrapping-world-maps-in-unity-csharp-part-1/#intro
    public class PlanetMap
    {
        public float[,] HeightData { get; }
        public float[,] HeatData { get; }
        public float Min { get; set; }
        public float Max { get; set; }

        public PlanetMap(int width, int height)
        {
            HeightData = new float[width, height];
            HeatData = new float[width, height];
            Min = float.MinValue;
            Max = float.MaxValue;
        }
    }
}