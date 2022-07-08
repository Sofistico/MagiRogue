using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;

namespace MagiRogue.Utils
{
    public static class PointUtils
    {
        public static float PointMagnitude(this Point point) =>
            MathF.Sqrt((point.X * point.X) + (point.Y * point.Y));

        public static IEnumerable<Point> GetTileLocationsAlongLine(int xOrigin,
            int yOrigin,
            int xDestination,
            int yDestination)
        {
            int dx = Math.Abs(xDestination - xOrigin);
            int dy = Math.Abs(yDestination - yOrigin);

            int sx = xOrigin < xDestination ? 1 : -1;
            int sy = yOrigin < yDestination ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                yield return new Point(xOrigin, yOrigin);
                if (xOrigin == xDestination && yOrigin == yDestination)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    xOrigin += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    yOrigin += sy;
                }
            }
        }
    }
}