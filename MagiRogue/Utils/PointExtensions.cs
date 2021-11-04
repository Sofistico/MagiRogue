using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;

namespace MagiRogue.Utils
{
    public static class PointExtensions
    {
        public static float PointMagnitude(this Point point) =>
            MathF.Sqrt((point.X * point.X) + (point.Y * point.Y));
    }
}