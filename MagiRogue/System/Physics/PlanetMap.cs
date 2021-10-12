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
        public float[,] Data { get; }
        public float Min { get; set; }
        public float Max { get; set; }

        public PlanetMap(int width, int height)
        {
            Data = new float[width, height];
            Min = float.MinValue;
            Max = float.MaxValue;
        }
    }
}