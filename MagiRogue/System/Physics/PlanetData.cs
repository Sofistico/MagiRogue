using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagiRogue.System.Physics
{
    public class PlanetData
    {
        public float[,] Data;
        public float Min { get; set; }
        public float Max { get; set; }

        public float Gravity { get; set; }

        public PlanetData(int width, int heigth, float gravity = 9.80f)
        {
            Data = new float[width, heigth];
            Min = float.MinValue;
            Max = float.MaxValue;
            Gravity = gravity;
        }
    }
}