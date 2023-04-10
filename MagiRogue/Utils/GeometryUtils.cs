using MagiRogue.Commands;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MagiRogue.Utils
{
    public static class GeometryUtils
    {
        /// <summary>
        /// Returns a cone
        /// </summary>
        /// <param name="originCoordinate">Where the cone begins</param>
        /// <param name="radius">The radius of the cone</param>
        /// <param name="target">The target that is creating the cone</param>
        /// <returns></returns>
        public static Shape Cone(this Point originCoordinate, in double radius, Target target, in double coneSpan = 90)
        {
            var map = GameLoop.GetCurrentMap();
            Point[] points;
            double angle = 0;
            if (target.TravelPath.Length > 0)
            {
                angle = Point.BearingOfLine(target.TravelPath.GetStep(0) - originCoordinate);
            }

            GoRogue.FOV.RecursiveShadowcastingFOV coneFov =
                new(map.TransparencyView);

            coneFov.Calculate(originCoordinate.X, originCoordinate.Y,
                radius, Distance.Chebyshev, angle, coneSpan);

            points = coneFov.CurrentFOV.ToArray();
#if DEBUG
            GameLoop.AddMessageLog(angle.ToString());
#endif
            return new Shape(points);
        }

        public static Shape HollowCircleFromOriginPoint(this in Point origin, in int radius)
        {
            int centerX = origin.X;
            int centerY = origin.Y;

            List<Point> points = new List<Point>();

            for (int angle = 0; angle < 360; angle++)
            {
                int x = (int)(centerX + (radius * Math.Cos(angle)));
                int y = (int)(centerY + (radius * Math.Sin(angle)));
                var point = new SadRogue.Primitives.Point(x, y);
                if (points.Contains(point))
                    continue;
                points.Add(point);
            }

            return new Shape(points.ToArray());
        }

        public static Shape CircleFromOriginPoint(this in Point origin, in int radius)
        {
            return new Shape(Radius.Circle.PositionsInRadius(origin, radius).ToArray());
        }
    }

    public readonly struct Shape
    {
        public readonly Point[] Points { get; }

        public Shape(Point[] points)
        {
            Points = points;
        }
    }

    /// <summary>
    /// Represents a simple 2d grid, does not hold any kind of objects and is there just to represent
    /// the space
    /// </summary>
    public struct Simple2DGrid
    {
        /// <summary>
        /// Also know as the rows
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Also know as the columns
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The total size of the grid
        /// </summary>
        public int Size { get; set; }

        public List<Point> Points { get; set; }

        public Simple2DGrid(int width, int height, List<Point> points)
        {
            Width = width;
            Height = height;
            Size = width * height;
            Points = points;
        }
    }
}